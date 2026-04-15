using System.Diagnostics.CodeAnalysis;
using System.Text;
using GroBuf;
using FastPersistentDictionary.Internals.Compression;
using System.IO;

namespace FastPersistentDictionary.Internals
{
    [SuppressMessage("ReSharper", "MustUseReturnValue")]
    public sealed class DictionaryIo<TKey, TValue>
    {
        private ICompressionHandler<TKey, TValue> _compressionHandler;
        private readonly object _lockObj;
        private readonly Serializer _serializer;
        private readonly DictionaryQuery<TKey, TValue> _dictionaryQuery;
        private DictionaryStructs.DictionarySaveHeader _currentHeader;
        internal FileStream FileStream;
        internal bool UseCompression;

        public DictionaryIo(
            FastPersistentDictionary<TKey, TValue> dict,
            ICompressionHandler<TKey, TValue> compressionHandler,
            DictionaryQuery<TKey, TValue> dictionaryQuery,
            object lockObj,
            FileStream fileStream,
            Serializer serializer,
            bool useCompression)
        {
            FileStream = fileStream;
            _fastPersistentDictionary = dict;
            _compressionHandler = compressionHandler;
            _serializer = serializer;
            _dictionaryQuery = dictionaryQuery;
            UseCompression = useCompression;
            _currentHeader = new DictionaryStructs.DictionarySaveHeader()
            {
                UseCompression = UseCompression, //LZ4 Fastest compression option
                CreationDate = DateTime.Now,
                LastModifiedDate = DateTime.Now,
                Version = _fastPersistentDictionary.FastPersistentDictionaryVersion,
                DictionaryCount = 0,
                KeyType = typeof(TKey).ToString(),
                KeyValue = typeof(TValue).ToString(),
                ByteLengthOfLookupTable = 0,
                PercentageChangeBeforeCompact = _fastPersistentDictionary.PercentageChangeBeforeCompact
            };

            _lockObj = lockObj;
        }

        private readonly FastPersistentDictionary<TKey, TValue> _fastPersistentDictionary;

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();

            lock (_lockObj)
            {
                foreach (var keyValPair in _fastPersistentDictionary.DictionaryAccessor)
                    stringBuilder.Append($"{keyValPair.Key + ": " + keyValPair.Value}\n");
            }

            return stringBuilder.ToString();
        }

        public long GetDatabaseSizeBytes()
        {
            return FileStream.Length;
        }

        public DictionaryStructs.DictionarySaveHeader SaveDictionary(string savePath, string name = "", string comment = "")
        {
            lock (_lockObj)
            {
                _dictionaryQuery.UpdateDatabaseTick(true);

                var header = new DictionaryStructs.DictionarySaveHeader()
                {
                    UseCompression = UseCompression,
                    CreationDate = _currentHeader.CreationDate,
                    LastModifiedDate = DateTime.Now,
                    Version = _fastPersistentDictionary.FastPersistentDictionaryVersion,
                    DictionaryCount = _fastPersistentDictionary.DictionarySerializedLookup.Count,
                    KeyType = typeof(TKey).ToString(),
                    KeyValue = typeof(TValue).ToString(),
                    ByteLengthOfLookupTable = 0,
                    Comment = comment,
                    Name = name,
                    PercentageChangeBeforeCompact = _fastPersistentDictionary.PercentageChangeBeforeCompact
                };

                return SaveDictionary(savePath, header);
            }
        }

        public DictionaryStructs.DictionarySaveHeader SaveDictionary(string savePath, DictionaryStructs.DictionarySaveHeader header = null)
        {
            var path = Path.GetDirectoryName(savePath);

            if (Directory.Exists(path) == false)
                Directory.CreateDirectory(path);

            var normalizedSavePath = Path.GetFullPath(savePath);
            var isSameFile = normalizedSavePath == FileStream.Name;

            lock (_lockObj)
            {
                var serializedLookup = _compressionHandler.SerializeCompressed(_fastPersistentDictionary.DictionarySerializedLookup);
                if (header == null)
                {
                    _dictionaryQuery.UpdateDatabaseTick(true);
                    header = new DictionaryStructs.DictionarySaveHeader()
                    {
                        UseCompression = UseCompression,
                        CreationDate = _currentHeader.CreationDate,
                        LastModifiedDate = DateTime.Now,
                        Version = _fastPersistentDictionary.FastPersistentDictionaryVersion,
                        DictionaryCount = _fastPersistentDictionary.DictionarySerializedLookup.Count,
                        KeyType = typeof(TKey).ToString(),
                        KeyValue = typeof(TValue).ToString(),
                        ByteLengthOfLookupTable = serializedLookup.Length,
                        Comment = "",
                        Name = _fastPersistentDictionary.GetType().Name,
                        PercentageChangeBeforeCompact = _fastPersistentDictionary.PercentageChangeBeforeCompact
                    };
                }

                //ensure in case not null from other overload
                header.ByteLengthOfLookupTable = serializedLookup.Length;

                var headerBytes = _compressionHandler.SerializeNotCompressed(header);

                // When saving to the same file, write to a temp file first to avoid
                // truncating the active FileStream (FileMode.Create zeroes the file).
                var writePath = isSameFile ? Path.GetTempFileName() : normalizedSavePath;

                using (var origFs = new FileStream(writePath, FileMode.Create, FileAccess.Write))
                {
                    //1. write out length of compressed database data as long
                    //2. write database data
                    //3. write out length of lookup table as int
                    //4. write out lookup table
                    //5. write out length of header
                    //6. write out header
                    var lookupLength = BitConverter.GetBytes(serializedLookup.Length);
                    var headerLength = BitConverter.GetBytes(headerBytes.Length);

                    FileStream.Seek(0, SeekOrigin.Begin);
                    var dbLength = BitConverter.GetBytes(FileStream.Length);

                    origFs.Write(dbLength, 0, dbLength.Length); //1
                    FileStream.CopyTo(origFs); //2


                    origFs.Write(lookupLength, 0, lookupLength.Length); //3
                    origFs.Write(serializedLookup, 0, serializedLookup.Length); //4
                    origFs.Write(headerLength, 0, headerLength.Length); //5
                    origFs.Write(headerBytes, 0, headerBytes.Length); //6
                }

                if (isSameFile)
                {
                    FileStream.SetLength(0);
                    FileStream.Seek(0, SeekOrigin.Begin);
                    using (var tempFs = new FileStream(writePath, FileMode.Open, FileAccess.Read))
                    {
                        tempFs.CopyTo(FileStream);
                    }
                    FileStream.Flush();
                    File.Delete(writePath);
                }

                return header;
            }
        }


        public DictionaryStructs.DictionarySaveHeader LoadDictionary(string loadPath)
        {
            if (File.Exists(loadPath) == false)
                throw new InvalidDataException("File Path does not exist.");
            //1. read in length of compressed database data as long
            //2. skip database data
            //3. read in length of lookup table as int
            //4. read in lookup table
            //5. read in length of header as int
            //6. read in header

            var normalizedLoadPath = Path.GetFullPath(loadPath);

            if (normalizedLoadPath == FileStream.Name)
            {
                //FileStream.Close();
                //FileStream = new FileStream(_fastPersistentDictionary.FileLocation, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite | FileShare.Delete, _fastPersistentDictionary.fileStreamBufferSize, _fastPersistentDictionary.fileOptions); //| FileOptions.DeleteOnClose);
                var tempPath = Path.GetTempFileName();
                using (var newFileStream = new FileStream(tempPath, FileMode.Open, FileAccess.ReadWrite))
                {
                    FileStream.Seek(0, SeekOrigin.Begin);
                    var lengthCompressedDataBaseBytes = new byte[8]; //long
                    var lengthLookupBytes = new byte[4]; //int //CONSIDER long, doubt file size would ever get that size but only 4 more bytes
                    var lengthHeaderBytes = new byte[4]; //int

                    FileStream.ReadExactly(lengthCompressedDataBaseBytes, 0, 8);
                    var lengthCompressedDataBase = BitConverter.ToInt64(lengthCompressedDataBaseBytes, 0);
                    FileStream.Position += lengthCompressedDataBase;

                    FileStream.ReadExactly(lengthLookupBytes, 0, 4);
                    var lengthLookup = BitConverter.ToInt32(lengthLookupBytes, 0);
                    var lookupBytes = new byte[lengthLookup];
                    FileStream.ReadExactly(lookupBytes, 0, lengthLookup);

                    FileStream.ReadExactly(lengthHeaderBytes, 0, 4);
                    var lengthHeader = BitConverter.ToInt32(lengthHeaderBytes, 0);
                    var lookupHeader = new byte[lengthHeader];
                    FileStream.ReadExactly(lookupHeader, 0, lengthHeader);

                    var header = _compressionHandler.DeserializeNotCompressed<DictionaryStructs.DictionarySaveHeader>(lookupHeader);
                    _currentHeader = header;

                    UseCompression = header.UseCompression;

                    if (UseCompression)
                        _compressionHandler = new SerializerCompress<TKey, TValue>(_serializer);
                    else
                        _compressionHandler = new SerializerUnCompressed<TKey, TValue>(_serializer);

                    var loadedLookup = _compressionHandler.DeserializeCompressed<Dictionary<TKey, KeyValuePair<long, int>>>(lookupBytes);
                    lock (_lockObj)
                    {
                        _fastPersistentDictionary.PercentageChangeBeforeCompact = header.PercentageChangeBeforeCompact;

                        _fastPersistentDictionary.DictionarySerializedLookup = loadedLookup;

                        FileStream.Seek(8, SeekOrigin.Begin);
                        var buffer = new byte[8192];

                        newFileStream.SetLength(0);

                        // Only copy the actual database data, not the trailing lookup/header
                        long remaining = lengthCompressedDataBase;
                        while (remaining > 0)
                        {
                            int toRead = (int)Math.Min(buffer.Length, remaining);
                            int count = FileStream.Read(buffer, 0, toRead);
                            if (count == 0) break;
                            newFileStream.Write(buffer, 0, count);
                            remaining -= count;
                        }

                        FileStream.SetLength(0);
                        FileStream.Seek(0, SeekOrigin.Begin);
                        newFileStream.Seek(0, SeekOrigin.Begin);
                        newFileStream.CopyTo(FileStream);

                        _dictionaryQuery.LastFileSizeCompact = FileStream.Length;
                        _dictionaryQuery.NextFileSizeCompact = _dictionaryQuery.LastFileSizeCompact + (_dictionaryQuery.LastFileSizeCompact * (long)(_fastPersistentDictionary.PercentageChangeBeforeCompact / 100));
                    }


                    return header;
                }
            }
            else
            {
                using (var newFileStream = new FileStream(loadPath, FileMode.Open, FileAccess.ReadWrite))
                {
                    newFileStream.Seek(0, SeekOrigin.Begin);
                    var lengthCompressedDataBaseBytes = new byte[8]; //long
                    var lengthLookupBytes = new byte[4]; //int //CONSIDER long, doubt file size would ever get that size but only 4 more bytes
                    var lengthHeaderBytes = new byte[4]; //int

                    newFileStream.ReadExactly(lengthCompressedDataBaseBytes, 0, 8);
                    var lengthCompressedDataBase = BitConverter.ToInt64(lengthCompressedDataBaseBytes, 0);
                    newFileStream.Position += lengthCompressedDataBase;

                    newFileStream.ReadExactly(lengthLookupBytes, 0, 4);
                    var lengthLookup = BitConverter.ToInt32(lengthLookupBytes, 0);
                    var lookupBytes = new byte[lengthLookup];
                    newFileStream.ReadExactly(lookupBytes, 0, lengthLookup);

                    newFileStream.ReadExactly(lengthHeaderBytes, 0, 4);
                    var lengthHeader = BitConverter.ToInt32(lengthHeaderBytes, 0);
                    var lookupHeader = new byte[lengthHeader];
                    newFileStream.ReadExactly(lookupHeader, 0, lengthHeader);

                    var header = _compressionHandler.DeserializeNotCompressed<DictionaryStructs.DictionarySaveHeader>(lookupHeader);
                    _currentHeader = header;

                    UseCompression = header.UseCompression;

                    if (UseCompression)
                        _compressionHandler = new SerializerCompress<TKey, TValue>(_serializer);
                    else
                        _compressionHandler = new SerializerUnCompressed<TKey, TValue>(_serializer);

                    var loadedLookup = _compressionHandler.DeserializeCompressed<Dictionary<TKey, KeyValuePair<long, int>>>(lookupBytes);
                    lock (_lockObj)
                    {
                        _fastPersistentDictionary.PercentageChangeBeforeCompact = header.PercentageChangeBeforeCompact;
                        _fastPersistentDictionary.DictionarySerializedLookup.Clear();
                        foreach (var kvp in loadedLookup)
                            _fastPersistentDictionary.DictionarySerializedLookup[kvp.Key] = kvp.Value;

                        newFileStream.Seek(8, SeekOrigin.Begin);
                        var buffer = new byte[8192];

                        FileStream.SetLength(0);

                        // Only copy the actual database data, not the trailing lookup/header
                        long remaining = lengthCompressedDataBase;
                        while (remaining > 0)
                        {
                            int toRead = (int)Math.Min(buffer.Length, remaining);
                            int count = newFileStream.Read(buffer, 0, toRead);
                            if (count == 0) break;
                            FileStream.Write(buffer, 0, count);
                            remaining -= count;
                        }

                        _dictionaryQuery.LastFileSizeCompact = FileStream.Length;
                        _dictionaryQuery.NextFileSizeCompact = _dictionaryQuery.LastFileSizeCompact + (_dictionaryQuery.LastFileSizeCompact * (long)(_fastPersistentDictionary.PercentageChangeBeforeCompact / 100));
                    }
                    return header;
                }

            }

     
        }

        public byte[] PackBuffer()
        {
            var allData = new Dictionary<TKey, TValue>();
            lock (_lockObj)
            {
                foreach (var kvp in _fastPersistentDictionary.DictionarySerializedLookup)
                {
                    var data = new byte[kvp.Value.Value];
                    FileStream.Seek(kvp.Value.Key, SeekOrigin.Begin);
                    FileStream.ReadExactly(data, 0, kvp.Value.Value);

                    allData[kvp.Key] = _compressionHandler.Deserialize<TValue>(data);
                }
            }

            return _compressionHandler.Serialize(allData);
        }
    }
}