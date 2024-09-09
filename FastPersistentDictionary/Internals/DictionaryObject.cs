using System.Collections;
using System.Runtime.CompilerServices;
using FastPersistentDictionary.Internals.Accessor;

namespace FastPersistentDictionary.Internals
{
    public sealed class DictionaryObject<TKey, TValue> : IEnumerable
    {
        private readonly object _lockObj;
        public DictionaryObject(DictionaryAccessor<TKey, TValue> dictionaryAccessor, object lockObj)
        {
            _dictionaryAccessor = dictionaryAccessor;
            _lockObj = lockObj;
        }

        private readonly DictionaryAccessor<TKey, TValue> _dictionaryAccessor;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _dictionaryAccessor.GetEnumerator();
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _dictionaryAccessor.GetEnumerator();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
        {
            lock (_lockObj)
            {
                if (this.GetHashCode() == obj.GetHashCode())
                    return true;

                if (ReferenceEquals(this, obj))
                    return true;

                lock (_lockObj)
                {
                    if (obj is FastPersistentDictionary<TKey, TValue> other == false)
                        return false;

                    if (other.DictionaryAccessor.Count != _dictionaryAccessor.Count)
                        return false;

                    foreach (var pair in this)
                    {
                        if (other.DictionaryAccessor.TryGetValue(pair.Key, out var otherVal) == false)
                            return false;

                        if (Equals(pair.Value, otherVal) == false)
                            return false;
                    }
                }

                return true;
            }
        }
    }
}