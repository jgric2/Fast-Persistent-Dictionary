using GroBuf;

namespace FastPersistentDictionary.Internals.Compression
{
    public interface ICompressionHandler<TKey, TValue>
    {
        //todo: clean this up, compression added as an afterthought
        public byte[] Serialize(TValue data);
        public byte[] Serialize<T>(T obj);

        public TValue Deserialize(byte[] compressedData);
        public TValue Deserialize<TValue>(byte[] compressedData);

        public TValue Deserialize(Type type, byte[] data);


        //Generic
        public byte[] SerializeNotCompressed<T>(T obj);
        public TValue DeserializeNotCompressed(byte[] compressedData);
        public TValue DeserializeNotCompressed<TValue>(byte[] compressedData);


        public TKey DeserializeKey(byte[] compressedData);
    



        public byte[] SerializeCompressed<T>(T obj);
        public TValue DeserializeCompressed(byte[] compressedData);
        public TValue DeserializeCompressed<TValue>(byte[] compressedData);
    }
}
