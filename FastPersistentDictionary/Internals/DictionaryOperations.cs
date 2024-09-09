using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using FastPersistentDictionary.Internals.Accessor;
using FastPersistentDictionary.Internals.Compression;

namespace FastPersistentDictionary.Internals
{
    public sealed class DictionaryOperations<TKey, TValue>
    {
        private readonly object _lockObj;
        private readonly System.Timers.Timer _updateTimer;
        private readonly ICompressionHandler<TKey, TValue> _compressionHandler;
        internal FileStream FileStream;

        public DictionaryOperations(
            FastPersistentDictionary<TKey, TValue> dict,
            DictionaryAccessor<TKey, TValue> dictionaryAccessor,
            System.Timers.Timer updateTimer,
            ICompressionHandler<TKey, TValue> compressionHandler,
            object lockObj,
            FileStream fileStream)
        {
            _persistentDictionaryPro = dict;
            _dictionaryAccessor = dictionaryAccessor;
            _updateTimer = updateTimer;
            _compressionHandler = compressionHandler;
            _lockObj = lockObj;
            FileStream = fileStream;
        }

        private readonly FastPersistentDictionary<TKey, TValue> _persistentDictionaryPro;
        private readonly DictionaryAccessor<TKey, TValue> _dictionaryAccessor;

        public IOrderedEnumerable<KeyValuePair<TKey, TValue>> OrderByDescending(Func<KeyValuePair<TKey, TValue>, object> selector)
        {
            var allEntries = new List<KeyValuePair<TKey, TValue>>();
            lock (_lockObj)
                foreach (var key in _persistentDictionaryPro.DictionarySerializedLookup.Keys)
                    allEntries.Add(new KeyValuePair<TKey, TValue>(key, _dictionaryAccessor.Get(key)));
            
            return allEntries.OrderByDescending(selector);
        }

        public IOrderedEnumerable<KeyValuePair<TKey, TValue>> OrderBy(Func<KeyValuePair<TKey, TValue>, object> selector)
        {
            var allEntries = new List<KeyValuePair<TKey, TValue>>();
            lock (_lockObj)
                foreach (var key in _persistentDictionaryPro.DictionarySerializedLookup.Keys)
                    allEntries.Add(new KeyValuePair<TKey, TValue>(key, _dictionaryAccessor.Get(key)));
            
            return allEntries.OrderBy(selector);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public List<KeyValuePair<TKey, TValue>> Reverse()
        {
            var allEntries = new List<KeyValuePair<TKey, TValue>>();
            lock (_lockObj)
                foreach (var key in _persistentDictionaryPro.DictionarySerializedLookup.Keys)
                    allEntries.Add(new KeyValuePair<TKey, TValue>(key, _dictionaryAccessor.Get(key)));
            
            allEntries.Reverse();
            return allEntries;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public KeyValuePair<TKey, TValue>[] ToArray()
        {
            lock (_lockObj)
            {
                var array = new KeyValuePair<TKey, TValue>[_persistentDictionaryPro.DictionarySerializedLookup.Count];
                var index = 0;

                foreach (var item in _persistentDictionaryPro.DictionarySerializedLookup)
                {
                    var data = new byte[item.Value.Value];
                    FileStream.Seek(item.Value.Key, SeekOrigin.Begin);
                    FileStream.Read(data, 0, item.Value.Value);

                    array[index++] = new KeyValuePair<TKey, TValue>(item.Key, _compressionHandler.Deserialize<TValue>(data));
                }
                return array;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public List<KeyValuePair<TKey, TValue>> ToList()
        {
            return ToArray().ToList();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<KeyValuePair<TKey, TValue>> Take(int count)
        {
            var startIndex = 0;
            foreach (var item in _dictionaryAccessor)
            {
                if (startIndex++ == count)
                    break;

                yield return item;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<KeyValuePair<TKey, TValue>> TakeWhile(Func<KeyValuePair<TKey, TValue>, bool> predicate)
        {
            foreach (var item in _dictionaryAccessor)
            {
                if (predicate(item) == false)
                    break;

                yield return item;
            }
        }

        /// <summary>
        /// `Copy()`: Creates a shallow copy of the dictionary.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FastPersistentDictionary<TKey, TValue> Copy(string path)
        {
            lock (_lockObj)
            {
                var copiedDictionary = new FastPersistentDictionary<TKey, TValue>(
                    path,
                    //_persistentDictionaryPro.CrashRecovery,
                    (int)_updateTimer.Interval,
                    _persistentDictionaryPro.PercentageChangeBeforeCompact,
                    8196,
                    "",
                    _persistentDictionaryPro.Comparer);

                _updateTimer.Stop();


                foreach (var kvp in _persistentDictionaryPro.DictionarySerializedLookup.Keys)
                    copiedDictionary.Add(kvp, _persistentDictionaryPro.DictionaryAccessor.Get(kvp));

                _updateTimer.Start();

                return copiedDictionary;
            }
        }

        /// <summary>
        /// SkipLast: This skips the last `n` items in the sequence
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<TKey, TValue>> SkipLast(int number)
        {
            lock (_lockObj)
            {
                var itemCount = _persistentDictionaryPro.DictionarySerializedLookup.Count;

                if (itemCount <= number)
                    return Enumerable.Empty<KeyValuePair<TKey, TValue>>();

                var results = new List<KeyValuePair<TKey, TValue>>(itemCount - number);
                var index = 0;
                var len = _persistentDictionaryPro.Count - number;

                foreach (var entry in _persistentDictionaryPro)
                {
                    if (index < len)
                    {
                        ++index;
                        results.Add(new KeyValuePair<TKey, TValue>(entry.Key, _persistentDictionaryPro.DictionaryAccessor.Get(entry.Key)));
                    }
                    else
                        break;
                }

                return results;
            }
        }


        public void Join(Dictionary<TKey, TValue> other, bool overwriteExistingKeys = true) => Merge(other, overwriteExistingKeys);
        public void Join(FastPersistentDictionary<TKey, TValue> other, bool overwriteExistingKeys = true) => Merge(other, overwriteExistingKeys);

        /// <summary>
        /// `Merge(Dictionary<Key, Value> other)`: Import all entries from another Dictionary, has an optional parameter for overwriting existing keys. 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="overwriteExistingKeys"></param>
        public void Merge(Dictionary<TKey, TValue> other, bool overwriteExistingKeys = true)
        {
            lock (_lockObj)
                foreach (var entry in other)
                {
                    if (overwriteExistingKeys == false && _persistentDictionaryPro.ContainsKey(entry.Key))
                        continue;

                    _persistentDictionaryPro.DictionaryAccessor.Set(entry.Key, entry.Value);
                }
        }

        /// <summary>
        /// `Merge(PersistentDictionaryPro<Key, Value> other)`: Import all entries from another PersistentDictionary, has an optional parameter for overwriting existing keys. 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="overwriteExistingKeys"></param>
        public void Merge(FastPersistentDictionary<TKey, TValue> other, bool overwriteExistingKeys = true)
        {
            lock (_lockObj)
                foreach (var entry in other)
                {
                    if (overwriteExistingKeys == false && _persistentDictionaryPro.ContainsKey(entry.Key))
                        continue;

                    _persistentDictionaryPro.DictionaryAccessor.Set(entry.Key, entry.Value);
                }
        }

        /// <summary>
        /// `Invert()`: If all values are unique, invert the dictionary so that the keys become values and vice versa.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public FastPersistentDictionary<TValue, TKey> Invert()
        {
            lock (_lockObj)
            {
                var invertedDictionary = new FastPersistentDictionary<TValue, TKey>(_persistentDictionaryPro.FileLocation);

                var valuesSet = new HashSet<TValue>(_persistentDictionaryPro.Values);

                if (_persistentDictionaryPro.Values.Count() != valuesSet.Count)
                    throw new InvalidOperationException(
                        "Cannot invert PersistentDictionaryPro: not all values are unique.");

                foreach (var key in _persistentDictionaryPro.Keys)
                    invertedDictionary.Add(_persistentDictionaryPro.DictionaryAccessor.Get(key), key);

                return invertedDictionary;
            }
        }

        public IEnumerable<KeyValuePair<TKey, TValue>> Skip(int number)
        {
            if (number < 0)
                throw new ArgumentOutOfRangeException(nameof(number), "Number of elements to skip cannot be negative.");

            var count = 0;
            lock (_lockObj)
                foreach (var key in _persistentDictionaryPro.Keys)
                {
                    if (count++ < number)
                        continue;

                    yield return new KeyValuePair<TKey, TValue>(key, _dictionaryAccessor.Get(key));
                }
        }

        public IEnumerable<KeyValuePair<TKey, TValue>> SkipWhile(Func<KeyValuePair<TKey, TValue>, bool> predicate)
        {
            lock (_lockObj)
            {
                var skipping = true;

                foreach (var key in _persistentDictionaryPro.Keys)
                {
                    var entry = new KeyValuePair<TKey, TValue>(key, _dictionaryAccessor.Get(key));
                    switch (skipping)
                    {
                        case true when predicate(entry):
                            continue;
                        case true:
                            skipping = false;
                            break;
                    }

                    yield return entry;
                }
            }
        }

        public IEnumerable<KeyValuePair<TKey, TValue>> DistinctByKey()
        {
            var distinctKeys = new HashSet<TKey>();
            lock (_lockObj)
                foreach (var pair in _dictionaryAccessor)
                    if (distinctKeys.Add(pair.Key))
                        yield return pair;
        }

        public IEnumerable<KeyValuePair<TKey, TValue>> Except(FastPersistentDictionary<TKey, TValue> otherDict)
        {
            var otherDictAsRegularDict = new Dictionary<TKey, TValue>();
            lock (_lockObj)
            {
                foreach (var pair in otherDict)
                    otherDictAsRegularDict.Add(pair.Key, pair.Value);

                foreach (var pair in _dictionaryAccessor)
                    if (otherDictAsRegularDict.ContainsKey(pair.Key) == false)
                        yield return pair;
            }
        }

        public void Concat(FastPersistentDictionary<TKey, TValue> other)
        {
            lock (_lockObj)
                foreach (var keyValuePair in _persistentDictionaryPro)
                {
                    var key = keyValuePair.Key;
                    _dictionaryAccessor.Add(key, other.DictionaryAccessor.Get(key));
                }
        }

        public FastPersistentDictionary<TKey, TValue> Intersect(FastPersistentDictionary<TKey, TValue> other)
        {
            lock (_lockObj)
            {
                var intersection =
                    new FastPersistentDictionary<TKey, TValue>(_persistentDictionaryPro.FileLocation);

                foreach (var pair in _dictionaryAccessor)
                    if (other.DictionaryAccessor.ContainsKey(pair.Key))
                        intersection.DictionaryAccessor.Add(pair.Key, pair.Value);

                return intersection;
            }
        }

        public FastPersistentDictionary<TKey, TValue> Union(FastPersistentDictionary<TKey, TValue> other)
        {
            lock (_lockObj)
            {
                var unionDictionary =
                    new FastPersistentDictionary<TKey, TValue>(_persistentDictionaryPro.FileLocation);


                foreach (var kvp in _persistentDictionaryPro.DictionarySerializedLookup.Keys)
                    unionDictionary.DictionaryAccessor.Add(kvp, _persistentDictionaryPro.DictionaryAccessor.Get(kvp));

                foreach (var kvp in _persistentDictionaryPro.DictionarySerializedLookup.Keys)
                    if (unionDictionary.DictionaryAccessor.ContainsKey(kvp) == false)
                        unionDictionary.DictionaryAccessor.Add(kvp, other.DictionaryAccessor.Get(kvp));

                return unionDictionary;
            }
        }

        /// <summary>
        ///  InBatchesOf: This method can process your sequence in batches of specified size.
        /// </summary>
        /// <param name="batchSize"></param>
        /// <returns></returns>
        public IEnumerable<IEnumerable<KeyValuePair<TKey, TValue>>> InBatchesOf(int batchSize)
        {
            var batch = new List<KeyValuePair<TKey, TValue>>();
            lock (_lockObj)
            {
                foreach (var key in _persistentDictionaryPro.Keys)
                {
                    batch.Add(new KeyValuePair<TKey, TValue>(key, _persistentDictionaryPro[key]));
                    if (batch.Count != batchSize)
                        continue;

                    yield return batch;
                    batch = new List<KeyValuePair<TKey, TValue>>();
                }

                if (batch.Any())
                    yield return batch;

            }
        }

        //Todo: this can be optimized more, got lazy
        /// <summary>
        ///  TakeLast: This method returns the last `n` items in the sequence.
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<TKey, TValue>> TakeLast(int number)
        {
            lock (_lockObj)
            {
                if (number > _persistentDictionaryPro.Count)
                    number = _persistentDictionaryPro.Count;

                foreach (var key in _persistentDictionaryPro.DictionarySerializedLookup.Keys.Skip(Math.Max(0, _persistentDictionaryPro.DictionarySerializedLookup.Keys.Count() - number)))
                    yield return new KeyValuePair<TKey, TValue>(key, _dictionaryAccessor.Get(key));
            }
        }

        public FastPersistentDictionary<TKey, Tuple<TValue, TSecond>> Zip<TSecond>(FastPersistentDictionary<TKey, TSecond> second)
        {
            lock (_lockObj)
            {
                var result = new FastPersistentDictionary<TKey, Tuple<TValue, TSecond>>(_persistentDictionaryPro.FileLocation, (int)_updateTimer.Interval);

                foreach (var key in _dictionaryAccessor.Keys)
                    if (second.DictionaryAccessor.TryGetValue(key, out var secondValue))
                        result.DictionaryAccessor.Add(key, Tuple.Create(_dictionaryAccessor.Get(key), secondValue));
                
                return result;
            }
        }

        public ImmutableList<KeyValuePair<TKey, TValue>> ToImmutableList() => ToArray().ToImmutableList();

        public ImmutableArray<KeyValuePair<TKey, TValue>> ToImmutableArray() => ToArray().ToImmutableArray();

        public ImmutableDictionary<TKey, TValue> ToImmutableDictionary() => ToArray().ToImmutableDictionary();

        public ImmutableHashSet<KeyValuePair<TKey, TValue>> ToImmutableHashSet() => ToArray().ToImmutableHashSet();

        public SortedSet<KeyValuePair<TKey, TValue>> ToSortedSet()
        {
            lock (_lockObj)
                return new SortedSet<KeyValuePair<TKey, TValue>>(ToArray(), new KeyValuePairComparer<TKey, TValue>());
        }

        public SortedDictionary<TKey, TValue> ToSortedDictionary()
        {
            var sortedDictionary = new SortedDictionary<TKey, TValue>();
            foreach (var keyValuePair in _dictionaryAccessor)
                sortedDictionary.Add(keyValuePair.Key, keyValuePair.Value);

            return sortedDictionary;
        }

        public IQueryable<KeyValuePair<TKey, TValue>> AsQueryable() => ToArray().AsQueryable();

        public ParallelQuery<KeyValuePair<TKey, TValue>> AsParallel()
        {
            lock (_lockObj)
            {
                var lookup = _persistentDictionaryPro.DictionarySerializedLookup.AsParallel().Select(kv => new KeyValuePair<TKey, TValue>(kv.Key, _dictionaryAccessor.Get(kv.Key)));
                return lookup;
            }
        }

        public void Shuffle()
        {
            var list = new List<KeyValuePair<TKey, TValue>>();
            lock (_lockObj)
            {
                foreach (var kvp in _persistentDictionaryPro.DictionarySerializedLookup.Keys)
                    list.Add(new KeyValuePair<TKey, TValue>(kvp, _persistentDictionaryPro.DictionaryAccessor.Get(kvp)));

                var shuffledList = list.OrderBy(x => Guid.NewGuid()).ToList();

                _persistentDictionaryPro.DictionarySerializedLookup.Clear();
                FileStream.SetLength(0);
                FileStream.Seek(0, SeekOrigin.Begin);

                foreach (var kvp in shuffledList)
                {
                    var compressedData = _compressionHandler.Serialize(kvp.Value);
                    var location = new KeyValuePair<long, int>(FileStream.Position, compressedData.Length);
                    FileStream.Write(compressedData, 0, compressedData.Length);

                    _persistentDictionaryPro.DictionarySerializedLookup[kvp.Key] = location;
                }
            }
        }
    }
}