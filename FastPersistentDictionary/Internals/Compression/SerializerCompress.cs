using GroBuf;

namespace FastPersistentDictionary.Internals.Compression
{
    public class SerializerCompress<TKey, TValue> : ICompressionHandler<TKey, TValue>
    {
        public readonly Serializer serializer;
        public byte[] Serialize(TValue data)
        {
            byte[] byteData = serializer.Serialize(data);

            return K4os.Compression.LZ4.LZ4Pickler.Pickle(byteData);
        }

        public byte[] Serialize<TValue>(TValue obj)
        {
            byte[] byteData = serializer.Serialize(obj);

            return K4os.Compression.LZ4.LZ4Pickler.Pickle(byteData);
        }

        public TValue Deserialize(byte[] compressedData)
        {
            compressedData = K4os.Compression.LZ4.LZ4Pickler.Unpickle(compressedData);

            return serializer.Deserialize<TValue>(compressedData);
        }

        public TValue Deserialize<TValue>(byte[] compressedData)
        {
            compressedData = K4os.Compression.LZ4.LZ4Pickler.Unpickle(compressedData);

            return serializer.Deserialize<TValue>(compressedData);
        }

        public TValue Deserialize(Type type, byte[] data)
        {

            return (TValue)serializer.Deserialize(type,data);
        }

        public byte[] SerializeNotCompressed<T>(T obj)
        {
            return serializer.Serialize(obj);
        }

        public TValue DeserializeNotCompressed(byte[] compressedData)
        {
            return serializer.Deserialize<TValue>(compressedData);
        }


        public TKey DeserializeKey(byte[] compressedData)
        {
            return serializer.Deserialize<TKey>(compressedData);
        }

        public byte[] SerializeCompressed<T>(T obj)
        {
            byte[] data = serializer.Serialize(obj);
            return K4os.Compression.LZ4.LZ4Pickler.Pickle(data);
        }

        public TValue DeserializeCompressed(byte[] compressedData)
        {
            compressedData = K4os.Compression.LZ4.LZ4Pickler.Unpickle(compressedData);
            return serializer.Deserialize<TValue>(compressedData);
        }

        public TValue DeserializeCompressed<TValue>(byte[] compressedData)
        {
            compressedData = K4os.Compression.LZ4.LZ4Pickler.Unpickle(compressedData);
            return serializer.Deserialize<TValue>(compressedData);
        }

        public TValue DeserializeNotCompressed<TValue>(byte[] compressedData)
        {
            return serializer.Deserialize<TValue>(compressedData);
        }

        public SerializerCompress(Serializer serializer)
        {
            this.serializer = serializer;
        }
    }
}
