using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FastPersistentDictionary.Internals.Accessor
{
    public interface IDictionaryAccessor<TKey, TValue>
    {
        IEqualityComparer<TKey> Comparer { get; set; }
        int Count { get; }
        IEnumerable<TKey> Keys { get; }
        IEnumerable<TValue> Values { get; }
        TValue this[TKey key] { get; set; }

        void Add(TKey key, TValue value);
        void Set(TKey key, TValue value);
        void AddOrUpdate(TKey key, TValue value);
        void UpdateValue(TKey key, Func<TValue, TValue> updater);
        TValue TryGetValueOrNull(TKey key);
        void RemoveAll(Func<TKey, TValue, bool> predicate);
        TValue GetOrCreate(TKey key, Func<TValue> creator);
        IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator();
        void GetObjectData(SerializationInfo info, StreamingContext context);
        Type GetKeyType();
        Type GetValueType();
        void Switch(TKey key1, TKey key2);
        bool Contains(KeyValuePair<TKey, TValue> item);
        void ClearWithValue(TValue val);
        void RenameKey(TKey oldKey, TKey newKey);
        bool ContainsKey(TKey key);
        bool HasKey(TKey key);
        bool HasValue(TValue key);
        bool ContainsValue(TValue value);
        TValue Pop(TKey key);
        void Push(TKey key, TValue value);
        void Clear();
        KeyValuePair<TKey, TValue> GetRandom();
        bool Remove(TKey key);
        bool TryGetValue(TKey key, out TValue value);
        bool TryGetValue(TKey key, out TValue value, Type custType);
        TValue Get(TKey key);
        TValue GetReturnDefaultIfNotFound(TKey key);
        TValue[] GetBulk(TKey[] keys);
        List<TValue> GetBulk(List<TKey> keys);
        KeyValuePair<TKey, TValue>[] GetBulk(Func<TKey, TValue, bool> predicate);
        void ForEach(Action<TKey, TValue> action);

        //public IEqualityComparer<TKey> Comparer;


    }
}
