using System.Collections;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Timers;
using FastPersistentDictionary.Internals.Accessor;
using GroBuf;
using GroBuf.DataMembersExtracters;
using FastPersistentDictionary.Internals;
using FastPersistentDictionary.Internals.Compression;
using Timer = System.Timers.Timer;

[assembly: InternalsVisibleTo("FastPersistentDictionary.Tests")]
namespace FastPersistentDictionary
{
    /// <summary>
    ///     Written by James Grice 2-8-2023 / 8-09-2024
    /// 
    ///     The `FastPersistentDictionary` class is a implementation of a persistent dictionary in C#.
    ///     It is designed to provide a dictionary-like data structure that persists data to disk.
    ///     It is designed to be used like a regular dictionary.
    ///     The class provides a flexible and efficient data structure for storing and querying large amounts of data while
    ///     minimizing disk space usage.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    [Serializable]

    public sealed class FastPersistentDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>,
        ISerializable, IDisposable
    {
        internal Dictionary<TKey, KeyValuePair<long, int>> DictionarySerializedLookup = new Dictionary<TKey, KeyValuePair<long, int>>();

        private readonly DictionaryIo<TKey, TValue> _dictionaryIo;
        private readonly DictionaryMathOperations<TKey, TValue> _dictionaryMathOperations;
        private readonly DictionaryObject<TKey, TValue> _dictionaryObject;
        private readonly DictionaryOperations<TKey, TValue> _dictionaryOperations;
        private readonly DictionaryQuery<TKey, TValue> _dictionaryQuery;


        public bool CanCompact = false;
        public FileStream FileStream;


        private bool _disposed;
        private readonly object _lockObj = new object();
        private readonly Serializer _serializer;
        private readonly ICompressionHandler<TKey, TValue> _compressionHandler;

        private Timer _updateTimer;
        internal readonly DictionaryAccessor<TKey, TValue> DictionaryAccessor;
        internal readonly string FileLocation;
        internal readonly string FastPersistentDictionaryVersion = "1.0.0.5";
        //internal readonly bool CrashRecovery;

        internal readonly bool UseCompression;
        internal float PercentageChangeBeforeCompact;

        //Main create or load a FastPersistentDictionary into memory
        public FastPersistentDictionary(
            string path = "",
            //bool crashRecovery = false,
            int updateRate = 1000,
            float percentageChangeBeforeCompact = 50,
            int fileStreamBufferSize = 8196,
            string importSavedFastPersistentDictionary = "",
            IEqualityComparer<TKey>? equalityComparer = null,
            bool useCompression = false)
        {
           // CrashRecovery = crashRecovery;

            if (path == "")
                path = Path.GetTempFileName();

            FileLocation = path;
            
            _serializer = new Serializer(new AllPropertiesExtractor(), options: GroBufOptions.WriteEmptyObjects);
            Directory.CreateDirectory(Path.GetDirectoryName(FileLocation));

            UseCompression = useCompression;


            if (File.Exists(FileLocation))
                File.Delete(FileLocation);

            FileStream = new FileStream(FileLocation, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite | FileShare.Delete, fileStreamBufferSize, FileOptions.RandomAccess); //| FileOptions.DeleteOnClose);
            _updateTimer = new Timer(updateRate);
            _updateTimer.Elapsed += UpdateTimerElapsedEventHandler;

            if (UseCompression)
                _compressionHandler = new SerializerCompress<TKey, TValue>(_serializer);
            else
                _compressionHandler = new SerializerUnCompressed<TKey, TValue>(_serializer);


            //if (CrashRecovery)
            //    DictionaryAccessor = new DictionaryAccessor_RecoverMode<TKey, TValue>(this, _updateTimer, _compressionHandler, _lockObj, FileStream);
            //else
                DictionaryAccessor = new DictionaryAccessor<TKey, TValue>(this, _updateTimer, _compressionHandler, _lockObj, FileStream);



         

            PercentageChangeBeforeCompact = percentageChangeBeforeCompact;
           
            _dictionaryQuery = new DictionaryQuery<TKey, TValue>(this, DictionaryAccessor, _updateTimer, _compressionHandler, PercentageChangeBeforeCompact, _lockObj, FileStream);
            _dictionaryIo = new DictionaryIo<TKey, TValue>(this, _compressionHandler, _dictionaryQuery, _lockObj, FileStream, _serializer, UseCompression);
            _dictionaryObject = new DictionaryObject<TKey, TValue>(DictionaryAccessor, _lockObj);
            _dictionaryOperations = new DictionaryOperations<TKey, TValue>(this, DictionaryAccessor, _updateTimer, _compressionHandler, _lockObj, FileStream);
            _dictionaryMathOperations = new DictionaryMathOperations<TKey, TValue>(this, DictionaryAccessor, _dictionaryOperations, _compressionHandler, _lockObj, FileStream);

            DictionaryAccessor.Comparer = equalityComparer ?? EqualityComparer<TKey>.Default;

            if (importSavedFastPersistentDictionary != "" && File.Exists(importSavedFastPersistentDictionary))
                _dictionaryIo.LoadDictionary(importSavedFastPersistentDictionary);
        }

        public void Dispose()
        {
            //todo fill this out
            if (_disposed)
                return;

            _updateTimer.Stop();
            _updateTimer = null;
            FileStream.Dispose();
            FileStream.Close();
            FileStream = null;

            try
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                if (File.Exists(FileLocation))
                    File.Delete(FileLocation);
            }
            catch
            {
                // ignored
            }

            _disposed = true;
            GC.SuppressFinalize(this);
        }

        ~FastPersistentDictionary()
        {
            Dispose();
        }

        public TValue this[TKey key]
        {
            get => DictionaryAccessor[key];
            set => DictionaryAccessor[key] = value;
        }
        public IEqualityComparer<TKey> Comparer => DictionaryAccessor.Comparer;
        public int Count => DictionaryAccessor.Count;
        public IEnumerable<TKey> Keys => DictionaryAccessor.Keys;
        public IEnumerable<TValue> Values => DictionaryAccessor.Values;

        public void Add(TKey key, TValue value) => DictionaryAccessor.Add(key, value);

        public void GetObjectData(SerializationInfo info, StreamingContext context) => DictionaryAccessor.GetObjectData(info, context);

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => DictionaryAccessor.GetEnumerator();

        public Type GetKeyType() => DictionaryAccessor.GetKeyType();

        public Type GetValueType() => DictionaryAccessor.GetValueType();

        public bool Contains(KeyValuePair<TKey, TValue> item) => DictionaryAccessor.Contains(item);

        public bool ContainsKey(TKey key) => DictionaryAccessor.ContainsKey(key);

        public bool ContainsValue(TValue value) => DictionaryAccessor.ContainsValue(value);

        public void Clear() => DictionaryAccessor.Clear();

        public TValue GetOrCreate(TKey key, Func<TValue> creator) => DictionaryAccessor.GetOrCreate(key, creator);

        public void UpdateValue(TKey key, Func<TValue, TValue> updater) => DictionaryAccessor.UpdateValue(key, updater);

        public void UpdateDatabaseTick(bool forceCompact = false) => _dictionaryQuery.UpdateDatabaseTick(forceCompact);

        public TValue TryGetValueOrNull(TKey key) => DictionaryAccessor.TryGetValueOrNull(key);

        public bool HasKey(TKey key) => DictionaryAccessor.HasKey(key);

        public bool HasValue(TValue key) => DictionaryAccessor.HasValue(key);

        public void AddOrUpdate(TKey key, TValue value) => DictionaryAccessor.AddOrUpdate(key, value);

        public TValue Pop(TKey key) => DictionaryAccessor.Pop(key);

        public void Push(TKey key, TValue value) => DictionaryAccessor.Push(key, value);

        public KeyValuePair<TKey, TValue> GetRandom() => DictionaryAccessor.GetRandom();

        public void ForEach(Action<TKey, TValue> action) => DictionaryAccessor.ForEach(action);

        public void RemoveAll(Func<TKey, TValue, bool> predicate) => DictionaryAccessor.RemoveAll(predicate);

        public TValue[] GetBulk(TKey[] keys) => DictionaryAccessor.GetBulk(keys);

        public List<TValue> GetBulk(List<TKey> keys) => DictionaryAccessor.GetBulk(keys);

        public void ClearWithValue(TValue val) => DictionaryAccessor.ClearWithValue(val);

        public void RenameKey(TKey oldKey, TKey newKey) => DictionaryAccessor.RenameKey(oldKey, newKey);

        public void Switch(TKey key1, TKey key2) => DictionaryAccessor.Switch(key1, key2);

        public KeyValuePair<TKey, TValue>[] GetBulk(Func<TKey, TValue, bool> predicate) => DictionaryAccessor.GetBulk(predicate);

        public bool Remove(TKey key) => DictionaryAccessor.Remove(key);

        public bool TryGetValue(TKey key, out TValue value) => DictionaryAccessor.TryGetValue(key, out value);

        public bool TryGetValue(TKey key, out TValue value, Type customType) => DictionaryAccessor.TryGetValue(key, out value, customType);

        public TValue Get(TKey key) => DictionaryAccessor.Get(key);

        public long GetDatabaseSizeBytes() => _dictionaryIo.GetDatabaseSizeBytes();

        IEnumerator IEnumerable.GetEnumerator() => _dictionaryObject.GetEnumerator();

        public override bool Equals(object obj) => _dictionaryObject.Equals(obj);

        public override int GetHashCode() => _dictionaryObject.GetHashCode();

        public IEnumerable<TResult> Select<TResult>(Func<KeyValuePair<TKey, TValue>, TResult> selector) =>
            _dictionaryQuery.Select(selector);

        public override string ToString() => _dictionaryIo.ToString();

        public DictionaryStructs.DictionarySaveHeader SaveDictionary(string savePath, string name = "", string comment = "") => _dictionaryIo.SaveDictionary(savePath, name, comment);

        public DictionaryStructs.DictionarySaveHeader SaveDictionary(string savePath, DictionaryStructs.DictionarySaveHeader header) => _dictionaryIo.SaveDictionary(savePath, header);

        public DictionaryStructs.DictionarySaveHeader LoadDictionary(string loadPath) => _dictionaryIo.LoadDictionary(loadPath);

        public byte[] PackBuffer() => _dictionaryIo.PackBuffer();

        public TValue Min() => _dictionaryMathOperations.Min();

        public TValue Sum() => _dictionaryMathOperations.Sum();

        public bool TryGetMax(out TValue max) => _dictionaryMathOperations.TryGetMax(out max);

        public bool TryGetMin(out TValue min) => _dictionaryMathOperations.TryGetMin(out min);

        public TValue Max() => _dictionaryMathOperations.Max();

        public TKey MaxKey() => _dictionaryMathOperations.MaxKey();

        public TKey MinKey() => _dictionaryMathOperations.MinKey();

        public double Average(Func<KeyValuePair<TKey, TValue>, double> selector) => _dictionaryMathOperations.Average(selector);

        public IOrderedEnumerable<KeyValuePair<TKey, TValue>> OrderByDescending(
            Func<KeyValuePair<TKey, TValue>, object> selector) => _dictionaryOperations.OrderByDescending(selector);

        public IOrderedEnumerable<KeyValuePair<TKey, TValue>>
            OrderBy(Func<KeyValuePair<TKey, TValue>, object> selector) => _dictionaryOperations.OrderBy(selector);

        public List<KeyValuePair<TKey, TValue>> Reverse() => _dictionaryOperations.Reverse();

        public KeyValuePair<TKey, TValue>[] ToArray() => _dictionaryOperations.ToArray();

        public List<KeyValuePair<TKey, TValue>> ToList() => _dictionaryOperations.ToList();

        public IEnumerable<KeyValuePair<TKey, TValue>> Take(int count) => _dictionaryOperations.Take(count);

        public IEnumerable<KeyValuePair<TKey, TValue>> TakeWhile(Func<KeyValuePair<TKey, TValue>, bool> predicate) =>
            _dictionaryOperations.TakeWhile(predicate);

        public IEnumerable<KeyValuePair<TKey, TValue>> Skip(int number) => _dictionaryOperations.Skip(number);

        public IEnumerable<KeyValuePair<TKey, TValue>> SkipWhile(Func<KeyValuePair<TKey, TValue>, bool> predicate) =>
            _dictionaryOperations.SkipWhile(predicate);

        public IEnumerable<KeyValuePair<TKey, TValue>> DistinctByKey() => _dictionaryOperations.DistinctByKey();

        public IEnumerable<KeyValuePair<TKey, TValue>> Except(FastPersistentDictionary<TKey, TValue> otherDict) =>
            _dictionaryOperations.Except(otherDict);

        public void Concat(FastPersistentDictionary<TKey, TValue> other) => _dictionaryOperations.Concat(other);

        public FastPersistentDictionary<TKey, TValue> Intersect(
            FastPersistentDictionary<TKey, TValue> other) => _dictionaryOperations.Intersect(other);

        public FastPersistentDictionary<TKey, TValue> Union(FastPersistentDictionary<TKey, TValue> other) =>
            _dictionaryOperations.Union(other);

        public FastPersistentDictionary<TKey, Tuple<TValue, TSecond>> Zip<TSecond>(
            FastPersistentDictionary<TKey, TSecond> second) => _dictionaryOperations.Zip(second);

        public ImmutableList<KeyValuePair<TKey, TValue>> ToImmutableList() => _dictionaryOperations.ToImmutableList();

        public ImmutableArray<KeyValuePair<TKey, TValue>> ToImmutableArray() =>
            _dictionaryOperations.ToImmutableArray();

        public ImmutableDictionary<TKey, TValue> ToImmutableDictionary() =>
            _dictionaryOperations.ToImmutableDictionary();

        public ImmutableHashSet<KeyValuePair<TKey, TValue>> ToImmutableHashSet() =>
            _dictionaryOperations.ToImmutableHashSet();

        public SortedSet<KeyValuePair<TKey, TValue>> ToSortedSet() => _dictionaryOperations.ToSortedSet();

        public SortedDictionary<TKey, TValue> ToSortedDictionary() => _dictionaryOperations.ToSortedDictionary();

        public IQueryable<KeyValuePair<TKey, TValue>> AsQueryable() => _dictionaryOperations.AsQueryable();

        public ParallelQuery<KeyValuePair<TKey, TValue>> AsParallel() => _dictionaryOperations.AsParallel();

        public void Merge(Dictionary<TKey, TValue> other, bool overwriteExistingKeys = true) => _dictionaryOperations.Merge(other, overwriteExistingKeys);

        public void Merge(FastPersistentDictionary<TKey, TValue> other, bool overwriteExistingKeys = true) => _dictionaryOperations.Merge(other, overwriteExistingKeys);

        public void Join(Dictionary<TKey, TValue> other, bool overwriteExistingKeys = false) => _dictionaryOperations.Join(other, overwriteExistingKeys);

        public void Join(FastPersistentDictionary<TKey, TValue> other, bool overwriteExistingKeys = false) => _dictionaryOperations.Join(other, overwriteExistingKeys);

        public FastPersistentDictionary<TKey, TValue> Copy(string path) => _dictionaryOperations.Copy(path);

        public FastPersistentDictionary<TValue, TKey> Invert() => _dictionaryOperations.Invert();

        public IEnumerable<KeyValuePair<TKey, TValue>> TakeLast(int number) => _dictionaryOperations.TakeLast(number);

        public IEnumerable<IEnumerable<KeyValuePair<TKey, TValue>>> InBatchesOf(int batchSize) => _dictionaryOperations.InBatchesOf(batchSize);

        public void Shuffle() => _dictionaryOperations.Shuffle();

        public IEnumerable<KeyValuePair<TKey, TValue>> SkipLast(int number) => _dictionaryOperations.SkipLast(number);

        public KeyValuePair<TKey, TValue> Single(Func<KeyValuePair<TKey, TValue>, bool> predicate = null) =>
            _dictionaryQuery.Single(predicate);

        private void UpdateTimerElapsedEventHandler(object sender, ElapsedEventArgs e) => _dictionaryQuery.UpdateDatabaseTick();

        public void Peek(Action<KeyValuePair<TKey, TValue>> action) => _dictionaryQuery.Peek(action);

        public void CompactDatabaseFile() => _dictionaryQuery.CompactDatabaseFile();

        public KeyValuePair<TKey, TValue> SingleOrDefault(Func<KeyValuePair<TKey, TValue>, bool> predicate = null) =>
            _dictionaryQuery.SingleOrDefault(predicate);

        public bool Any() => _dictionaryQuery.Any();

        public bool Any(Func<TKey, TValue, bool> predicate) => _dictionaryQuery.Any(predicate);

        public IEnumerable<KeyValuePair<TKey, TValue>> Where(Func<TKey, TValue, bool> predicate) =>
            _dictionaryQuery.Where(predicate);

        public KeyValuePair<TKey, TValue>? FirstOrDefault() => _dictionaryQuery.FirstOrDefault();

        public KeyValuePair<TKey, TValue> Last() => _dictionaryQuery.Last();

        public KeyValuePair<TKey, TValue>? LastOrDefault() => _dictionaryQuery.LastOrDefault();

        public KeyValuePair<TKey, TValue> First() => _dictionaryQuery.First();

        public KeyValuePair<TKey, TValue> ElementAt(int index) => _dictionaryQuery.ElementAt(index);

        public bool All(Func<KeyValuePair<TKey, TValue>, bool> predicate) => _dictionaryQuery.All(predicate);

        public TKey FindKeyByValue(TValue value) => _dictionaryQuery.FindKeyByValue(value);

        public List<TKey> FindAllKeysByValue(TValue value) => _dictionaryQuery.FindAllKeysByValue(value);

        public FastPersistentDictionary<TKey, TValue> GetSubset(Func<TKey, TValue, bool> predicate) => _dictionaryQuery.GetSubset(predicate);

        public FastPersistentDictionary<TKey, TValue> GetSubset(List<TKey> keys) => _dictionaryQuery.GetSubset(keys);

        public int CountValues(TValue value) => _dictionaryQuery.CountValues(value);

        public FastPersistentDictionary<TKey, TValue> FilteredWhere(Func<TKey, TValue, bool> predicate) =>
            _dictionaryQuery.FilteredWhere(predicate);

        public IDictionary<TKey, (TValue value, int index)> Index() => _dictionaryQuery.Index();

        public (IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>) Partition(Func<KeyValuePair<TKey, TValue>, bool> predicate) =>
            _dictionaryQuery.Partition(predicate);

        public IEnumerable<T> Flatten<T>(IEnumerable<IEnumerable<T>> enumerable)
        {
            foreach (var innerEnumerable in enumerable)
                foreach (var item in innerEnumerable)
                    yield return item;
        }
    }
}