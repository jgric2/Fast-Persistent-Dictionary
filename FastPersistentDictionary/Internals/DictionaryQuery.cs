using System.Runtime.CompilerServices;
using System.Timers;
using FastPersistentDictionary.Internals.Accessor;
using FastPersistentDictionary.Internals.Compression;
using Timer = System.Timers.Timer;

namespace FastPersistentDictionary.Internals
{
    public sealed class DictionaryQuery<TKey, TValue>
    {
        private readonly object _lockObj;
        private readonly Timer _updateTimer;
        private readonly Timer _idleCompactTimer;

        private readonly ICompressionHandler<TKey, TValue> _compressionHandler;
        private const int IdleCompactTime = 1000;
        private readonly float _percentageDiffBeforeCompact;
        public long LastFileSizeCompact { get; set; }
        public long NextFileSizeCompact { get; set; }
      //  private bool _canCompact;

        internal FileStream FileStream;
        public DictionaryQuery(
            FastPersistentDictionary<TKey, TValue> dict,
            DictionaryAccessor<TKey, TValue> dictionaryAccessor,
            Timer updateTimer,
            ICompressionHandler<TKey, TValue> compressionHandler,
            float percentageDiffBeforeCompact,
            object lockObj,
            FileStream fileStream)
        {
            FileStream = fileStream;
            _fastPersistentDictionary = dict;
            _dictionaryAccessor = dictionaryAccessor;
            _updateTimer = updateTimer;
            _compressionHandler = compressionHandler;
            _lockObj = lockObj;
            _percentageDiffBeforeCompact = percentageDiffBeforeCompact;
            LastFileSizeCompact = fileStream.Length;


            NextFileSizeCompact = LastFileSizeCompact + (long)(LastFileSizeCompact * (_percentageDiffBeforeCompact / 100f));

            _idleCompactTimer = new Timer(IdleCompactTime + _updateTimer.Interval);
            _idleCompactTimer.Elapsed += UpdateCompactTimerElapsedEventHandler;
            _idleCompactTimer.Enabled = false;
        }

        private readonly FastPersistentDictionary<TKey, TValue> _fastPersistentDictionary;
        private readonly DictionaryAccessor<TKey, TValue> _dictionaryAccessor;

        private void UpdateCompactTimerElapsedEventHandler(object sender, ElapsedEventArgs e)
        {
            IdleCompactTimerEvent();
        }

        public bool All(Func<KeyValuePair<TKey, TValue>, bool> predicate)
        {
            lock (_lockObj)
                foreach (var entry in _fastPersistentDictionary.DictionarySerializedLookup.Keys)
                    if (predicate(new KeyValuePair<TKey, TValue>(entry, _dictionaryAccessor.Get(entry))) == false)
                        return false;

            return true;
        }

        public bool Any()
        {
            lock (_lockObj)
                return _fastPersistentDictionary.DictionarySerializedLookup.Count > 0;
        }

        public bool Any(Func<TKey, TValue, bool> predicate)
        {
            lock (_lockObj)
                foreach (var key in _fastPersistentDictionary.DictionarySerializedLookup.Keys)
                    if (predicate(key, _dictionaryAccessor.Get(key)))
                        return true;


            return false;
        }

        public void IdleCompactTimerEvent()
        {
            _idleCompactTimer.Stop();
            //if (_canCompact)
                CompactDatabaseFile();
        }


        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public async Task CompactDatabaseFile()//Async()
        //{
        //    Stopwatch stpw = Stopwatch.StartNew();
        //    Console.WriteLine("compact");
        //    var tempFilePath = Path.GetTempFileName();

        //        _fastPersistentDictionary.CanCompact = false;
        //        _idleCompactTimer.Stop();

        //        var tempDict = new Dictionary<TKey, KeyValuePair<long, int>>();

        //        using (var tempFileStream = new FileStream(tempFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite, bufferSize: 1024 * 1024, useAsync: true))
        //        {
        //            List<Task> tasks = new();
        //        //lock (_lockObj)
        //        //{
        //            foreach (var kvp in _fastPersistentDictionary.DictionarySerializedLookup)
        //            {
        //                tasks.Add(Task.Run(async () =>
        //                {
        //                    byte[] buffer = new byte[kvp.Value.Value];

        //                    lock (_lockObj)
        //                    {
        //                        FileStream.Seek(kvp.Value.Key, SeekOrigin.Begin);
        //                    }

        //                    int bytesRead = await FileStream.ReadAsync(buffer, 0, kvp.Value.Value).ConfigureAwait(false);
        //                    await tempFileStream.WriteAsync(buffer, 0, bytesRead).ConfigureAwait(false);

        //                    lock (_lockObj)
        //                    {
        //                        tempDict[kvp.Key] = new KeyValuePair<long, int>(tempFileStream.Position - kvp.Value.Value, kvp.Value.Value);
        //                    }
        //                }));
        //            }
        //        //}

        //            Task.WhenAll(tasks);

        //            // Replace the old dictionary with the updated one
        //            _fastPersistentDictionary.DictionarySerializedLookup = tempDict;

        //            FileStream.SetLength(0);
        //            FileStream.Seek(0, SeekOrigin.Begin);
        //            tempFileStream.Seek(0, SeekOrigin.Begin);
        //        tempFileStream.CopyTo(FileStream);

        //            LastFileSizeCompact = FileStream.Length;
        //            NextFileSizeCompact = LastFileSizeCompact + (long)(LastFileSizeCompact * (_percentageDiffBeforeCompact / 100f));
        //        }


        //    if (File.Exists(tempFilePath))
        //        File.Delete(tempFilePath);

        //    stpw.Stop();
        //    Console.WriteLine(stpw.ElapsedMilliseconds);
        //}


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CompactDatabaseFile()
        {
            //Stopwatch stpw = Stopwatch.StartNew();
            //Console.WriteLine("compact");
            var tempFilePath = Path.GetTempFileName();
            lock (_lockObj)
            {
                _fastPersistentDictionary.CanCompact = false;
                // _canCompact = false;
                _idleCompactTimer.Stop();

                using (var tempFileStream = new FileStream(tempFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.None, bufferSize: 1024 * 1024, FileOptions.SequentialScan | FileOptions.DeleteOnClose))
                {
                    Span<byte> buffer = new Span<byte>();

                    var tempDict = new Dictionary<TKey, KeyValuePair<long, int>>(_fastPersistentDictionary.DictionarySerializedLookup.Count);
                    foreach (var kvp in _fastPersistentDictionary.DictionarySerializedLookup)
                    {
                        FileStream.Seek(kvp.Value.Key, SeekOrigin.Begin);

                        var bytesLeft = kvp.Value.Value;
                        buffer = new byte[bytesLeft];


                        var bytesRead = FileStream.Read(buffer);
                        tempFileStream.Write(buffer);
                        tempDict[kvp.Key] = new KeyValuePair<long, int>(tempFileStream.Position - kvp.Value.Value, kvp.Value.Value);
                    }

                    // Replace the old dictionary with the updated one
                    _fastPersistentDictionary.DictionarySerializedLookup = tempDict;

                    FileStream.SetLength(0);
                    FileStream.Seek(0, SeekOrigin.Begin);
                    tempFileStream.Seek(0, SeekOrigin.Begin);
                    tempFileStream.CopyTo(FileStream);

                    LastFileSizeCompact = FileStream.Length;
                    NextFileSizeCompact = LastFileSizeCompact + (long)(LastFileSizeCompact * (_percentageDiffBeforeCompact / 100f));
                }
            }

            //if (File.Exists(tempFilePath))
            //    File.Delete(tempFilePath);

            //stpw.Stop();
            //Console.WriteLine(stpw.ElapsedMilliseconds);
        }


        /// <summary>
        /// `CountValues(Value value)`: Counts the number of keys associated with a particular value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CountValues(TValue value)
        {
            var count = 0;
            lock (_lockObj)
                foreach (var kvp in _fastPersistentDictionary.DictionarySerializedLookup.Values)
                {
                    var data = new byte[kvp.Value];
                    FileStream.Seek(kvp.Key, SeekOrigin.Begin);
                    FileStream.Read(data, 0, kvp.Value);

                    if (_compressionHandler.Deserialize<TValue>(data).Equals(value))
                        ++count;
                }

            return count;
        }

        public KeyValuePair<TKey, TValue> ElementAt(int index)
        {
            lock (_lockObj)
            {
                if (index < 0 || index >= _fastPersistentDictionary.DictionarySerializedLookup.Count)
                    throw new ArgumentOutOfRangeException(nameof(index));

                var valuePair = _fastPersistentDictionary.DictionarySerializedLookup.ElementAt(index);

                FileStream.Seek(valuePair.Value.Key, SeekOrigin.Begin);
                var data = new byte[valuePair.Value.Value];
                FileStream.Read(data, 0, valuePair.Value.Value);
                var value = _compressionHandler.Deserialize<TValue>(data);

                return new KeyValuePair<TKey, TValue>(valuePair.Key, value);
            }
        }

        /// <summary>
        /// `FilteredWhere(Func<Key, Value, bool> predicate)`: Query the dictionary, returning a new dictionary containing only key/value pairs that match the predicate.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public FastPersistentDictionary<TKey, TValue> FilteredWhere(Func<TKey, TValue, bool> predicate)
        {
            lock (_lockObj)
            {
                var result = new FastPersistentDictionary<TKey, TValue>(_fastPersistentDictionary.FileLocation, equalityComparer: _fastPersistentDictionary.Comparer);
                foreach (var kvp in _fastPersistentDictionary)
                    if (predicate(kvp.Key, kvp.Value))
                        result.Add(kvp.Key, kvp.Value);
                
                return result;
            }
        }

        public KeyValuePair<TKey, TValue> First()
        {
            lock (_lockObj)
            {
                UpdateDatabaseTick();

                var key = _fastPersistentDictionary.DictionarySerializedLookup.Keys.First();
                var value = _dictionaryAccessor.Get(key);
                return new KeyValuePair<TKey, TValue>(key, value);
            }
        }

        public KeyValuePair<TKey, TValue>? FirstOrDefault()
        {
            lock (_lockObj)
            {
                UpdateDatabaseTick();

                var key = _fastPersistentDictionary.DictionarySerializedLookup.Keys.FirstOrDefault();
                if (key == null)
                    return null;

                var value = _dictionaryAccessor.GetReturnDefaultIfNotFound(key);

                return new KeyValuePair<TKey, TValue>(key, value);
            }
        }

        /// <summary>
        /// `FindAllKeysByValue(Value value)`: Finds all keys in the dictionary by its value Returning a list.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public List<TKey> FindAllKeysByValue(TValue value)
        {
            lock (_lockObj)
            {
                var keys = new List<TKey>();
                // Check the file database
                foreach (var entry in _fastPersistentDictionary.DictionarySerializedLookup.Keys)
                {
                    var serializedKey = entry;
                    if (EqualityComparer<TValue>.Default.Equals(_dictionaryAccessor.Get(serializedKey), value))
                        keys.Add(serializedKey);
                }

                return keys;
            }
        }

        /// <summary>
        /// `FindKeyByValue(Value value)`: Finds a key in the dictionary by its value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public TKey FindKeyByValue(TValue value)
        {
            lock (_lockObj)
            {
                foreach (var entry in _fastPersistentDictionary.DictionarySerializedLookup.Keys)
                {
                    var serializedKey = entry;
                    if (EqualityComparer<TValue>.Default.Equals(_dictionaryAccessor.Get(serializedKey), value))
                        return serializedKey;
                }

                throw new KeyNotFoundException("The key for the given value was not found in the dictionary.");
            }
        }

        /// <summary>
        /// `GetSubset(List<Key> keys)`: Returns a new Dictionary that is a subset of the original Dictionary and only includes the keys provided. 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FastPersistentDictionary<TKey, TValue> GetSubset(Func<TKey, TValue, bool> predicate)
        {
            lock (_lockObj)
            {
                var newDict = new FastPersistentDictionary<TKey, TValue>(_fastPersistentDictionary.FileLocation);
                foreach (var kvp in _fastPersistentDictionary)
                {
                    if (predicate(kvp.Key, kvp.Value))
                        newDict.Add(kvp.Key, kvp.Value);
                }

                return newDict;
            }
        }

        /// <summary>
        /// `GetSubset(List<Key> keys)`: Returns a new Dictionary that is a subset of the original Dictionary and only includes the keys provided. 
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public FastPersistentDictionary<TKey, TValue> GetSubset(List<TKey> keys)
        {
            lock (_lockObj)
            {
                var newDict = new FastPersistentDictionary<TKey, TValue>(_fastPersistentDictionary.FileLocation);

                foreach (var key in keys)
                    if (_fastPersistentDictionary.ContainsKey(key))
                        newDict.Add(key, _dictionaryAccessor.Get(key));

                return newDict;
            }
        }

        public IDictionary<TKey, (TValue value, int index)> Index()
        {
            lock (_lockObj)
            {
                IDictionary<TKey, (TValue value, int index)> indexDictionary = new Dictionary<TKey, (TValue value, int index)>();
                var index = 0;

                foreach (var key in _fastPersistentDictionary.DictionarySerializedLookup.Keys)
                    indexDictionary.Add(key, (_dictionaryAccessor.Get(key), index++));

                return indexDictionary;
            }
        }

        public KeyValuePair<TKey, TValue> Last()
        {
            lock (_lockObj)
            {
                UpdateDatabaseTick();

                var lastKey = _fastPersistentDictionary.DictionarySerializedLookup.Keys.LastOrDefault();
                if (lastKey != null)
                    return new KeyValuePair<TKey, TValue>(lastKey, _dictionaryAccessor.Get(lastKey));

                throw new InvalidOperationException("No elements in the dictionary.");
            }
        }

        public KeyValuePair<TKey, TValue>? LastOrDefault()
        {
            lock (_lockObj)
            {
                UpdateDatabaseTick();

                var lastKey = _fastPersistentDictionary.DictionarySerializedLookup.Keys.LastOrDefault();
                if (lastKey != null)
                    return new KeyValuePair<TKey, TValue>(lastKey, _dictionaryAccessor.GetReturnDefaultIfNotFound(lastKey));

                return null;
            }
        }

        public (IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>) Partition(Func<KeyValuePair<TKey, TValue>, bool> predicate)
        {
            var trueList = new List<KeyValuePair<TKey, TValue>>();
            var falseList = new List<KeyValuePair<TKey, TValue>>();

            lock (_lockObj)
            {
                foreach (var key in _fastPersistentDictionary.DictionarySerializedLookup.Keys)
                {
                    var pair = new KeyValuePair<TKey, TValue>(key, _dictionaryAccessor.Get(key));
                    if (predicate(pair))
                        trueList.Add(pair);
                    else
                        falseList.Add(pair);
                }
            }
            return (trueList, falseList);
        }

        /// <summary>
        /// Peek: This method can be used to perform an action on each item in the sequence while iterating without changing the elements in the collection.
        /// </summary>
        /// <param name="action"></param>
        public void Peek(Action<KeyValuePair<TKey, TValue>> action)
        {
            lock (_lockObj)
            {
                foreach (var key in _fastPersistentDictionary.DictionarySerializedLookup.Keys)
                    action(new KeyValuePair<TKey, TValue>(key, _dictionaryAccessor.Get(key)));
            }
        }

        public IEnumerable<TResult> Select<TResult>(Func<KeyValuePair<TKey, TValue>, TResult> selector)
        {
            lock (_lockObj)
            {
                var results = new List<TResult>();

                foreach (var key in _fastPersistentDictionary.DictionarySerializedLookup.Keys)
                    results.Add(selector(new KeyValuePair<TKey, TValue>(key, _dictionaryAccessor.Get(key))));

                return results;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public KeyValuePair<TKey, TValue> Single(Func<KeyValuePair<TKey, TValue>, bool> predicate)
        {
            lock (_lockObj)
            {
                KeyValuePair<TKey, TValue> foundPair = default;
                var found = false;

                foreach (var entry in _fastPersistentDictionary.DictionarySerializedLookup.Keys)
                {
                    var pair = new KeyValuePair<TKey, TValue>(entry, _dictionaryAccessor.Get(entry));

                    if (predicate != null && predicate(pair) == false)
                        continue;

                    if (found)
                        throw new InvalidOperationException("More than one element satisfies the condition.");

                    foundPair = pair;
                    found = true;
                }

                if (found == false)
                    throw new InvalidOperationException("The source sequence is empty.");

                return foundPair;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public KeyValuePair<TKey, TValue> SingleOrDefault(Func<KeyValuePair<TKey, TValue>, bool> predicate)
        {
            lock (_lockObj)
            {
                KeyValuePair<TKey, TValue> foundPair = default;
                var found = false;

                foreach (var entry in _fastPersistentDictionary.DictionarySerializedLookup.Keys)
                {
                    var pair = new KeyValuePair<TKey, TValue>(entry, _dictionaryAccessor.Get(entry));

                    if (predicate != null && predicate(pair) == false)
                        continue;

                    if (found)
                        return default;

                    foundPair = pair;
                    found = true;
                }

                return found ? foundPair : default;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UpdateDatabaseTick(bool forceCompact = false)
        {
            lock (_lockObj)
            {
                _updateTimer.Stop();
            }

            if (forceCompact)
            {
                CompactDatabaseFile();
            }

            //if (forceCompact == false && (_canCompact == false || FileStream.Length < NextFileSizeCompact))
            if (FileStream.Length > NextFileSizeCompact)
            {

                //Task.Run(async () =>
                //{
                    CompactDatabaseFile();
               // });
                
                //_idleCompactTimer.Stop();
                //_idleCompactTimer.Start();
                // return;
            }


            //lock (_lockObj)
            //{
            //    _updateTimer.Start();
            //}
            //if zero percent this means we ONLY compact on the idle timer.
            //if (_percentageDiffBeforeCompact != 0)
            //    CompactDatabaseFile();
        }

        public IEnumerable<KeyValuePair<TKey, TValue>> Where(Func<TKey, TValue, bool> predicate)
        {
            lock (_lockObj)
            {
                foreach (var key in _fastPersistentDictionary.DictionarySerializedLookup.Keys)
                {
                    var value = _dictionaryAccessor.Get(key);
                    if (predicate(key, value))
                        yield return new KeyValuePair<TKey, TValue>(key, value);
                }
            }
        }
    }
}