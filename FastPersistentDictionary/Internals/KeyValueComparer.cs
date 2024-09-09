namespace FastPersistentDictionary.Internals
{
    public class KeyValuePairComparer<TKey, TValue> : IComparer<KeyValuePair<TKey, TValue>>
    {
        public int Compare(KeyValuePair<TKey, TValue> x, KeyValuePair<TKey, TValue> y)
        {
            int result = Comparer<TKey>.Default.Compare(x.Key, y.Key);
            if (result == 0)
            {
                // If keys are equal, compare by values.
                return Comparer<TValue>.Default.Compare(x.Value, y.Value);
            }
            else
            {
                // If keys are not equal, return result.
                return result;
            }
        }
    }
}
