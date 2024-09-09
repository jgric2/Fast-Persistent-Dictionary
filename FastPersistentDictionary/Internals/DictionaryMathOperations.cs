using FastPersistentDictionary.Internals.Accessor;
using FastPersistentDictionary.Internals.Compression;

namespace FastPersistentDictionary.Internals
{
    public sealed class DictionaryMathOperations<TKey, TValue> 
    {
        private readonly object _lockObj;
        private readonly ICompressionHandler<TKey, TValue> _compressionHandler;
        internal FileStream FileStream;

        public DictionaryMathOperations(
            FastPersistentDictionary<TKey, TValue> dict,
            DictionaryAccessor<TKey, TValue> dictionaryAccessor,
            DictionaryOperations<TKey, TValue> dictionaryOperations,
            ICompressionHandler<TKey, TValue> compressionHandler,
            object lockObj,
            FileStream fileStream)
        {
            FileStream = fileStream;
            _persistentDictionaryPro = dict;
            _dictionaryAccessor = dictionaryAccessor;
            _dictionaryOperations = dictionaryOperations;
            _compressionHandler = compressionHandler;
            _lockObj = lockObj;
        }

        private readonly FastPersistentDictionary<TKey, TValue> _persistentDictionaryPro;
        private readonly DictionaryAccessor<TKey, TValue> _dictionaryAccessor;
        private readonly DictionaryOperations<TKey, TValue> _dictionaryOperations;

        public TValue Min()
        {
            lock (_lockObj)
            {
                var comparer = Comparer<TValue>.Default;
                var min = default(TValue);

                var isFirstValue = true;

                foreach (var key in _persistentDictionaryPro.DictionarySerializedLookup.Keys)
                {
                    var value = _dictionaryAccessor.Get(key);
                    if (isFirstValue == false && comparer.Compare(value, min) >= 0)
                        continue;

                    min = value;
                    isFirstValue = false;
                }

                if (isFirstValue) 
                    throw new InvalidOperationException("No elements in the dictionary.");

                return min;
            }
        }

        public TValue Sum()
        {
            if (typeof(TValue).IsPrimitive == false && typeof(TValue) != typeof(decimal))
                throw new InvalidOperationException("Sum method can only be used for TValue being a primitive type.");

            dynamic sumVal = 0;
            lock (_lockObj)
            {
                foreach (var kvp in _persistentDictionaryPro.DictionarySerializedLookup.Values)
                {
                    var data = new byte[kvp.Value];
                    FileStream.Seek(kvp.Key, SeekOrigin.Begin);
                    FileStream.Read(data, 0, kvp.Value);

                    sumVal += _compressionHandler.Deserialize<TValue>(data);
                }
            }
            return (TValue)sumVal;
        }

        public bool TryGetMax(out TValue max)
        {
            lock (_lockObj)
            {
                if (_persistentDictionaryPro.DictionarySerializedLookup.Count == 0)
                {
                    max = default;
                    return false;
                }

                var comparer = Comparer<TValue>.Default;
                max = default;
                var firstRound = true;

                foreach (var key in _persistentDictionaryPro.DictionarySerializedLookup.Keys)
                {
                    var value = _dictionaryAccessor.Get(key);

                    if (value == null)
                        continue;
                    if (firstRound == false && comparer.Compare(value, max) <= 0)
                        continue;

                    max = value;
                    firstRound = false;
                }
            }

            return true;
        }

        public bool TryGetMin(out TValue min)
        {
            var comparer = Comparer<TValue>.Default;
            min = default;

            var isFirstValue = true;
            lock (_lockObj)
            {
                foreach (var key in _persistentDictionaryPro.DictionarySerializedLookup.Keys)
                {
                    var value = _dictionaryAccessor.Get(key);
                    if (value == null)
                        continue;

                    if (isFirstValue == false && comparer.Compare(value, min) >= 0)
                        continue;

                    min = value;
                    isFirstValue = false;
                }
            }

            if (min == null)
                return false;

            return isFirstValue == false;
        }

        public TValue Max()
        {
            lock (_lockObj)
            {
                if (_persistentDictionaryPro.Count == 0)
                    throw new InvalidOperationException("No items in dictionary");

                var comparer = Comparer<TValue>.Default;
                var max = default(TValue);

                foreach (var key in _persistentDictionaryPro.DictionarySerializedLookup.Keys)
                {
                    var value = _dictionaryAccessor.Get(key);
                    if (comparer.Compare(value, max) > 0)
                        max = value;
                }

                return max;
            }
        }

        public double Average(Func<KeyValuePair<TKey, TValue>, double> selector)
        {
            lock (_lockObj)
                return _dictionaryOperations.ToList().Average(selector);
        }

        public TKey MaxKey()
        {
            lock (_lockObj)
            {
                var totalKeys = _persistentDictionaryPro.Keys;
                if (totalKeys.Any() == false)
                    throw new InvalidOperationException("The Dictionary is empty.");

                return totalKeys.Max();
            }
        }

        public TKey MinKey()
        {
            lock (_lockObj)
            {
                var totalKeys = _persistentDictionaryPro.Keys;
                if (totalKeys.Any() == false)
                    throw new InvalidOperationException("The Dictionary is empty.");

                return totalKeys.Min();
            }
        }
    }
}