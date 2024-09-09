namespace FastPersistentDictionary.Internals
{
    public static class DictionaryStructs
    {
        public class DictionarySaveHeader
        {
             public bool UseCompression { get; set; }
             public DateTime CreationDate { get; set; }
             public DateTime LastModifiedDate { get; set; }
             public string Version { get; set; }
             public long DictionaryCount { get; set; }
             public string KeyType { get; set; }
             public string KeyValue { get; set; }
             public int ByteLengthOfLookupTable { get; set; }
             public string Comment { get; set; }
             public string Name { get; set; }
             public float PercentageChangeBeforeCompact { get; set; }
        }
    }
}
