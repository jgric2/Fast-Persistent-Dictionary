using FastPersistentDictionary.Internals.Compression;
using FastPersistentDictionary;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace FastPersistentDictionary.Internals.Accessor
{
    public sealed class DictionaryAccessor_RecoverMode<TKey, TValue> : IDictionaryAccessor<TKey, TValue>
    {
        private readonly ICompressionHandler<TKey, TValue> _compressionHandler;

        private readonly object _lockObj;
        private readonly FastPersistentDictionary<TKey, TValue> _persistentDictionaryPro;
        private readonly System.Timers.Timer _updateTimer;
        internal FileStream FileStream;

        internal FileStream FileStream_Keys;

        public DictionaryAccessor_RecoverMode(
            FastPersistentDictionary<TKey, TValue> dict,
            System.Timers.Timer updateTimer,
            ICompressionHandler<TKey, TValue> compressionHandler,
            object lockObj,
            FileStream fileStream,
            FileStream fileStream_Keys)
        {
            _lockObj = lockObj;
            FileStream = fileStream;
            FileStream_Keys = fileStream_Keys;
            _persistentDictionaryPro = dict;
            _compressionHandler = compressionHandler;
            _updateTimer = updateTimer;
        }

        public IEqualityComparer<TKey> Comparer { get; set; }

        public int Count => _persistentDictionaryPro.DictionarySerializedLookup.Count;

        public IEnumerable<TKey> Keys => _persistentDictionaryPro.DictionarySerializedLookup.Keys;

        public IEnumerable<TValue> Values
        {
            get
            {
                lock (_lockObj)
                {
                    foreach (var kvp in _persistentDictionaryPro.DictionarySerializedLookup.Values)
                    {
                        var data = new byte[kvp.Value];
                        FileStream.Seek(kvp.Key, SeekOrigin.Begin);
                        FileStream.Read(data, 0, kvp.Value);

                        yield return _compressionHandler.Deserialize<TValue>(data);
                    }
                }
            }
        }

        public TValue this[TKey key]
        {
            get => Get(key);
            set => Set(key, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(TKey key, TValue value)
        {
            lock (_lockObj)
            {
                if (_persistentDictionaryPro.ContainsKey(key))
                    throw new ArgumentException("Key already exists");


                var data = _compressionHandler.Serialize(value);

                FileStream.Seek(0, SeekOrigin.End);
                var kvpPair = new KeyValuePair<long, int>(FileStream.Position, data.Length);
                _persistentDictionaryPro.DictionarySerializedLookup[key] = kvpPair;


                byte[] KeySerialized = _compressionHandler.SerializeNotCompressed(key);
                byte[] len = BitConverter.GetBytes((UInt16)KeySerialized.Length);
                byte[] keyPosSerialized = new byte[12];
                Buffer.BlockCopy(BitConverter.GetBytes(kvpPair.Key), 0, keyPosSerialized, 0, 8);
                Buffer.BlockCopy(BitConverter.GetBytes(kvpPair.Value), 0, keyPosSerialized, 8, 4);

                // Calculate total length
                int totalLength = len.Length + KeySerialized.Length + keyPosSerialized.Length;
                byte[] allData = new byte[totalLength];

                // Copy all data into single array
                Buffer.BlockCopy(len, 0, allData, 0, len.Length);
                Buffer.BlockCopy(KeySerialized, 0, allData, len.Length, KeySerialized.Length);
                Buffer.BlockCopy(keyPosSerialized, 0, allData, len.Length + KeySerialized.Length, keyPosSerialized.Length);

                // Write to file
                FileStream_Keys.Write(allData);
                FileStream_Keys.Flush();

                FileStream.Write(data, 0, data.Length);
                FileStream.Flush();
            }
            _updateTimer.Start();
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set(TKey key, TValue value)
        {
            var data = _compressionHandler.Serialize(value);
            lock (_lockObj)
            {
                FileStream.Seek(0, SeekOrigin.End);
                var kvpLookup = new KeyValuePair<long, int>(FileStream.Position, data.Length);
                _persistentDictionaryPro.DictionarySerializedLookup[key] = kvpLookup;

                byte[] KeySerialized = _compressionHandler.SerializeNotCompressed(key);
                byte[] len = BitConverter.GetBytes((UInt16)KeySerialized.Length);
                byte[] keyPosSerialized = new byte[12];
                Buffer.BlockCopy(BitConverter.GetBytes(kvpLookup.Key), 0, keyPosSerialized, 0, 8);
                Buffer.BlockCopy(BitConverter.GetBytes(kvpLookup.Value), 0, keyPosSerialized, 8, 4);

                // Calculate total length
                int totalLength = len.Length + KeySerialized.Length + keyPosSerialized.Length;
                byte[] allData = new byte[totalLength];

                // Copy all data into single array
                Buffer.BlockCopy(len, 0, allData, 0, len.Length);
                Buffer.BlockCopy(KeySerialized, 0, allData, len.Length, KeySerialized.Length);
                Buffer.BlockCopy(keyPosSerialized, 0, allData, len.Length + KeySerialized.Length, keyPosSerialized.Length);

                // Write to file
                FileStream_Keys.Write(allData);
                FileStream_Keys.Flush();

                FileStream.Write(data, 0, data.Length);
                FileStream.Flush();
            }

            _updateTimer.Start();
        }

        /// <summary>
        /// `AddOrUpdate(Key key, Value value)`: If the key exists in the dictionary, update the value. If it doesn't exist, add the new key-value pair
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddOrUpdate(TKey key, TValue value)
        {
            lock (_lockObj)
                if (_persistentDictionaryPro.DictionarySerializedLookup.ContainsKey(key))
                    _persistentDictionaryPro.DictionarySerializedLookup.Remove(key);

            var data = _compressionHandler.Serialize(value);
            lock (_lockObj)
            {
                FileStream.Seek(0, SeekOrigin.End);
                var kvpLookup = new KeyValuePair<long, int>(FileStream.Position, data.Length);
                _persistentDictionaryPro.DictionarySerializedLookup[key] = kvpLookup;

                byte[] KeySerialized = _compressionHandler.SerializeNotCompressed(key);
                byte[] len = BitConverter.GetBytes((UInt16)KeySerialized.Length);
                byte[] keyPosSerialized = new byte[12];
                Buffer.BlockCopy(BitConverter.GetBytes(kvpLookup.Key), 0, keyPosSerialized, 0, 8);
                Buffer.BlockCopy(BitConverter.GetBytes(kvpLookup.Value), 0, keyPosSerialized, 8, 4);

                // Calculate total length
                int totalLength = len.Length + KeySerialized.Length + keyPosSerialized.Length;
                byte[] allData = new byte[totalLength];

                // Copy all data into single array
                Buffer.BlockCopy(len, 0, allData, 0, len.Length);
                Buffer.BlockCopy(KeySerialized, 0, allData, len.Length, KeySerialized.Length);
                Buffer.BlockCopy(keyPosSerialized, 0, allData, len.Length + KeySerialized.Length, keyPosSerialized.Length);

                // Write to file
                FileStream_Keys.Write(allData);
                FileStream_Keys.Flush();

                FileStream.Write(data, 0, data.Length);
                FileStream.Flush();
            }

            _updateTimer.Start();
        }

        /// <summary>
        /// UpdateValue(Key key, Func<Value,Value> updater): Update the value for a key using a function that transforms the old value into a new one.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="updater"></param>
        /// <exception cref="KeyNotFoundException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UpdateValue(TKey key, Func<TValue, TValue> updater)
        {
            KeyValuePair<long, int> lookupCoordinates;
            lock (_lockObj)
            {
                if (_persistentDictionaryPro.DictionarySerializedLookup.TryGetValue(key, out lookupCoordinates) == false)
                    throw new KeyNotFoundException($"The key {key} was not found in the dictionary.");

                var data = new byte[lookupCoordinates.Value];

                FileStream.Seek(lookupCoordinates.Key, SeekOrigin.Begin);
                FileStream.Read(data, 0, lookupCoordinates.Value);

                var valueDeserialized = updater(_compressionHandler.Deserialize<TValue>(data));
                var newData = _compressionHandler.Serialize(valueDeserialized);

                if (newData.Length <= data.Length)
                {
                  
                    var kvpLookup = new KeyValuePair<long, int>(lookupCoordinates.Key, newData.Length);
                    _persistentDictionaryPro.DictionarySerializedLookup[key] = kvpLookup;

                    byte[] KeySerialized = _compressionHandler.SerializeNotCompressed(key);
                    byte[] len = BitConverter.GetBytes((UInt16)KeySerialized.Length);
                    byte[] keyPosSerialized = new byte[12];
                    Buffer.BlockCopy(BitConverter.GetBytes(kvpLookup.Key), 0, keyPosSerialized, 0, 8);
                    Buffer.BlockCopy(BitConverter.GetBytes(kvpLookup.Value), 0, keyPosSerialized, 8, 4);

                    // Calculate total length
                    int totalLength = len.Length + KeySerialized.Length + keyPosSerialized.Length;
                    byte[] allData = new byte[totalLength];

                    // Copy all data into single array
                    Buffer.BlockCopy(len, 0, allData, 0, len.Length);
                    Buffer.BlockCopy(KeySerialized, 0, allData, len.Length, KeySerialized.Length);
                    Buffer.BlockCopy(keyPosSerialized, 0, allData, len.Length + KeySerialized.Length, keyPosSerialized.Length);

                    // Write to file
                    FileStream_Keys.Write(allData);
                    FileStream_Keys.Flush();

                    FileStream.Seek(lookupCoordinates.Key, SeekOrigin.Begin);
                    FileStream.Write(newData, 0, newData.Length);
                    FileStream.Flush();
                }
                else
                {
                  
                    var kvpLookup = new KeyValuePair<long, int>(FileStream.Position, newData.Length);
                    _persistentDictionaryPro.DictionarySerializedLookup[key] = kvpLookup;

                    byte[] KeySerialized = _compressionHandler.SerializeNotCompressed(key);
                    byte[] len = BitConverter.GetBytes((UInt16)KeySerialized.Length);
                    byte[] keyPosSerialized = new byte[12];
                    Buffer.BlockCopy(BitConverter.GetBytes(kvpLookup.Key), 0, keyPosSerialized, 0, 8);
                    Buffer.BlockCopy(BitConverter.GetBytes(kvpLookup.Value), 0, keyPosSerialized, 8, 4);

                    // Calculate total length
                    int totalLength = len.Length + KeySerialized.Length + keyPosSerialized.Length;
                    byte[] allData = new byte[totalLength];

                    // Copy all data into single array
                    Buffer.BlockCopy(len, 0, allData, 0, len.Length);
                    Buffer.BlockCopy(KeySerialized, 0, allData, len.Length, KeySerialized.Length);
                    Buffer.BlockCopy(keyPosSerialized, 0, allData, len.Length + KeySerialized.Length, keyPosSerialized.Length);

                    // Write to file
                    FileStream_Keys.Write(allData);
                    FileStream_Keys.Flush();

                    FileStream.Seek(0, SeekOrigin.End);
                    FileStream.Write(newData, 0, newData.Length);
                    FileStream.Flush();

                    _updateTimer.Start();
                }
            }
        }

        /// <summary>
        /// `TryGetValueOrNull(Key key)`: A variation on `TryGetValue` that simply returns null if the key is not in the dictionary, rather than requiring a separate 'out' variable.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TValue TryGetValueOrNull(TKey key)
        {
            return SerializedLookup_TryGetValue(key, out var deserializedData) == false ? default : deserializedData;
        }

        /// <summary>
        /// `RemoveAll(Func<Key, Value, bool> predicate)`: Removes all the key-value pairs that satisfy the provided condition.
        /// </summary>
        /// <param name="predicate"></param>
        public void RemoveAll(Func<TKey, TValue, bool> predicate)
        {
            var keysToRemove = new List<TKey>();

            lock (_lockObj)
            {
                foreach (var kvp in _persistentDictionaryPro.DictionarySerializedLookup.Keys)
                    if (predicate(kvp, Get(kvp)))
                        keysToRemove.Add(kvp);

                foreach (var key in keysToRemove)
                    _persistentDictionaryPro.DictionarySerializedLookup.Remove(key);

                _updateTimer.Start();
            }
        }

        /// <summary>
        /// `GetOrCreate(Key key, Func<Value> creator)`: Method to get a value if it exists; otherwise, use the provided function to create a new value and insert it into the dictionary.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="creator"></param>
        /// <returns></returns>
        public TValue GetOrCreate(TKey key, Func<TValue> creator)
        {
            if (ContainsKey(key))
                return Get(key);

            var newValue = creator();
            Add(key, newValue);

            _updateTimer.Start();

            return newValue;
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            lock (_lockObj)
                foreach (var key in _persistentDictionaryPro.DictionarySerializedLookup.Keys)
                    yield return new KeyValuePair<TKey, TValue>(key, this[key]);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            var index = 0;
            lock (_lockObj)
            {
                var entries = new KeyValuePair<TKey, TValue>[_persistentDictionaryPro.DictionarySerializedLookup.Count];

                foreach (var keyValuePair in _persistentDictionaryPro.DictionarySerializedLookup.Keys)
                    entries[index++] = new KeyValuePair<TKey, TValue>(keyValuePair, Get(keyValuePair));

                info.AddValue("entries", entries);
            }
        }

        public Type GetKeyType()
        {
            return typeof(TKey);
        }

        public Type GetValueType()
        {
            return typeof(TValue);
        }

        public void Switch(TKey key1, TKey key2)
        {
            if (TryGetValue(key1, out var value1) == false || TryGetValue(key2, out var value2) == false)
                throw new ArgumentException("One or both keys are not present in the dictionary.");

            Remove(key1);
            Remove(key2);

            Add(key1, value2);
            Add(key2, value1);
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            lock (_lockObj)
                return _persistentDictionaryPro.DictionarySerializedLookup.ContainsKey(item.Key) && Get(item.Key).Equals(item.Value);
        }

        ///// <summary>
        ///// ClearWithValue(Value val) : Clears the dictionary but sets all keys to be associated with a specified default value.
        ///// </summary>
        ///// <param name = "val" ></ param >
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ClearWithValue(TValue val)
        {
            var defaultValue = _compressionHandler.Serialize(val);
            lock (_lockObj)
            {
                FileStream.SetLength(0);
                FileStream_Keys.SetLength(0);
                FileStream_Keys.Flush();
                FileStream.Flush();
                foreach (var key in _persistentDictionaryPro.DictionarySerializedLookup.Keys)
                {
                    var location = new KeyValuePair<long, int>(FileStream.Position, defaultValue.Length);
                    FileStream.Write(defaultValue, 0, defaultValue.Length);

                    _persistentDictionaryPro.DictionarySerializedLookup[key] = location;

                    byte[] KeySerialized = _compressionHandler.SerializeNotCompressed(key);
                    byte[] len = BitConverter.GetBytes((UInt16)KeySerialized.Length);

                    byte[] keyPosSerialized = new byte[12];
                    Buffer.BlockCopy(BitConverter.GetBytes(location.Key), 0, keyPosSerialized, 0, 8);
                    Buffer.BlockCopy(BitConverter.GetBytes(location.Value), 0, keyPosSerialized, 8, 4);

                    // Calculate total length
                    int totalLength = len.Length + KeySerialized.Length + keyPosSerialized.Length;
                    byte[] allData = new byte[totalLength];

                    // Copy all data into single array
                    Buffer.BlockCopy(len, 0, allData, 0, len.Length);
                    Buffer.BlockCopy(KeySerialized, 0, allData, len.Length, KeySerialized.Length);
                    Buffer.BlockCopy(keyPosSerialized, 0, allData, len.Length + KeySerialized.Length, keyPosSerialized.Length);

                    // Write to file
                    FileStream_Keys.Write(allData);

                }
                FileStream_Keys.Flush();
                FileStream.Flush();
            }
        }

        /// <summary>
        /// RenameKey(Key oldKey, Key newKey): Changes the name of a key while keeping its associated value. 
        /// </summary>
        /// <param name="oldKey"></param>
        /// <param name="newKey"></param>
        /// <exception cref="KeyNotFoundException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public void RenameKey(TKey oldKey, TKey newKey)
        {
            if (ContainsKey(oldKey) == false)
                throw new KeyNotFoundException($"The key {oldKey} was not found in the dictionary.");

            var value = Get(oldKey);

            Remove(oldKey);
            Set(newKey, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsKey(TKey key)
        {
            lock (_lockObj)
                return _persistentDictionaryPro.DictionarySerializedLookup.ContainsKey(key);
        }

        public bool HasKey(TKey key) => ContainsKey(key);

        public bool HasValue(TValue key) => ContainsValue(key);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsValue(TValue value)
        {
            lock (_lockObj)
                foreach (var key in _persistentDictionaryPro.DictionarySerializedLookup.Values)
                {
                    var val = SeekReadAndDeserialize(key.Key, key.Value);
                    if (val == null && value == null || val.Equals(value))
                        return true;
                }

            return false;
        }

        /// <summary>
        ///  `Pop(Key key)`: Removes the item with the specified key and returns its value.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TValue Pop(TKey key)
        {
            var value = Get(key);
            Remove(key);
            return value;
        }

        public void Push(TKey key, TValue value)
        {
            Add(key, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            lock (_lockObj)
            {
                _persistentDictionaryPro.DictionarySerializedLookup.Clear();
                FileStream.SetLength(0);
                FileStream_Keys.SetLength(0);
                FileStream_Keys.Flush();
                FileStream.Flush();

                //FileStream.Seek(0, SeekOrigin.Begin);
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ClearCacheAndLookup()
        {
            _persistentDictionaryPro.DictionarySerializedLookup.Clear();
        }

        /// <summary>
        /// `GetRandom()`: Returns a random key-value pair from the dictionary.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public KeyValuePair<TKey, TValue> GetRandom()
        {
            var allKeys = Keys.Count();

            if (allKeys == 0)
                throw new InvalidOperationException("Dictionary is empty");

            var rng = new Random();

            var randomKey = Keys.ElementAt(rng.Next(allKeys));

            return new KeyValuePair<TKey, TValue>(randomKey, Get(randomKey));
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Remove(TKey key)
        {
            lock (_lockObj)
            {
                //var tempKey = _persistentDictionaryPro.DictionarySerializedLookup[key];


                var removedFromLookup = _persistentDictionaryPro.DictionarySerializedLookup.Remove(key);




                if (removedFromLookup == false)
                    return false;
                else
                {
                    byte[] KeySerialized = _compressionHandler.SerializeNotCompressed(key);
                    byte[] len = BitConverter.GetBytes((UInt16)KeySerialized.Length);

                    byte[] keyPosSerialized = new byte[12];
                    var tempKey = new KeyValuePair<long, int>(-1,-1);

                    Buffer.BlockCopy(BitConverter.GetBytes(tempKey.Key), 0, keyPosSerialized, 0, 8);
                    Buffer.BlockCopy(BitConverter.GetBytes(tempKey.Value), 0, keyPosSerialized, 8, 4);

                    // Calculate total length
                    int totalLength = len.Length + KeySerialized.Length + keyPosSerialized.Length;
                    byte[] allData = new byte[totalLength];

                    // Copy all data into single array
                    Buffer.BlockCopy(len, 0, allData, 0, len.Length);
                    Buffer.BlockCopy(KeySerialized, 0, allData, len.Length, KeySerialized.Length);
                    Buffer.BlockCopy(keyPosSerialized, 0, allData, len.Length + KeySerialized.Length, keyPosSerialized.Length);

                    // Write to file
                    FileStream_Keys.Write(allData);
                    FileStream_Keys.Flush();
                    FileStream.Flush();
                }
            }

            _updateTimer.Start();

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetValue(TKey key, out TValue value)
        {
            if (SerializedLookup_TryGetValue(key, out value))
                return true;

            value = default;
            return false;
        }


        public bool TryGetValue(TKey key, out TValue value, Type custType)
        {
            if (SerializedLookup_TryGetValueCustomType(key, out value, custType))
                return true;

            value = default;
            return false;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TValue Get(TKey key)
        {
            if (SerializedLookup_TryGetValue(key, out var value) == false)
                throw new KeyNotFoundException($"The key {key} was not found in the dictionary.");

            return value;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TValue GetReturnDefaultIfNotFound(TKey key)
        {
            lock (_lockObj)
            {
                if (SerializedLookup_TryGetValue(key, out var value) == false)
                    return default;

                return value;
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TValue[] GetBulk(TKey[] keys)
        {
            var returnValues = new TValue[keys.Length];
            lock (_lockObj)
            {
                for (var index = 0; index < keys.Length; index++)
                {
                    if (SerializedLookup_TryGetValue(keys[index], out returnValues[index]) == false)
                        throw new KeyNotFoundException($"The key {keys[index]} was not found in the dictionary.");
                }
            }

            return returnValues;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public List<TValue> GetBulk(List<TKey> keys) => GetBulk(keys.ToArray()).ToList();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public KeyValuePair<TKey, TValue>[] GetBulk(Func<TKey, TValue, bool> predicate)
        {
            var returnValuesList = new List<KeyValuePair<TKey, TValue>>();
            lock (_lockObj)
            {
                foreach (var key in Keys)
                {
                    if (SerializedLookup_TryGetValue(key, out var value) == false)
                        throw new KeyNotFoundException($"The key {key} was not found in the dictionary.");

                    if (predicate(key, value))
                        returnValuesList.Add(new KeyValuePair<TKey, TValue>(key, value));
                }
            }

            return returnValuesList.ToArray();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ForEach(Action<TKey, TValue> action)
        {
            lock (_lockObj)
            {
                foreach (var item in _persistentDictionaryPro.DictionarySerializedLookup)
                    action(item.Key, SeekReadAndDeserialize(item.Value.Key, item.Value.Value));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool SerializedLookup_TryGetValue(TKey key, out TValue value)
        {
            lock (_lockObj)
            {
                if (_persistentDictionaryPro.DictionarySerializedLookup.TryGetValue(key, out var lookupCoordinates))
                {
                    var data = new byte[lookupCoordinates.Value];

                    FileStream.Seek(lookupCoordinates.Key, SeekOrigin.Begin);
                    FileStream.Read(data, 0, lookupCoordinates.Value);

                    value = _compressionHandler.Deserialize<TValue>(data);
                    return true;
                }
            }

            value = default;
            return false;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool SerializedLookup_TryGetValueCustomType(TKey key, out TValue value, Type custType)
        {
            lock (_lockObj)
            {
                if (_persistentDictionaryPro.DictionarySerializedLookup.TryGetValue(key, out var lookupCoordinates))
                {
                    var data = new byte[lookupCoordinates.Value];

                    FileStream.Seek(lookupCoordinates.Key, SeekOrigin.Begin);
                    FileStream.Read(data, 0, lookupCoordinates.Value);

                    value = _compressionHandler.Deserialize(custType, data);
                    return true;
                }
            }

            value = default;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private TValue SeekReadAndDeserialize(long index, int length)
        {
            var data = new byte[length];
            FileStream.Seek(index, SeekOrigin.Begin);
            FileStream.Read(data, 0, length);

            return _compressionHandler.Deserialize<TValue>(data);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private byte[] SeekRead(long index, int length)
        {
            var data = new byte[length];
            FileStream.Seek(index, SeekOrigin.Begin);
            FileStream.Read(data, 0, length);

            return data;
        }

    }
}
