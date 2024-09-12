namespace FastPersistentDictionary.Tests;

public sealed partial class PersistentDictionaryProTests
{
    [TestFixture]
    public class ConstructorTests
    {
        //[Test]
        //public void DefaultConstructor_SetsDefaultValues()
        //{
        //    var dictionary = new FastPersistentDictionary<int, string>();


        //    Assert.AreEqual(0f, dictionary.PercentageChangeBeforeCompact);
        //    Assert.IsNotNull(dictionary.DictionarySerializedLookup);
        //    Assert.IsNotNull(dictionary.FileLocation);
        //    Assert.IsNotNull(dictionary.FastPersistentDictionaryVersion);
        //    Assert.IsNotNull(dictionary.FileStream);
        //    Assert.IsNotNull(dictionary.DictionaryAccessor);
        //    Assert.IsNotNull(dictionary.Comparer);
        //    Assert.IsNotNull(dictionary.Count);
        //    Assert.IsNotNull(dictionary.Keys);
        //    Assert.IsNotNull(dictionary.Values);
        //}

        [Test]
        public void CustomConstructor_SetsCustomValues()
        {
            var path = "./tempdictSave.pcd";
            var updateRate = 100;
            var percentageChangeBeforeCompact = 0.2f;
            var importSavedPersistentDictionaryPro = "savedDictionaryPath";
            var equalityComparer = EqualityComparer<int>.Default;


            var dictionary = new FastPersistentDictionary<int, string>(
                path,
                false,
                updateRate,
                true,
                percentageChangeBeforeCompact,
                8196,
                importSavedPersistentDictionaryPro,
                equalityComparer);


            Assert.AreEqual(percentageChangeBeforeCompact, dictionary.PercentageChangeBeforeCompact);
            // Assert.IsNotNull(dictionary.DictionarySerializedCache);
            Assert.IsNotNull(dictionary.DictionarySerializedLookup);
            //   Assert.IsNotNull(dictionary.DictionaryChangedEntries);
            Assert.AreEqual(path, dictionary.FileLocation);
            Assert.IsNotNull(dictionary.FastPersistentDictionaryVersion);
            Assert.IsNotNull(dictionary.FileStream);
            Assert.IsNotNull(dictionary.DictionaryAccessor);
            Assert.AreEqual(equalityComparer, dictionary.Comparer);
            Assert.IsNotNull(dictionary.Count);
            Assert.IsNotNull(dictionary.Keys);
            Assert.IsNotNull(dictionary.Values);
        }
    }
}