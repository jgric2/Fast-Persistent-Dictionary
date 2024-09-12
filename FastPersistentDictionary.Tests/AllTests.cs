using FastPersistentDictionary.Internals;
using System.Collections.Immutable;


namespace FastPersistentDictionary.Tests
{
    public sealed class PersistentDictionaryProAllTests
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
            //    // Assert.IsNotNull(dictionary.DictionaryChangedEntries);
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

        [TestFixture]
        public class DisposeTests
        {
            [Test]
            public void Dispose_CallsDisposeMethod()
            {
                using (var dictionary = new FastPersistentDictionary<int, string>())
                {
                    dictionary.Dispose();
                }
            }

            [Test]
            public void Dispose_CleansUpResources()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                dictionary.Dispose();
                Assert.IsNull(dictionary.FileStream);
            }

            [Test]
            public void Dispose_CanBeCalledMultipleTimes()
            {
                var dictionary = new FastPersistentDictionary<int, string>();

                dictionary.Dispose();
                dictionary.Dispose();
            }
        }

        [TestFixture]
        public class IndexTests
        {
            [Test]
            public void Index_ReturnsDictionaryIndexedByKeys()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                dictionary.Add(1, "One");
                dictionary.Add(2, "Two");
                dictionary.Add(3, "Three");

                var indexedDictionary = dictionary.Index();

                Assert.AreEqual(3, indexedDictionary.Count);
                Assert.IsTrue(indexedDictionary.ContainsKey(1));
                Assert.IsTrue(indexedDictionary.ContainsKey(2));
                Assert.IsTrue(indexedDictionary.ContainsKey(3));
                Assert.AreEqual(("One", 0), indexedDictionary[1]);
                Assert.AreEqual(("Two", 1), indexedDictionary[2]);
                Assert.AreEqual(("Three", 2), indexedDictionary[3]);
            }

            [Test]
            public void Index_ReturnsEmptyDictionaryForEmptyPersistentDictionaryPro()
            {
                var dictionary = new FastPersistentDictionary<int, string>();

                var indexedDictionary = dictionary.Index();
                Assert.AreEqual(0, indexedDictionary.Count);
            }

            [Test]
            public void Index_ReturnsDictionaryIndexedByValues()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                dictionary.Add(1, "One");
                dictionary.Add(2, "Two");
                dictionary.Add(3, "Three");

                var indexedDictionary = dictionary.Index().ToDictionary(x => x.Value.value, x => x.Value.index);

                Assert.AreEqual(3, indexedDictionary.Count);
                Assert.IsTrue(indexedDictionary.ContainsKey("One"));
                Assert.IsTrue(indexedDictionary.ContainsKey("Two"));
                Assert.IsTrue(indexedDictionary.ContainsKey("Three"));
                Assert.AreEqual(0, indexedDictionary["One"]);
                Assert.AreEqual(1, indexedDictionary["Two"]);
                Assert.AreEqual(2, indexedDictionary["Three"]);
            }
        }

        [TestFixture]
        public class IndexerTests
        {
            [Test]
            public void GetSetIndexer_GetsAndSetsValue()
            {
                var dictionary = new FastPersistentDictionary<int, string>();

                dictionary[1] = "one";
                var value = dictionary[1];

                Assert.AreEqual("one", value);
            }
        }

        [TestFixture]
        public class ComparerTests
        {
            [Test]
            public void Comparer_DefaultComparer_ReturnsDefaultComparer()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                var comparer = dictionary.Comparer;

                Assert.AreEqual(EqualityComparer<int>.Default, comparer);
            }
        }

        [TestFixture]
        public class CountTests
        {
            [TestFixture]
            public class WhenDictionaryIsEmpty
            {
                [Test]
                public void ReturnsZero()
                {
                    var dictionary = new FastPersistentDictionary<int, string>();
                    var count = dictionary.Count;

                    Assert.AreEqual(0, count);
                }
            }

            [TestFixture]
            public class WhenDictionaryHasOneElement
            {
                [Test]
                public void ReturnsOne()
                {
                    var dictionary = new FastPersistentDictionary<int, string>();
                    dictionary.Add(1, "Value 1");

                    var count = dictionary.Count;
                    Assert.AreEqual(1, count);
                }
            }

            [TestFixture]
            public class WhenDictionaryHasMultipleElements
            {
                [Test]
                public void ReturnsCorrectCount()
                {
                    var dictionary = new FastPersistentDictionary<int, string>();
                    dictionary.Add(1, "Value 1");
                    dictionary.Add(2, "Value 2");
                    dictionary.Add(3, "Value 3");

                    var count = dictionary.Count;
                    Assert.AreEqual(3, count);
                }
            }
        }

        [TestFixture]
        public class KeysTests
        {
            [Test]
            public void EmptyDictionary_ReturnsEmptyCollection()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                var keys = dictionary.Keys;
                Assert.IsEmpty(keys);
            }

            [Test]
            public void NonEmptyDictionary_ReturnsAllKeys()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                dictionary.Add(1, "one");
                dictionary.Add(2, "two");
                dictionary.Add(3, "three");

                var keys = dictionary.Keys;
                Assert.That(keys, Is.EquivalentTo(new[] { 1, 2, 3 }));
            }

            [Test]
            public void AfterAddingAndRemovingKeys_ReturnsRemainingKeys()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                dictionary.Add(1, "one");
                dictionary.Add(2, "two");
                dictionary.Add(3, "three");
                dictionary.Remove(2);
                var keys = dictionary.Keys;
                Assert.That(keys, Is.EquivalentTo(new[] { 1, 3 }));
            }
        }

        [TestFixture]
        public class LastTests
        {
            [TestFixture]
            public class WhenDictionaryIsEmpty
            {
                [Test]
                public void ThrowsInvalidOperationException()
                {
                    var dictionary = new FastPersistentDictionary<int, string>();

                    Assert.Throws<KeyNotFoundException>(() => dictionary.Last());
                }

                [Test]
                public void ReturnsDefaultValueIfDefaultIsSpecified()
                {
                    var dictionary = new FastPersistentDictionary<int, string>();

                    var lastKeyValuePair = dictionary.LastOrDefault();

                    Assert.AreEqual(default(KeyValuePair<int, string>), lastKeyValuePair);
                }
            }

            [TestFixture]
            public class WhenDictionaryIsNotEmpty
            {
                [Test]
                public void ReturnsLastKeyValuePair()
                {
                    var dictionary = new FastPersistentDictionary<int, string>();
                    dictionary.Add(1, "One");
                    dictionary.Add(2, "Two");
                    dictionary.Add(3, "Three");

                    var lastKeyValuePair = dictionary.Last();

                    Assert.AreEqual(3, lastKeyValuePair.Key);
                    Assert.AreEqual("Three", lastKeyValuePair.Value);
                }
            }
        }


        [TestFixture]
        public class LoadDictionaryTests
        {
            [Test]
            public void LoadDictionary_PathDoesNotExist_ThrowsFileNotFoundException()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                var loadPath = "nonexistentDictionary.pcd";

                Assert.Throws<InvalidDataException>(() => dictionary.LoadDictionary(loadPath));
            }

            [Test]
            public void LoadDictionary_ValidPath_LoadsDictionaryFromPath()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                var savePath = "./validDictionary.pcd";
                var header = dictionary.SaveDictionary(savePath);

                var loadedHeader = dictionary.LoadDictionary(savePath);

                Assert.AreEqual(header.Name, loadedHeader.Name);
                Assert.AreEqual(header.Comment, loadedHeader.Comment);
            }

        }


        [TestFixture]
        public class LastOrDefaultTests
        {
            [Test]
            public void LastOrDefault_NonEmptyDictionary_ReturnsLastKeyValuePair()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                dictionary.Add(1, "one");
                dictionary.Add(2, "two");
                dictionary.Add(3, "three");

                var result = dictionary.LastOrDefault();
                Assert.AreEqual(new KeyValuePair<int, string>(3, "three"), result);
            }


            [Test]
            public void LastOrDefault_PredicateNonEmptyDictionary_ReturnsLastMatchingKeyValuePair()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                dictionary.Add(1, "one");
                dictionary.Add(2, "two");
                dictionary.Add(3, "three");

                var result = dictionary.LastOrDefault(kvp => kvp.Key < 3);

                Assert.AreEqual(new KeyValuePair<int, string>(2, "two"), result);
            }
        }

        [TestFixture]
        public class ValuesTests
        {
            [Test]
            public void Values_EmptyDictionary_ReturnsEmptyCollection()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                var values = dictionary.Values;

                Assert.IsEmpty(values);
            }

            [Test]
            public void Values_NonEmptyDictionary_ReturnsAllValues()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                dictionary.Add(1, "one");
                dictionary.Add(2, "two");

                var values = dictionary.Values;

                Assert.Contains("one", values.ToList());
                Assert.Contains("two", values.ToList());
            }
        }

        [TestFixture]
        public class AddTests
        {
            [Test]
            public void Add_AddsKeyValuePairToDictionary()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                dictionary.Add(1, "one");

                Assert.IsTrue(dictionary.ContainsKey(1));
                Assert.AreEqual("one", dictionary[1]);
            }

            [Test]
            public void Add_AddsDuplicateKey_ThrowsException()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                var key = 1;
                var value1 = "One";
                var value2 = "Two";
                dictionary.Add(key, value1);

                Assert.Throws<ArgumentException>(() => dictionary.Add(key, value2));
            }

            [Test]
            public void Add_AddsNullKey_ThrowsException()
            {
                var dictionary = new FastPersistentDictionary<string, int>();
                string key = null;
                var value = 1;

                Assert.Throws<ArgumentNullException>(() => dictionary.Add(key, value));
            }

            [Test]
            public void Add_AddsNullValue()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                var key = 1;
                string value = null;

                dictionary.Add(key, value);

                Assert.IsTrue(dictionary.ContainsKey(key));
                Assert.IsNull(dictionary[key]);
            }

            [Test]
            public void Add_AddsMultipleKeyValuePairsToDictionary()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                var pairs = new List<KeyValuePair<int, string>>
                {
                    new KeyValuePair<int, string>(1, "One"),
                    new KeyValuePair<int, string>(2, "Two"),
                    new KeyValuePair<int, string>(3, "Three")
                };

                foreach (var pair in pairs) dictionary.Add(pair.Key, pair.Value);

                foreach (var pair in pairs)
                {
                    Assert.IsTrue(dictionary.ContainsKey(pair.Key));
                    Assert.AreEqual(pair.Value, dictionary[pair.Key]);
                }
            }

            [Test]
            public void Add_AddsKeyValuePairWithCustomEqualityComparer()
            {
                var dictionary =
                    new FastPersistentDictionary<string, int>(equalityComparer: StringComparer.OrdinalIgnoreCase);
                var key1 = "One";
                var key2 = "ONE";
                var value = 1;

                dictionary.Add(key1, value);
                dictionary.Add(key2, value);

                Assert.IsTrue(dictionary.ContainsKey(key1));
                Assert.IsTrue(dictionary.ContainsKey(key2));
                Assert.AreEqual(value, dictionary[key1]);
                Assert.AreEqual(value, dictionary[key2]);
            }
        }

        [TestFixture]
        public class AddOrUpdateTests
        {
            [TestFixture]
            public class WhenKeyDoesNotExist
            {
                [Test]
                public void AddsNewKeyValuePairToDictionary()
                {
                    var dictionary = new FastPersistentDictionary<int, string>();
                    dictionary.AddOrUpdate(1, "Value 1");

                    Assert.AreEqual("Value 1", dictionary[1]);
                }
            }

            [TestFixture]
            public class WhenKeyExists
            {
                [Test]
                public void UpdatesValueForExistingKey()
                {
                    var dictionary = new FastPersistentDictionary<int, string>();
                    dictionary.Add(1, "Value 1");

                    dictionary.AddOrUpdate(1, "Updated Value");

                    Assert.AreEqual("Updated Value", dictionary[1]);
                }
            }

            [TestFixture]
            public class WhenDefaultEqualityComparerIsUsed
            {
                [Test]
                public void AddsNewKeyValuePairToDictionaryWithDifferentKeyInstance()
                {
                    var dictionary = new FastPersistentDictionary<string, int>();

                    dictionary.AddOrUpdate("Key 1", 1);

                    Assert.AreEqual(1, dictionary["Key 1"]);
                }

                [Test]
                public void UpdatesValueForExistingKeyWithDifferentKeyInstance()
                {
                    var dictionary = new FastPersistentDictionary<string, int>();
                    dictionary.Add("Key 1", 1);

                    dictionary.AddOrUpdate("Key 1", 2);

                    Assert.AreEqual(2, dictionary["Key 1"]);
                }
            }

            [TestFixture]
            public class WhenCustomEqualityComparerIsUsed
            {
                private class CaseInsensitiveStringComparer : IEqualityComparer<string>
                {
                    public bool Equals(string x, string y)
                    {
                        return string.Equals(x, y, StringComparison.OrdinalIgnoreCase);
                    }

                    public int GetHashCode(string obj)
                    {
                        return obj.GetHashCode();
                    }
                }

                [Test]
                public void AddsNewKeyValuePairToDictionaryWithDifferentKeyInstanceButSameValueCase()
                {
                    var dictionary =
                        new FastPersistentDictionary<string, int>(
                            equalityComparer: new CaseInsensitiveStringComparer());

                    dictionary.AddOrUpdate("Key 1", 1);

                    Assert.AreEqual(1, dictionary["Key 1"]);
                }

                [Test]
                public void UpdatesValueForExistingKeyWithDifferentKeyInstanceButSameValueCase()
                {
                    var dictionary =
                        new FastPersistentDictionary<string, int>(
                            equalityComparer: new CaseInsensitiveStringComparer());
                    dictionary.Add("Key 1", 1);

                    dictionary.AddOrUpdate("Key 1", 2);

                    Assert.AreEqual(2, dictionary["Key 1"]);
                }
            }
        }

        [TestFixture]
        public class AllTests
        {
            [TestFixture]
            public class WhenDictionaryIsEmpty
            {
                [SetUp]
                public void SetUp()
                {
                    dictionary = new FastPersistentDictionary<int, string>();
                }

                private FastPersistentDictionary<int, string> dictionary;

                [Test]
                public void ReturnsTrue_WhenPredicateIsAlwaysTrue()
                {
                    bool predicate(KeyValuePair<int, string> kvp)
                    {
                        return true;
                    }

                    var result = dictionary.All(predicate);

                    Assert.IsTrue(result);
                }

                [Test]
                public void ReturnsTrue_WhenPredicateIsAlwaysFalse()
                {
                    bool predicate(KeyValuePair<int, string> kvp)
                    {
                        return false;
                    }

                    var result = dictionary.All(predicate);
                    Assert.IsTrue(result);
                }

                [Test]
                public void ReturnsTrue_WhenPredicateIsEmpty()
                {
                    bool predicate(KeyValuePair<int, string> kvp)
                    {
                        throw new Exception("This predicate should not be called");
                    }

                    var result = dictionary.All(predicate);

                    Assert.IsTrue(result);
                }
            }

            [TestFixture]
            public class WhenDictionaryHasElements
            {
                [SetUp]
                public void SetUp()
                {
                    dictionary = new FastPersistentDictionary<int, string>
                    {
                        { 1, "One" },
                        { 2, "Two" },
                        { 3, "Three" }
                    };
                }

                private FastPersistentDictionary<int, string> dictionary;

                [Test]
                public void ReturnsTrue_WhenAllElementsMatchPredicate()
                {
                    bool predicate(KeyValuePair<int, string> kvp)
                    {
                        return kvp.Key > 0 && kvp.Value.Length > 0;
                    }

                    var result = dictionary.All(predicate);

                    Assert.IsTrue(result);
                }

                [Test]
                public void ReturnsFalse_WhenNotAllElementsMatchPredicate()
                {
                    bool predicate(KeyValuePair<int, string> kvp)
                    {
                        return kvp.Key == 1 && kvp.Value.Length > 0;
                    }


                    var result = dictionary.All(predicate);


                    Assert.IsFalse(result);
                }
            }
        }


        [TestFixture]
        public class ContainsTests
        {
            [TestFixture]
            public class WhenDictionaryIsEmpty
            {
                [Test]
                public void ReturnsFalse()
                {
                    var dictionary = new FastPersistentDictionary<int, string>();

                    var item = new KeyValuePair<int, string>(1, "value");


                    var result = dictionary.Contains(item);


                    Assert.IsFalse(result);
                }
            }

            [TestFixture]
            public class WhenDictionaryContainsItem
            {
                [Test]
                public void ReturnsTrue()
                {
                    var dictionary = new FastPersistentDictionary<int, string>();
                    dictionary.Add(1, "value");

                    var item = new KeyValuePair<int, string>(1, "value");


                    var result = dictionary.Contains(item);


                    Assert.IsTrue(result);
                }
            }

            [TestFixture]
            public class WhenDictionaryDoesNotContainItem
            {
                [Test]
                public void ReturnsFalse()
                {
                    var dictionary = new FastPersistentDictionary<int, string>();
                    dictionary.Add(1, "value");


                    var item = new KeyValuePair<int, string>(2, "value");


                    var result = dictionary.Contains(item);


                    Assert.IsFalse(result);
                }
            }
        }

        [TestFixture]
        public class ContainsKeyTests
        {
            [Test]
            public void ContainsKey_ChecksIfKeyExists_ReturnsTrueIfFound()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                dictionary.Add(1, "one");


                var containsKey = dictionary.ContainsKey(1);


                Assert.IsTrue(containsKey);
            }

            [Test]
            public void ContainsKey_ChecksIfKeyExists_ReturnsFalseIfNotFound()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                dictionary.Add(1, "one");


                var containsKey = dictionary.ContainsKey(2);


                Assert.IsFalse(containsKey);
            }

            [Test]
            public void ReturnsFalse_WhenDictionaryIsEmpty()
            {
                var dictionary = new FastPersistentDictionary<int, string>();

                var containsKey = dictionary.ContainsKey(1);


                Assert.IsFalse(containsKey);
            }
        }

        //[TestFixture]
        //public class ConcatTests
        //{
        //    private string _savePath1 = "./tempdictSave1.pcd";
        //    private string _savePath2 = "./tempdictSave2.pcd";


        //    [TearDown]
        //    public void Cleanup()
        //    {
        //     
        //        if (File.Exists(_savePath1))
        //            File.Delete(_savePath1);

        //        if (File.Exists(_savePath2))
        //            File.Delete(_savePath2);
        //    }

        //    [Test]
        //    public void Concat_TwoEmptyDictionaries_ReturnsEmptyDictionary()
        //    {
        //       
        //        var dictionary1 = new PersistentDictionaryPro<int, string>();
        //        var dictionary2 = new PersistentDictionaryPro<int, string>();

        //        
        //        dictionary1.Concat(dictionary2);

        //       
        //        Assert.AreEqual(0, dictionary1.Count);
        //    }

        //    [Test]
        //    public void Concat_OneEmptyDictionary_ReturnsNonEmptyDictionary()
        //    {
        //       
        //        var dictionary1 = new PersistentDictionaryPro<int, string>(_savePath1);
        //        var dictionary2 = new PersistentDictionaryPro<int, string>(_savePath2);

        //        dictionary1.Add(1, "One");
        //        dictionary1.Add(2, "Two");

        //        
        //        dictionary1.Concat(dictionary2);

        //       
        //        Assert.AreEqual(2, dictionary1.Count);
        //        Assert.AreEqual("One", dictionary1[1]);
        //        Assert.AreEqual("Two", dictionary1[2]);

        //      
        //        dictionary1.Dispose();
        //        dictionary2.Dispose();
        //    }

        //    [Test]
        //    public void Concat_TwoDictionariesWithNoOverlappingKeys_ReturnsCombinedDictionary()
        //    {
        //       
        //        var dictionary1 = new PersistentDictionaryPro<int, string>(_savePath1);
        //        var dictionary2 = new PersistentDictionaryPro<int, string>(_savePath2);

        //        dictionary1.Add(5, "Five");
        //        dictionary1.Add(6, "Six");

        //        dictionary2.Add(7, "Seven");
        //        dictionary2.Add(8, "Eight");

        //        
        //        dictionary1.Concat(dictionary2);

        //       
        //        Assert.AreEqual(6, dictionary1.Count);
        //        Assert.AreEqual("One", dictionary1[1]);
        //        Assert.AreEqual("Two", dictionary1[2]);
        //        Assert.AreEqual("Five", dictionary1[5]);
        //        Assert.AreEqual("Six", dictionary1[6]);
        //        Assert.AreEqual("Seven", dictionary1[7]);
        //        Assert.AreEqual("Eight", dictionary1[8]);

        //     
        //        dictionary1.Dispose();
        //        dictionary2.Dispose();
        //    }

        //    [Test]
        //    public void Concat_TwoDictionariesWithOverlappingKeys_ReturnsCombinedDictionary()
        //    {
        //       
        //        var dictionary1 = new PersistentDictionaryPro<int, string>(_savePath1);
        //        var dictionary2 = new PersistentDictionaryPro<int, string>(_savePath2);

        //        dictionary1.Add(1, "NewOne");
        //        dictionary1.Add(2, "NewTwo");

        //        dictionary2.Add(2, "NewTwo");
        //        dictionary2.Add(3, "Three");
        //        dictionary2.Add(4, "Four");

        //        
        //        dictionary1.Concat(dictionary2);

        //       
        //        Assert.AreEqual(3, dictionary1.Count);
        //        Assert.AreEqual("NewOne", dictionary1[1]);
        //        Assert.AreEqual("NewTwo", dictionary1[2]);
        //        Assert.AreEqual("Three", dictionary1[3]);
        //        Assert.AreEqual("Four", dictionary1[4]);

        //       
        //        dictionary1.Dispose();
        //        dictionary2.Dispose();
        //    }
        //}

        [TestFixture]
        public class ContainsValueTests
        {
            [TestFixture]
            public class WhenDictionaryIsEmpty
            {
                [Test]
                public void ReturnsFalse()
                {
                    var dictionary = new FastPersistentDictionary<int, string>();


                    var containsValue = dictionary.ContainsValue("value");


                    Assert.IsFalse(containsValue);
                }
            }

            [TestFixture]
            public class WhenValueExistsInDictionary
            {
                [Test]
                public void ReturnsTrue()
                {
                    var dictionary = new FastPersistentDictionary<int, string>();
                    dictionary.Add(1, "value1");
                    dictionary.Add(2, "value2");
                    dictionary.Add(3, "value3");


                    var containsValue = dictionary.ContainsValue("value2");


                    Assert.IsTrue(containsValue);
                }

                [Test]
                public void ReturnsTrue_WhenValueIsFoundMultipleTimes()
                {
                    var dictionary = new FastPersistentDictionary<int, string>();
                    dictionary.Add(1, "value1");
                    dictionary.Add(2, "value2");
                    dictionary.Add(3, "value2");


                    var containsValue = dictionary.ContainsValue("value2");


                    Assert.IsTrue(containsValue);
                }
            }

            [TestFixture]
            public class WhenValueDoesNotExistInDictionary
            {
                [Test]
                public void ReturnsFalse()
                {
                    var dictionary = new FastPersistentDictionary<int, string>();
                    dictionary.Add(1, "value1");
                    dictionary.Add(2, "value2");
                    dictionary.Add(3, "value3");


                    var containsValue = dictionary.ContainsValue("value4");


                    Assert.IsFalse(containsValue);
                }
            }
        }

        public class ClearTests
        {
            [Test]
            public void Clear_WhenDictionaryIsEmpty_ShouldNotThrowException()
            {
                var dictionary = new FastPersistentDictionary<int, string>();


                dictionary.Clear();


                Assert.AreEqual(0, dictionary.Count);
                Assert.IsEmpty(dictionary.Keys);
            }

            [Test]
            public void Clear_WhenDictionaryHasItems_ShouldRemoveAllItems()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                dictionary.Add(1, "One");
                dictionary.Add(2, "Two");
                dictionary.Add(3, "Three");


                dictionary.Clear();


                Assert.AreEqual(0, dictionary.Count);
                Assert.IsEmpty(dictionary.Keys);
                Assert.IsFalse(dictionary.ContainsKey(1));
                Assert.IsFalse(dictionary.ContainsKey(2));
                Assert.IsFalse(dictionary.ContainsKey(3));
            }

            [Test]
            public void Clear_ShouldClearCachedData()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                dictionary.Add(1, "One");
                dictionary.Add(2, "Two");
                dictionary.Add(3, "Three");


                dictionary.Clear();

                Assert.IsEmpty(dictionary.DictionarySerializedLookup);
                //   Assert.IsEmpty(dictionary.DictionaryChangedEntries);
            }


            [Test]
            public void Clear_ShouldClearDictionarySerializedLookup()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                dictionary.Add(1, "One");
                dictionary.Add(2, "Two");
                dictionary.Add(3, "Three");


                dictionary.Clear();


                Assert.IsEmpty(dictionary.DictionarySerializedLookup);
            }

            [Test]
            public void Clear_ShouldClearDictionaryChangedEntries()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                dictionary.Add(1, "One");
                dictionary.Add(2, "Two");
                dictionary.Add(3, "Three");


                dictionary.Clear();


                // Assert.IsEmpty(dictionary.DictionaryChangedEntries);
            }

            [Test]
            public void Clear_ShouldClearDictionaryAccessor()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                dictionary.Add(1, "One");
                dictionary.Add(2, "Two");
                dictionary.Add(3, "Three");


                dictionary.Clear();


                Assert.AreEqual(0, dictionary.DictionaryAccessor.Count);
                Assert.IsEmpty(dictionary.DictionaryAccessor.Keys);
                Assert.IsEmpty(dictionary.DictionaryAccessor.Values);
            }

            [Test]
            public void Clear_ShouldClearFileStream()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                dictionary.Add(1, "One");
                dictionary.Add(2, "Two");
                dictionary.Add(3, "Three");


                dictionary.Clear();


                Assert.That(dictionary.FileStream.Length == 0);
            }
        }

        [TestFixture]
        public class DistinctByKeyTests
        {
            [Test]
            public void DistinctByKey_ReturnsDistinctKeyValuePairs()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                dictionary.Add(1, "One");
                dictionary.Add(2, "Two");
                dictionary.Add(3, "Three");
                dictionary.Add(4, "Four");
                dictionary.Add(5, "Five");


                var distinctPairs = dictionary.DistinctByKey().ToList();


                Assert.AreEqual(5, distinctPairs.Count);
                Assert.Contains(new KeyValuePair<int, string>(1, "One"), distinctPairs);
                Assert.Contains(new KeyValuePair<int, string>(2, "Two"), distinctPairs);
                Assert.Contains(new KeyValuePair<int, string>(3, "Three"), distinctPairs);
                Assert.Contains(new KeyValuePair<int, string>(4, "Four"), distinctPairs);
                Assert.Contains(new KeyValuePair<int, string>(5, "Five"), distinctPairs);
            }

            [Test]
            public void DistinctByKey_WithCustomEqualityComparer_ReturnsDistinctKeyValuePairs()
            {
                var dictionary =
                    new FastPersistentDictionary<int, string>(equalityComparer: new ModEqualityComparer());
                dictionary.Add(1, "One");
                dictionary.Add(2, "Two");
                dictionary.Add(3, "Three");
                dictionary.Add(4, "Four");
                dictionary.Add(5, "Five");


                var distinctPairs = dictionary.DistinctByKey().ToList();


                Assert.AreEqual(5, distinctPairs.Count);
            }

            [Test]
            public void DistinctByKey_EmptyDictionary_ReturnsEmptyList()
            {
                var dictionary = new FastPersistentDictionary<int, string>();


                var distinctPairs = dictionary.DistinctByKey().ToList();


                Assert.IsEmpty(distinctPairs);
            }

            private class ModEqualityComparer : IEqualityComparer<int>
            {
                public bool Equals(int x, int y)
                {
                    return x % 2 == y % 2;
                }

                public int GetHashCode(int obj)
                {
                    return obj % 2;
                }
            }
        }


        [TestFixture]
        public class ClearWithValueTests
        {
            [Test]
            public void ClearWithValue_SetsAllKeyValuePairs_WhenDictionaryIsEmpty()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                var value = "test";


                dictionary.ClearWithValue(value);


                Assert.AreEqual(0, dictionary.Count);
            }

            [Test]
            public void ClearWithValue_SetsAllKeyValuePairsWithMatchingValue_WhenDictionaryHasMultipleMatchingValues()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                dictionary.Add(1, "test1");
                dictionary.Add(2, "test2");
                dictionary.Add(3, "test1");
                dictionary.Add(4, "test2");

                var value = "test1";

                dictionary.ClearWithValue(value);


                Assert.AreEqual(4, dictionary.Count);
                Assert.IsTrue(dictionary[1] == value);
                Assert.IsTrue(dictionary[2] == value);
                Assert.IsTrue(dictionary[3] == value);
                Assert.IsTrue(dictionary[4] == value);
            }

            [Test]
            public void ClearWithValue_SetsAllKeyValuePairsWithMatchingValue_WhenDictionaryHasNoMatchingValues()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                dictionary.Add(1, "test1");
                dictionary.Add(2, "test2");


                dictionary.ClearWithValue("test3");


                Assert.AreEqual(2, dictionary.Count);
                Assert.IsTrue(dictionary.ContainsKey(1));
                Assert.IsTrue(dictionary.ContainsKey(2));
            }

            [Test]
            public void ClearWithValue_SetsAllKeyValuePairsWithNullValues_WhenDictionaryHasSingleNullValue()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                dictionary.Add(1, null);


                dictionary.ClearWithValue("Test");


                Assert.That(dictionary[1] == "Test");
            }

            [Test]
            public void ClearWithValue_SetsAllKeyValuePairsWithNullValues_WhenDictionaryHasNoNullValues()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                dictionary.Add(1, "test1");
                dictionary.Add(2, "test2");


                dictionary.ClearWithValue(null);


                Assert.AreEqual(2, dictionary.Count);
                Assert.IsTrue(dictionary.ContainsKey(1));
                Assert.IsTrue(dictionary.ContainsKey(2));
            }
        }


        [TestFixture]
        public class TryGetMinTests
        {
            [Test]
            public void ReturnsTrueAndMinValue_WhenDictionaryIsNotEmpty()
            {
                var dictionary = new FastPersistentDictionary<int, int>();
                dictionary.Add(1, 10);
                dictionary.Add(2, 5);
                dictionary.Add(3, 3);


                var result = dictionary.TryGetMin(out var minValue);


                Assert.IsTrue(result);
                Assert.AreEqual(3, minValue);
            }

            [Test]
            public void ReturnsFalseAndDefault_WhenDictionaryIsEmpty()
            {
                var dictionary = new FastPersistentDictionary<int, int>();


                var result = dictionary.TryGetMin(out var minValue);


                Assert.IsFalse(result);
                Assert.AreEqual(default(int), minValue);
            }

            [Test]
            public void ReturnsFalseAndDefault_WhenDictionaryContainsOnlyNullValues()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                dictionary.Add(1, null);
                dictionary.Add(2, null);
                dictionary.Add(3, null);


                var result = dictionary.TryGetMin(out var minValue);


                Assert.IsFalse(result);
                Assert.AreEqual(default(string), minValue);
            }

            [Test]
            public void ReturnsTrueAndMinValue_WhenDictionaryContainsOnlyNullAndNonNullValues()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                dictionary.Add(1, null);
                dictionary.Add(2, "abc");
                dictionary.Add(3, null);


                var result = dictionary.TryGetMin(out var minValue);


                Assert.IsTrue(result);
                Assert.AreEqual("abc", minValue);
            }
        }

        [TestFixture]
        public class TryGetMaxTests
        {
            [TestFixture]
            public class WhenDictionaryIsEmpty
            {
                [Test]
                public void ReturnsFalse()
                {
                    var dictionary = new FastPersistentDictionary<int, int>();


                    var result = dictionary.TryGetMax(out var max);


                    Assert.IsFalse(result);
                }
            }

            [TestFixture]
            public class WhenDictionaryContainsElements
            {
                [Test]
                public void ReturnsTrueAndMaxValue()
                {
                    var dictionary = new FastPersistentDictionary<int, int>();
                    dictionary.Add(1, 10);
                    dictionary.Add(2, 20);
                    dictionary.Add(3, 30);


                    var result = dictionary.TryGetMax(out var max);


                    Assert.IsTrue(result);
                    Assert.AreEqual(30, max);
                }
            }

            [TestFixture]
            public class WhenDictionaryContainsElementsWithCustomComparer
            {
                [Test]
                public void ReturnsTrueAndMaxValue()
                {
                    var dictionary =
                        new FastPersistentDictionary<int, int>(equalityComparer: new CustomComparer());
                    dictionary.Add(1, 10);
                    dictionary.Add(2, 20);
                    dictionary.Add(3, 30);


                    var result = dictionary.TryGetMax(out var max);


                    Assert.IsTrue(result);
                    Assert.AreEqual(30, max);
                }
            }

            [TestFixture]
            public class WhenDictionaryContainsElementsOfDifferentType
            {
                [Test]
                public void ReturnsTrueAndMaxValue()
                {
                    var dictionary = new FastPersistentDictionary<int, string>();
                    dictionary.Add(1, "10");
                    dictionary.Add(2, "20");
                    dictionary.Add(3, "30");


                    var result = dictionary.TryGetMax(out var max);


                    Assert.IsTrue(result);
                    Assert.AreEqual("30", max);
                }
            }
        }

        public class CustomComparer : IEqualityComparer<int>
        {
            public bool Equals(int x, int y)
            {
                return x % 10 == y % 10;
            }

            public int GetHashCode(int obj)
            {
                return obj % 10;
            }
        }

        [TestFixture]
        public class TryGetValueTests
        {
            [Test]
            public void TryGetValue_KeyExists_ReturnsTrueAndSetsValue()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                dictionary.Add(1, "Hello World");


                var found = dictionary.TryGetValue(1, out var value);


                Assert.IsTrue(found);
                Assert.AreEqual("Hello World", value);
            }

            [Test]
            public void TryGetValue_KeyDoesNotExist_ReturnsFalseAndNullValue()
            {
                var dictionary = new FastPersistentDictionary<int, string>();


                var found = dictionary.TryGetValue(1, out var value);


                Assert.IsFalse(found);
                Assert.IsNull(value);
            }
        }

        [TestFixture]
        public class TryGetValueOrNullTests
        {
            [SetUp]
            public void Setup()
            {
                _dictionary = new FastPersistentDictionary<int, string>();
            }

            private FastPersistentDictionary<int, string> _dictionary;

            [Test]
            public void TryGetValueOrNull_KeyExists_ReturnsValue()
            {
                _dictionary.Add(1, "Value");


                var result = _dictionary.TryGetValueOrNull(1);


                Assert.AreEqual("Value", result);
            }

            [Test]
            public void TryGetValueOrNull_KeyDoesNotExist_ReturnsNull()
            {
                var result = _dictionary.TryGetValueOrNull(1);


                Assert.IsNull(result);
            }

            [Test]
            public void TryGetValueOrNull_DictionaryIsEmpty_ReturnsNull()
            {
                var result = _dictionary.TryGetValueOrNull(1);


                Assert.IsNull(result);
            }
        }


        [TestFixture]
        public class GetOrCreateTests
        {
            [SetUp]
            public void SetUp()
            {
                _dictionary = new FastPersistentDictionary<int, string>();
            }

            [TearDown]
            public void TearDown()
            {
                _dictionary.Dispose();
            }

            private FastPersistentDictionary<int, string> _dictionary;

            [Test]
            public void GetOrCreate_GetsExistingValueForKey_ReturnsExistingValue()
            {
                var key = 1;
                var existingValue = "existingValue";
                _dictionary.Add(key, existingValue);


                var result = _dictionary.GetOrCreate(key, () => "newValue");


                Assert.AreEqual(existingValue, result);
            }

            [Test]
            public void GetOrCreate_GetsNonExistingValueForKey_ReturnsNewValue()
            {
                var key = 1;


                var result = _dictionary.GetOrCreate(key, () => "newValue");


                Assert.AreEqual("newValue", result);
            }

            [Test]
            public void GetOrCreate_GetsNonExistingValueForKey_CallsCreatorFunction()
            {
                var key = 1;
                var creatorCalled = false;


                _dictionary.GetOrCreate(key, () =>
                {
                    creatorCalled = true;
                    return "newValue";
                });


                Assert.IsTrue(creatorCalled);
            }
        }

        [TestFixture]
        public class GetKeyTypeTests
        {
            [Test]
            public void GetKeyType_ReturnsKeyType()
            {
                var dictionary = new FastPersistentDictionary<int, string>();


                var keyType = dictionary.GetKeyType();


                Assert.AreEqual(typeof(int), keyType);
            }
        }


        [TestFixture]
        public class GetEnumeratorTests
        {
            [Test]
            public void GetEnumerator_WhenDictionaryIsEmpty_ReturnsEmptyEnumerator()
            {
                var dictionary = new FastPersistentDictionary<int, string>();


                var enumerator = dictionary.GetEnumerator();


                Assert.IsFalse(enumerator.MoveNext());
            }

            [Test]
            public void GetEnumerator_WhenDictionaryHasOneElement_ReturnsEnumeratorWithOneElement()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                dictionary.Add(1, "One");


                var enumerator = dictionary.GetEnumerator();


                Assert.IsTrue(enumerator.MoveNext());
                Assert.AreEqual(1, enumerator.Current.Key);
                Assert.AreEqual("One", enumerator.Current.Value);
                Assert.IsFalse(enumerator.MoveNext());
            }

            [Test]
            public void GetEnumerator_WhenDictionaryHasMultipleElements_ReturnsEnumeratorWithAllElements()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                dictionary.Add(1, "One");
                dictionary.Add(2, "Two");
                dictionary.Add(3, "Three");


                var enumerator = dictionary.GetEnumerator();


                Assert.IsTrue(enumerator.MoveNext());
                Assert.AreEqual(1, enumerator.Current.Key);
                Assert.AreEqual("One", enumerator.Current.Value);

                Assert.IsTrue(enumerator.MoveNext());
                Assert.AreEqual(2, enumerator.Current.Key);
                Assert.AreEqual("Two", enumerator.Current.Value);

                Assert.IsTrue(enumerator.MoveNext());
                Assert.AreEqual(3, enumerator.Current.Key);
                Assert.AreEqual("Three", enumerator.Current.Value);

                Assert.IsFalse(enumerator.MoveNext());
            }
        }

        [TestFixture]
        public class HasValueTests
        {
            [SetUp]
            public void SetUp()
            {
                dictionary = new FastPersistentDictionary<int, string>();
            }

            [TearDown]
            public void TearDown()
            {
                dictionary.Dispose();
            }

            private FastPersistentDictionary<int, string> dictionary;

            [Test]
            public void HasValue_ValueExists_ReturnsTrue()
            {
                dictionary.Add(1, "Value");


                var result = dictionary.HasValue("Value");


                Assert.IsTrue(result);
            }

            [Test]
            public void HasValue_ValueDoesNotExist_ReturnsFalse()
            {
                dictionary.Add(1, "Value");


                var result = dictionary.HasValue("OtherValue");


                Assert.IsFalse(result);
            }

            [Test]
            public void HasValue_ValueExistsWithDifferentKey_ReturnsFalse()
            {
                dictionary.Add(1, "Value");


                var result = dictionary.HasValue("Value2");


                Assert.IsFalse(result);
            }

            [Test]
            public void HasValue_ValueIsNull_ReturnsTrueIfNullValueExists()
            {
                dictionary.Add(1, null);


                var result = dictionary.HasValue(null);


                Assert.IsTrue(result);
            }

            [Test]
            public void HasValue_ValueIsNull_ReturnsFalseIfNonNullValueExists()
            {
                dictionary.Add(1, "Value");


                var result = dictionary.HasValue(null);


                Assert.IsFalse(result);
            }

            [Test]
            public void HasValue_EmptyDictionary_ReturnsFalse()
            {
                var result = dictionary.HasValue("Value");


                Assert.IsFalse(result);
            }
        }

        [TestFixture]
        public class HasKeyTests
        {
            [Test]
            public void ReturnsTrue_WhenKeyExistsInDictionary()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                dictionary.Add(1, "One");


                var result = dictionary.HasKey(1);


                Assert.IsTrue(result);
            }

            [Test]
            public void ReturnsFalse_WhenKeyDoesNotExistInDictionary()
            {
                var dictionary = new FastPersistentDictionary<int, string>();


                var result = dictionary.HasKey(1);


                Assert.IsFalse(result);
            }

            [Test]
            public void ReturnsTrue_WhenKeyExistsInEmptyDictionary()
            {
                var dictionary = new FastPersistentDictionary<int, string>();


                var result = dictionary.HasKey(1);


                Assert.IsFalse(result);
            }

            [Test]
            public void ReturnsTrue_WhenUsingCustomEqualityComparerAndKeyExistsInDictionary()
            {
                var dictionary =
                    new FastPersistentDictionary<int, string>(equalityComparer: new CustomEqualityComparer());
                dictionary.Add(1, "One");


                var result = dictionary.HasKey(1);


                Assert.IsTrue(result);
            }

            [Test]
            public void ReturnsFalse_WhenUsingCustomEqualityComparerAndKeyDoesNotExistInDictionary()
            {
                var dictionary =
                    new FastPersistentDictionary<int, string>(equalityComparer: new CustomEqualityComparer());
                dictionary.Add(1, "One");


                var result = dictionary.HasKey(2);


                Assert.IsFalse(result);
            }

            private class CustomEqualityComparer : IEqualityComparer<int>
            {
                public bool Equals(int x, int y)
                {
                    return x == y;
                }

                public int GetHashCode(int obj)
                {
                    return obj.GetHashCode();
                }
            }
        }

        [TestFixture]
        public class GetValueTypeTests
        {
            [Test]
            public void GetValueType_ReturnsValueType()
            {
                var dictionary = new FastPersistentDictionary<int, string>();


                var valueType = dictionary.GetValueType();


                Assert.AreEqual(typeof(string), valueType);
            }
        }

        [TestFixture]
        public class AnyTests
        {
            [Test]
            public void Any_EmptyDictionary_ReturnsFalse()
            {
                var dictionary = new FastPersistentDictionary<int, string>();


                var any = dictionary.Any();


                Assert.IsFalse(any);
            }

            [Test]
            public void Any_DictionaryWithElements_ReturnsTrue()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                dictionary.Add(1, "one");


                var any = dictionary.Any();


                Assert.IsTrue(any);
            }

            [Test]
            public void Any_Predicate_SomeElementsMatch_ReturnsTrue()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                dictionary.Add(1, "one");
                dictionary.Add(2, "two");
                dictionary.Add(3, "three");


                var any = dictionary.Any((key, value) => value.StartsWith("t"));


                Assert.IsTrue(any);
            }

            [Test]
            public void Any_Predicate_NoElementsMatch_ReturnsFalse()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                dictionary.Add(1, "one");
                dictionary.Add(2, "two");
                dictionary.Add(3, "three");


                var any = dictionary.Any((key, value) => value.StartsWith("z"));


                Assert.IsFalse(any);
            }
        }

        [TestFixture]
        public class AnyPredicateTests
        {
            [Test]
            public void Any_Predicate_SomeElementsMatch_ReturnsTrue()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                dictionary.Add(1, "one");
                dictionary.Add(2, "two");
                dictionary.Add(3, "three");


                var any = dictionary.Any((key, value) => value.StartsWith("t"));


                Assert.IsTrue(any);
            }

            [Test]
            public void Any_Predicate_NoElementsMatch_ReturnsFalse()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                dictionary.Add(1, "one");
                dictionary.Add(2, "two");
                dictionary.Add(3, "three");


                var any = dictionary.Any((key, value) => value.StartsWith("z"));


                Assert.IsFalse(any);
            }
        }

        [TestFixture]
        public class WhereTests
        {
            [Test]
            public void Where_Predicate_ReturnsFilteredDictionary()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                dictionary.Add(1, "one");
                dictionary.Add(2, "two");
                dictionary.Add(3, "three");


                var filteredDictionary = dictionary.Where((key, value) => value.Length > 3);


                Assert.AreEqual(1, filteredDictionary.Count());
                Assert.AreEqual("three", filteredDictionary.ElementAt(0).Value);
            }
        }


        [TestFixture]
        public class ZipTests
        {
            [SetUp]
            public void Setup()
            {
                _dict1 = new FastPersistentDictionary<int, string>();
                _dict1.Add(1, "One");
                _dict1.Add(2, "Two");
                _dict1.Add(3, "Three");

                _dict2 = new FastPersistentDictionary<int, int>();
                _dict2.Add(1, 100);
                _dict2.Add(2, 200);
                _dict2.Add(3, 300);
            }

            private FastPersistentDictionary<int, string> _dict1;
            private FastPersistentDictionary<int, int> _dict2;

            [Test]
            public void Zip_ZipsDictionariesWithMatchingKeys_ReturnsDictionaryWithZippedValues()
            {
                var result = _dict1.Zip(_dict2);


                Assert.AreEqual(3, result.Count);
                Assert.AreEqual("One:100", result[1].Item1 + ":" + result[1].Item2);
                Assert.AreEqual("Two:200", result[2].Item1 + ":" + result[2].Item2);
                Assert.AreEqual("Three:300", result[3].Item1 + ":" + result[3].Item2);
            }

            [Test]
            public void Zip_ZipsWithEmptyDictionary_ReturnsEmptyDictionary()
            {
                var emptyDict = new FastPersistentDictionary<int, int>();


                var result = _dict1.Zip(emptyDict);


                Assert.AreEqual(0, result.Count);
            }
        }


        [TestFixture]
        public class CompactDatabaseFileTests
        {
            [Test]
            public void CompactDatabaseFile_CompactsDatabaseFile()
            {
                var dictionary = new FastPersistentDictionary<int, string>();

                for (var i = 0; i < 400; i++) dictionary.Add(i, "test " + i);
                dictionary.UpdateDatabaseTick();

                for (var i = 0; i < 400; i++) dictionary[i] = "test " + i + " - " + new Guid();


                dictionary.UpdateDatabaseTick();


                var size = dictionary.FileStream.Length;

                dictionary.CompactDatabaseFile();


                var sizeCompact = dictionary.FileStream.Length;

                Assert.That(sizeCompact <= size);
            }
        }

        [TestFixture]
        public class PeekTests
        {
            [Test]
            public void Peek_PerformsActionOnEachKeyValuePair()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                dictionary.Add(1, "one");
                dictionary.Add(2, "two");
                var actionCalledCount = 0;


                dictionary.Peek(pair => actionCalledCount++);


                Assert.AreEqual(2, actionCalledCount);
            }
        }

        [TestFixture]
        public class MergeTests
        {
            [Test]
            public void Merge_MergesDictionaryWithOtherDictionary_OverwriteExistingKeysIsFalse()
            {
                var dictionary1 = new FastPersistentDictionary<int, string>();
                dictionary1.Add(1, "One");
                dictionary1.Add(2, "Two");
                dictionary1.Add(3, "Three");

                var dictionary2 = new Dictionary<int, string>
                {
                    { 2, "New Two" },
                    { 4, "Four" },
                    { 5, "Five" }
                };


                dictionary1.Merge(dictionary2, false);


                Assert.AreEqual("One", dictionary1[1]);
                Assert.AreEqual("Two", dictionary1[2]);
                Assert.AreEqual("Three", dictionary1[3]);
                Assert.AreEqual("Four", dictionary1[4]);
                Assert.AreEqual("Five", dictionary1[5]);
                Assert.AreEqual(5, dictionary1.Count);
            }

            [Test]
            public void Merge_MergesDictionaryWithOtherDictionary_OverwriteExistingKeysIsTrue()
            {
                var dictionary1 = new FastPersistentDictionary<int, string>();
                dictionary1.Add(1, "One");
                dictionary1.Add(2, "Two");
                dictionary1.Add(3, "Three");

                var dictionary2 = new Dictionary<int, string>
                {
                    { 2, "New Two" },
                    { 4, "Four" },
                    { 5, "Five" }
                };


                dictionary1.Merge(dictionary2);


                Assert.AreEqual("One", dictionary1[1]);
                Assert.AreEqual("New Two", dictionary1[2]);
                Assert.AreEqual("Three", dictionary1[3]);
                Assert.AreEqual("Four", dictionary1[4]);
                Assert.AreEqual("Five", dictionary1[5]);
                Assert.AreEqual(5, dictionary1.Count);
            }

            [Test]
            public void Merge_MergesDictionaryWithOtherPersistentDictionaryPro_OverwriteExistingKeysIsFalse()
            {
                var dictionary1 = new FastPersistentDictionary<int, string>();
                dictionary1.Add(1, "One");
                dictionary1.Add(2, "Two");
                dictionary1.Add(3, "Three");

                var dictionary2 = new FastPersistentDictionary<int, string>();
                dictionary2.Add(2, "New Two");
                dictionary2.Add(4, "Four");
                dictionary2.Add(5, "Five");


                dictionary1.Merge(dictionary2, false);


                Assert.AreEqual("One", dictionary1[1]);
                Assert.AreEqual("Two", dictionary1[2]);
                Assert.AreEqual("Three", dictionary1[3]);
                Assert.AreEqual("Four", dictionary1[4]);
                Assert.AreEqual("Five", dictionary1[5]);
                Assert.AreEqual(5, dictionary1.Count);
            }

            [Test]
            public void Merge_MergesDictionaryWithOtherPersistentDictionaryPro_OverwriteExistingKeysIsTrue()
            {
                var dictionary1 = new FastPersistentDictionary<int, string>();
                dictionary1.Add(1, "One");
                dictionary1.Add(2, "Two");
                dictionary1.Add(3, "Three");

                var dictionary2 = new FastPersistentDictionary<int, string>();
                dictionary2.Add(2, "New Two");
                dictionary2.Add(4, "Four");
                dictionary2.Add(5, "Five");


                dictionary1.Merge(dictionary2);


                Assert.AreEqual("One", dictionary1[1]);
                Assert.AreEqual("New Two", dictionary1[2]);
                Assert.AreEqual("Three", dictionary1[3]);
                Assert.AreEqual("Four", dictionary1[4]);
                Assert.AreEqual("Five", dictionary1[5]);
                Assert.AreEqual(5, dictionary1.Count);
            }
        }


        [TestFixture]
        public class FindKeyByValueTests
        {
            [SetUp]
            public void SetUp()
            {
                dictionary = new FastPersistentDictionary<int, string>();
            }

            private FastPersistentDictionary<int, string> dictionary;

            [Test]
            public void FindKeyByValue_KeyExists_ReturnsKey()
            {
                dictionary.Add(1, "Value1");
                dictionary.Add(2, "Value2");
                dictionary.Add(3, "Value3");


                var key = dictionary.FindKeyByValue("Value2");


                Assert.AreEqual(2, key);
            }

            [Test]
            public void FindKeyByValue_KeyDoesNotExist_ReturnsDefaultKey()
            {
                dictionary.Add(1, "Value1");
                dictionary.Add(2, "Value2");
                dictionary.Add(3, "Value3");
                Assert.Throws<KeyNotFoundException>(() => dictionary.FindKeyByValue("Value4"));
            }

            [Test]
            public void FindKeyByValue_DuplicateValuesExist_ReturnsFirstMatchingKey()
            {
                dictionary.Add(1, "Value1");
                dictionary.Add(2, "Value2");
                dictionary.Add(3, "Value2");


                var key = dictionary.FindKeyByValue("Value2");


                Assert.AreEqual(2, key);
            }

            [Test]
            public void FindKeyByValue_EmptyDictionary_ReturnsDefaultKey()
            {
                Assert.Throws<KeyNotFoundException>(() => dictionary.FindKeyByValue("Value1"));
            }

            [Test]
            public void FindKeyByValue_NullValue_ReturnsDefaultKey()
            {
                dictionary.Add(1, "Value1");
                dictionary.Add(2, null);
                dictionary.Add(3, "Value3");


                var key = dictionary.FindKeyByValue(null);


                Assert.AreEqual(2, key);
            }
        }

        [TestFixture]
        public class FindAllKeysByValueTests
        {
            [Test]
            public void ReturnsEmptyList_WhenValueDoesNotExist()
            {
                var dictionary = new FastPersistentDictionary<int, string>();


                var result = dictionary.FindAllKeysByValue("value");


                Assert.IsEmpty(result);
            }

            [Test]
            public void ReturnsListOfKeys_WhenValueExists()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                dictionary.Add(1, "value");
                dictionary.Add(2, "value");
                dictionary.Add(3, "value");


                var result = dictionary.FindAllKeysByValue("value");


                Assert.AreEqual(new List<int> { 1, 2, 3 }, result);
            }

            [Test]
            public void ReturnsMultipleListsOfKeys_WhenValuesExist()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                dictionary.Add(1, "value1");
                dictionary.Add(2, "value2");
                dictionary.Add(3, "value1");
                dictionary.Add(4, "value3");
                dictionary.Add(5, "value1");


                var result1 = dictionary.FindAllKeysByValue("value1");
                var result2 = dictionary.FindAllKeysByValue("value2");
                var result3 = dictionary.FindAllKeysByValue("value3");


                Assert.AreEqual(new List<int> { 1, 3, 5 }, result1);
                Assert.AreEqual(new List<int> { 2 }, result2);
                Assert.AreEqual(new List<int> { 4 }, result3);
            }
        }

        [TestFixture]
        public class FilteredWhereTests
        {
            [SetUp]
            public void SetUp()
            {
                _dictionary = new FastPersistentDictionary<int, string>();
                _dictionary.Add(1, "One");
                _dictionary.Add(2, "Two");
                _dictionary.Add(3, "Three");
            }

            private FastPersistentDictionary<int, string> _dictionary;

            [Test]
            public void FilteredWhere_ReturnsSubsetMatchingPredicate()
            {
                Func<int, string, bool> predicate = (key, value) => key % 2 == 0;


                var subset = _dictionary.FilteredWhere(predicate);


                Assert.AreEqual(1, subset.Count);
                Assert.True(subset.ContainsKey(2));
                Assert.AreEqual("Two", subset[2]);
            }

            [Test]
            public void FilteredWhere_ReturnsEmptyDictionary_WhenNoElementsMatchPredicate()
            {
                Func<int, string, bool> predicate = (key, value) => key > 10;


                var subset = _dictionary.FilteredWhere(predicate);


                Assert.IsEmpty(subset);
            }

            [Test]
            public void FilteredWhere_ReturnsAllElements_WhenPredicateMatchesAll()
            {
                Func<int, string, bool> predicate = (key, value) => key > 0;


                var subset = _dictionary.FilteredWhere(predicate);


                Assert.AreEqual(3, subset.Count);
                Assert.True(subset.ContainsKey(1));
                Assert.AreEqual("One", subset[1]);
                Assert.True(subset.ContainsKey(2));
                Assert.AreEqual("Two", subset[2]);
                Assert.True(subset.ContainsKey(3));
                Assert.AreEqual("Three", subset[3]);
            }


            [Test]
            public void FilteredWhere_ReturnsEmptyDictionary_WhenDictionaryIsEmpty()
            {
                var emptyDictionary = new FastPersistentDictionary<int, string>();
                Func<int, string, bool> predicate = (key, value) => key % 2 == 0;


                var subset = emptyDictionary.FilteredWhere(predicate);


                Assert.IsEmpty(subset);
            }
        }

        [TestFixture]
        public class FirstTests
        {
            [TestFixture]
            public class WhenDictionaryIsEmpty
            {
                [Test]
                public void ThrowsInvalidOperationException()
                {
                    var dictionary = new FastPersistentDictionary<int, string>();


                    Assert.Throws<InvalidOperationException>(() => dictionary.First());
                }
            }

            [TestFixture]
            public class WhenDictionaryIsNotEmpty
            {
                [TestFixture]
                public class AndNoPredicateIsProvided
                {
                    [Test]
                    public void ReturnsFirstKeyValuePairInDictionary()
                    {
                        var dictionary = new FastPersistentDictionary<int, string>();
                        dictionary.Add(1, "One");
                        dictionary.Add(2, "Two");
                        dictionary.Add(3, "Three");


                        var result = dictionary.First();


                        Assert.AreEqual(1, result.Key);
                        Assert.AreEqual("One", result.Value);
                    }
                }

                [TestFixture]
                public class AndPredicateIsProvided
                {
                    [TestFixture]
                    public class AndKeyValuePairMatchingPredicateExists
                    {
                        [Test]
                        public void ReturnsFirstKeyValuePairMatchingPredicate()
                        {
                            var dictionary = new FastPersistentDictionary<int, string>();
                            dictionary.Add(1, "One");
                            dictionary.Add(2, "Two");
                            dictionary.Add(3, "Three");


                            var result = dictionary.First(kvp => kvp.Key > 1);


                            Assert.AreEqual(2, result.Key);
                            Assert.AreEqual("Two", result.Value);
                        }
                    }

                    [TestFixture]
                    public class AndNoKeyValuePairMatchingPredicateExists
                    {
                        [Test]
                        public void ThrowsInvalidOperationException()
                        {
                            var dictionary = new FastPersistentDictionary<int, string>();
                            dictionary.Add(1, "One");
                            dictionary.Add(2, "Two");
                            dictionary.Add(3, "Three");


                            Assert.Throws<InvalidOperationException>(() => dictionary.First(kvp => kvp.Key > 3));
                        }
                    }
                }
            }
        }


        [TestFixture]
        public class FirstOrDefaultTests
        {
            [TestFixture]
            public class WhenDictionaryIsEmpty
            {
                [SetUp]
                public void SetUp()
                {
                    dictionary = new FastPersistentDictionary<int, string>();
                }

                private FastPersistentDictionary<int, string> dictionary;

                [Test]
                public void ReturnsDefaultKeyValuePair()
                {
                    var result = dictionary.FirstOrDefault();


                    Assert.AreEqual(default(KeyValuePair<int, string>), result);
                }

                [Test]
                public void ReturnsDefaultKeyValuePairWithPredicate()
                {
                    var result = dictionary.FirstOrDefault(x => x.Key > 0);


                    Assert.AreEqual(default(KeyValuePair<int, string>), result);
                }
            }

            [TestFixture]
            public class WhenDictionaryIsNotEmpty
            {
                [SetUp]
                public void SetUp()
                {
                    dictionary = new FastPersistentDictionary<int, string>();
                    dictionary.Add(1, "one");
                    dictionary.Add(2, "two");
                    dictionary.Add(3, "three");
                }

                private FastPersistentDictionary<int, string> dictionary;

                [Test]
                public void ReturnsFirstKeyValuePair()
                {
                    var result = dictionary.FirstOrDefault();


                    Assert.AreEqual(new KeyValuePair<int, string>(1, "one"), result);
                }

                [Test]
                public void ReturnsFirstOrDefaultKeyValuePairWithPredicate()
                {
                    var result = dictionary.FirstOrDefault(x => x.Value.Length > 3);


                    Assert.AreEqual(new KeyValuePair<int, string>(3, "three"), result);
                }

                [Test]
                public void ReturnsDefaultKeyValuePairWithFalsePredicate()
                {
                    var result = dictionary.FirstOrDefault(x => x.Key > 10);


                    Assert.AreEqual(default(KeyValuePair<int, string>), result);
                }
            }
        }

        [TestFixture]
        public class RemoveAllTests
        {
            [Test]
            public void RemoveAll_RemovesMatchingKeyValuePairsBasedOnPredicate()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                dictionary.Add(1, "one");
                dictionary.Add(2, "two");
                dictionary.Add(3, "three");


                dictionary.RemoveAll((key, value) => key % 2 == 0);


                Assert.AreEqual(2, dictionary.Count);
                Assert.IsFalse(dictionary.ContainsKey(2));
            }
        }

        [TestFixture]
        public class RemoveTests
        {
            [Test]
            public void Remove_ExistingKeyValuePair_ReturnsTrue()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                dictionary.Add(1, "One");


                var result = dictionary.Remove(1);


                Assert.True(result);
                Assert.AreEqual(0, dictionary.Count);
                Assert.IsFalse(dictionary.ContainsKey(1));
            }

            [Test]
            public void Remove_NonExistingKeyValuePair_ReturnsFalse()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                dictionary.Add(1, "One");


                var result = dictionary.Remove(2);


                Assert.False(result);
                Assert.AreEqual(1, dictionary.Count);
                Assert.IsTrue(dictionary.ContainsKey(1));
            }
        }

        [TestFixture]
        public class TakeTests
        {
            [Test]
            public void Take_ReturnsFirstNKeyValuePairs()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                dictionary.Add(1, "one");
                dictionary.Add(2, "two");
                dictionary.Add(3, "three");


                var result = dictionary.Take(2);


                Assert.AreEqual(2, result.Count());
                Assert.IsTrue(result.Contains(new KeyValuePair<int, string>(1, "one")));
                Assert.IsTrue(result.Contains(new KeyValuePair<int, string>(2, "two")));
            }
        }


        [TestFixture]
        public class MaxKeyTests
        {
            [SetUp]
            public void SetUp()
            {
                _dictionary = new FastPersistentDictionary<int, string>();
            }

            [TearDown]
            public void TearDown()
            {
                _dictionary.Dispose();
                _dictionary = null;
            }

            private const int Key1 = 1;
            private const int Key2 = 2;

            private FastPersistentDictionary<int, string> _dictionary;

            [Test]
            public void MaxKey_WhenDictionaryIsEmpty_ThrowsInvalidOperationException()
            {
                Assert.Throws<InvalidOperationException>(() => _dictionary.MaxKey());
            }

            [Test]
            public void MaxKey_WhenDictionaryContainsSingleKeyValuePair_ReturnsTheKey()
            {
                _dictionary.Add(Key1, "Value1");


                var result = _dictionary.MaxKey();


                Assert.AreEqual(Key1, result);
            }

            [Test]
            public void MaxKey_WhenDictionaryContainsMultipleKeyValuePairs_ReturnsTheMaximumKey()
            {
                _dictionary.Add(Key1, "Value1");
                _dictionary.Add(Key2, "Value2");


                var result = _dictionary.MaxKey();


                Assert.AreEqual(Key2, result);
            }

            [Test]
            public void MaxKey_WhenCustomComparerIsUsed_ReturnsTheMaximumKeyBasedOnTheComparer()
            {
                _dictionary = new FastPersistentDictionary<int, string>(equalityComparer: new CustomComparer());
                _dictionary.Add(Key1, "Value1");
                _dictionary.Add(Key2, "Value2");


                var result = _dictionary.MaxKey();


                Assert.AreEqual(Key2, result);
            }


            private class CustomComparer : IEqualityComparer<int>
            {
                public bool Equals(int x, int y)
                {
                    return x == y;
                }

                public int GetHashCode(int obj)
                {
                    return obj.GetHashCode();
                }
            }

            private class ComplexKey
            {
                public ComplexKey(int id, string name)
                {
                    Id = id;
                    Name = name;
                }

                public int Id { get; }
                public string Name { get; }

                public override bool Equals(object obj)
                {
                    if (obj == null || GetType() != obj.GetType()) return false;

                    var otherKey = (ComplexKey)obj;
                    return otherKey.Id == Id && otherKey.Name == Name;
                }
            }
        }

        [TestFixture]
        public class MaxTests
        {
            [Test]
            public void Max_ReturnsMaxValue()
            {
                var dictionary = new FastPersistentDictionary<int, int>();
                dictionary.Add(1, 10);
                dictionary.Add(2, 20);
                dictionary.Add(3, 30);


                var maxValue = dictionary.Max();


                Assert.AreEqual(30, maxValue);
            }

            [Test]
            public void Max_EmptyDictionary_ThrowsInvalidOperationException()
            {
                var dictionary = new FastPersistentDictionary<int, int>();


                Assert.Throws<InvalidOperationException>(() => dictionary.Max());
            }
        }

        [TestFixture]
        public class MinKeyTests
        {
            [SetUp]
            public void Setup()
            {
                dictionary = new FastPersistentDictionary<int, string>();
            }

            private FastPersistentDictionary<int, string> dictionary;

            [Test]
            public void EmptyDictionary_ThrowsInvalidOperationException()
            {
                Assert.Throws<InvalidOperationException>(() => dictionary.MinKey());
            }

            [Test]
            public void DictionaryWithOneKeyValuePair_ReturnsKey()
            {
                dictionary.Add(1, "Value");


                var minKey = dictionary.MinKey();


                Assert.AreEqual(1, minKey);
            }

            [Test]
            public void DictionaryWithMultipleKeyValuePairs_ReturnsMinimumKey()
            {
                dictionary.Add(2, "Value1");
                dictionary.Add(1, "Value2");
                dictionary.Add(3, "Value3");


                var minKey = dictionary.MinKey();


                Assert.AreEqual(1, minKey);
            }

            [Test]
            public void DictionaryWithMultipleKeyValuePair_KeysAreAllSameType()
            {
                dictionary.Add(2, "Value1");
                dictionary.Add(1, "Value2");
                dictionary.Add(3, "Value3");


                var keys = dictionary.Keys;
                var keyType = keys.First().GetType();


                Assert.That(keys, Has.All.TypeOf(keyType));
            }
        }

        [TestFixture]
        public class MinTests
        {
            [Test]
            public void Min_ReturnsMinValue()
            {
                var dictionary = new FastPersistentDictionary<int, int>();
                dictionary.Add(1, 10);
                dictionary.Add(2, 20);
                dictionary.Add(3, 30);


                var min = dictionary.Min();


                Assert.AreEqual(10, min);
            }

            [Test]
            public void Min_EmptyDictionary_ThrowsInvalidOperationException()
            {
                var dictionary = new FastPersistentDictionary<int, int>();


                Assert.Throws<InvalidOperationException>(() => dictionary.Min());
            }
        }

        [TestFixture]
        public class SingleOrDefaultTests
        {
            [SetUp]
            public void SetUp()
            {
                _dictionary = new FastPersistentDictionary<int, string>();
            }

            [TearDown]
            public void TearDown()
            {
                _dictionary.Dispose();
                _dictionary = null;
            }

            private FastPersistentDictionary<int, string> _dictionary;

            [Test]
            public void SingleOrDefault_WithEmptyDictionary_ReturnsNull()
            {
                var result = _dictionary.SingleOrDefault();

                var resultexpected = new KeyValuePair<int, string>(0, null);

                Assert.That(result.Value == resultexpected.Value && result.Key == resultexpected.Key);
            }

            [Test]
            public void SingleOrDefault_WithNonEmptyDictionary_ReturnsKeyValuePair()
            {
                _dictionary.Add(1, "One");


                var result = _dictionary.SingleOrDefault();


                Assert.AreEqual(new KeyValuePair<int, string>(1, "One"), result);
            }

            [Test]
            public void SingleOrDefault_WithPredicate_WithEmptyDictionary_ReturnsNull()
            {
                var result = _dictionary.SingleOrDefault(kvp => kvp.Key == 1);


                var resultexpected = new KeyValuePair<int, string>(0, null);

                Assert.That(result.Value == resultexpected.Value && result.Key == resultexpected.Key);
            }

            [Test]
            public void SingleOrDefault_WithPredicate_WithNonEmptyDictionary_ReturnsMatchingKeyValuePair()
            {
                _dictionary.Add(1, "One");
                _dictionary.Add(2, "Two");
                _dictionary.Add(3, "Three");


                var result = _dictionary.SingleOrDefault(kvp => kvp.Key == 2);


                Assert.AreEqual(new KeyValuePair<int, string>(2, "Two"), result);
            }

            [Test]
            public void SingleOrDefault_WithPredicate_WithNonEmptyDictionary_ReturnsNullIfNoMatch()
            {
                _dictionary.Add(1, "One");
                _dictionary.Add(2, "Two");
                _dictionary.Add(3, "Three");


                var result = _dictionary.SingleOrDefault(kvp => kvp.Key == 4);

                var resultexpected = new KeyValuePair<int, string>(0, null);

                Assert.That(result.Value == resultexpected.Value && result.Key == resultexpected.Key);
            }
        }

        [TestFixture]
        public class SkipLastTests
        {
            [Test]
            public void SkipLast_SkipsLastNKeyValuePairs()
            {
                var dictionary = CreateDictionaryWithValues();
                var number = 2;


                var result = dictionary.SkipLast(number);


                Assert.AreEqual(dictionary.Count - number, result.Count());

                var expected = dictionary.Take(dictionary.Count - number);
                CollectionAssert.AreEqual(expected, result);
            }

            [Test]
            public void SkipLast_WhenNumberIsZero_ReturnsOriginalDictionary()
            {
                var dictionary = CreateDictionaryWithValues();
                var number = 0;


                var result = dictionary.SkipLast(number);


                Assert.AreEqual(dictionary.Count, result.Count());
                CollectionAssert.AreEqual(dictionary, result);
            }


            [Test]
            public void SkipLast_WhenNumberIsGreaterThanOrEqualToDictionaryCount_ReturnsEmptyDictionary()
            {
                var dictionary = CreateDictionaryWithValues();
                var number = dictionary.Count;


                var result = dictionary.SkipLast(number);


                Assert.AreEqual(0, result.Count());
                CollectionAssert.IsEmpty(result);
            }

            private FastPersistentDictionary<int, string> CreateDictionaryWithValues()
            {
                var dictionary = new FastPersistentDictionary<int, string>();

                dictionary.Add(1, "One");
                dictionary.Add(2, "Two");
                dictionary.Add(3, "Three");
                dictionary.Add(4, "Four");
                dictionary.Add(5, "Five");

                return dictionary;
            }
        }

        [TestFixture]
        public class SkipWhileTests
        {
            [TestFixture]
            public class WhenPredicateIsTrueForAllElements
            {
                [Test]
                public void ReturnsEmptyCollection()
                {
                    var dictionary = new FastPersistentDictionary<int, string>();
                    dictionary.Add(1, "one");
                    dictionary.Add(2, "two");
                    dictionary.Add(3, "three");


                    var result = dictionary.SkipWhile((key, value) => true);


                    Assert.IsEmpty(result);
                }
            }

            [TestFixture]
            public class WhenPredicateIsFalseForFirstElement
            {
                [TestFixture]
                public class AndPredicateIsTrueForRestOfTheElements
                {
                    [Test]
                    public void SkipsFirstElementAndReturnsRestOfTheCollection()
                    {
                        var dictionary = new FastPersistentDictionary<int, string>();
                        dictionary.Add(1, "one");
                        dictionary.Add(2, "two");
                        dictionary.Add(3, "three");


                        var result = dictionary.SkipWhile(item => item.Key == 1);


                        Assert.AreEqual(2, result.Count());
                        Assert.AreEqual("two", result.First().Value);
                        Assert.AreEqual("three", result.Last().Value);
                    }
                }
            }

            [TestFixture]
            public class WhenPredicateIsFalseForAllElements
            {
                [Test]
                public void ReturnsWholeCollection()
                {
                    var dictionary = new FastPersistentDictionary<int, string>();
                    dictionary.Add(1, "one");
                    dictionary.Add(2, "two");
                    dictionary.Add(3, "three");


                    var result = dictionary.SkipWhile((key, value) => false);


                    Assert.AreEqual(3, result.Count());
                    Assert.AreEqual("one", result.First().Value);
                    Assert.AreEqual("three", result.Last().Value);
                }
            }
        }


        [TestFixture]
        public class SkipTests
        {
            [Test]
            public void Skip_Zero_ReturnsAllKeyValuePairs()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                dictionary.Add(1, "One");
                dictionary.Add(2, "Two");
                dictionary.Add(3, "Three");


                var result = dictionary.Skip(0);


                CollectionAssert.AreEquivalent(dictionary, result);
            }

            [Test]
            public void Skip_Positive_ReturnsCorrectKeyValuePairs()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                dictionary.Add(1, "One");
                dictionary.Add(2, "Two");
                dictionary.Add(3, "Three");


                var result = dictionary.Skip(1);


                Assert.AreEqual(2, result.Count());
                Assert.IsFalse(result.Any(item => item.Key == 1));
                Assert.IsTrue(result.Any(item => item.Key == 2));
                Assert.IsTrue(result.Any(item => item.Key == 3));
                Assert.AreEqual("Two", result.ElementAt(0).Value);
                Assert.AreEqual("Three", result.ElementAt(1).Value);
            }

            [Test]
            public void Skip_GreaterThanCount_ReturnsEmptyDictionary()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                dictionary.Add(1, "One");
                dictionary.Add(2, "Two");
                dictionary.Add(3, "Three");


                var result = dictionary.Skip(4);


                Assert.AreEqual(0, result.Count());
            }

            [Test]
            public void Skip_ZeroWithPredicate_ReturnsAllKeyValuePairs()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                dictionary.Add(1, "One");
                dictionary.Add(2, "Two");
                dictionary.Add(3, "Three");


                var result = dictionary.Skip(0).Where(key => key.Key >= 0);


                CollectionAssert.AreEquivalent(dictionary, result);
            }

            [Test]
            public void Skip_PositiveWithPredicate_ReturnsCorrectKeyValuePairs()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                dictionary.Add(1, "One");
                dictionary.Add(2, "Two");
                dictionary.Add(3, "Three");


                var result = dictionary.Skip(1).Where(key => key.Key >= 1);


                Assert.AreEqual(2, result.Count());
                Assert.IsFalse(result.Any(item => item.Key == 1));
                Assert.IsTrue(result.Any(item => item.Key == 2));
                Assert.IsTrue(result.Any(item => item.Key == 3));
                Assert.AreEqual("Two", result.ElementAt(0).Value);
                Assert.AreEqual("Three", result.ElementAt(1).Value);
            }

            [Test]
            public void Skip_GreaterThanCountWithPredicate_ReturnsEmptyDictionary()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                dictionary.Add(1, "One");
                dictionary.Add(2, "Two");
                dictionary.Add(3, "Three");


                var result = dictionary.Skip(4).Where(key => key.Key >= 1);


                Assert.AreEqual(0, result.Count());
            }
        }

        [TestFixture]
        public class SingleTests
        {
            [TestFixture]
            public class WhenDictionaryIsEmpty
            {
                [SetUp]
                public void SetUp()
                {
                    _dictionary = new FastPersistentDictionary<int, string>();
                }

                private FastPersistentDictionary<int, string> _dictionary;

                [Test]
                public void ThrowsInvalidOperationException()
                {
                    Assert.Throws<InvalidOperationException>(() => { _dictionary.Single(); });
                }

                [Test]
                public void PredicateThrowsInvalidOperationException()
                {
                    Assert.Throws<InvalidOperationException>(() => { _dictionary.Single(key => true); });
                }

                [Test]
                public void WithDefault_ReturnsDefaultValue()
                {
                    var defaultValue = new KeyValuePair<int, string>(0, null);

                    var singleValue = _dictionary.SingleOrDefault(item =>
                        item.Value == defaultValue.Value && item.Key == defaultValue.Key);

                    Assert.AreEqual(defaultValue, singleValue);
                }
            }

            [TestFixture]
            public class WhenDictionaryHasSingleElement
            {
                [SetUp]
                public void SetUp()
                {
                    _dictionary = new FastPersistentDictionary<int, string>();
                    _expectedKeyValuePair = new KeyValuePair<int, string>(1, "Value 1");

                    _dictionary.Add(_expectedKeyValuePair.Key, _expectedKeyValuePair.Value);
                }

                private FastPersistentDictionary<int, string> _dictionary;
                private KeyValuePair<int, string> _expectedKeyValuePair;

                [Test]
                public void ReturnsSingleKeyValuePair()
                {
                    var singleKeyValuePair = _dictionary.Single();

                    Assert.AreEqual(_expectedKeyValuePair, singleKeyValuePair);
                }

                [Test]
                public void PredicateReturnsTrue_ReturnsSingleKeyValuePair()
                {
                    var singleKeyValuePair = _dictionary.Single(item => item.Key == _expectedKeyValuePair.Key);

                    Assert.AreEqual(_expectedKeyValuePair, singleKeyValuePair);
                }

                [Test]
                public void PredicateReturnsFalse_ThrowsInvalidOperationException()
                {
                    Assert.Throws<InvalidOperationException>(() =>
                    {
                        _dictionary.Single(item => item.Key != _expectedKeyValuePair.Key);
                    });
                }
            }

            [TestFixture]
            public class WhenDictionaryHasMultipleElements
            {
                [SetUp]
                public void SetUp()
                {
                    _dictionary = new FastPersistentDictionary<int, string>();

                    _dictionary.Add(1, "Value 1");
                    _dictionary.Add(2, "Value 2");
                    _dictionary.Add(3, "Value 3");
                }

                private FastPersistentDictionary<int, string> _dictionary;

                [Test]
                public void ThrowsInvalidOperationException()
                {
                    Assert.Throws<InvalidOperationException>(() => { _dictionary.Single(); });
                }

                [Test]
                public void WithPredicateReturnsSingleKeyValuePair()
                {
                    var singleKeyValuePair =
                        _dictionary.Single(item => item.Key == 2 && item.Value == "Value 2");

                    var expectedKeyValuePair = new KeyValuePair<int, string>(2, "Value 2");

                    Assert.AreEqual(expectedKeyValuePair, singleKeyValuePair);
                }

                [Test]
                public void WithPredicateReturnsMultipleKeyValuePairs_ThrowsInvalidOperationException()
                {
                    Assert.Throws<InvalidOperationException>(() => { _dictionary.Single(item => item.Key != 1); });
                }
            }
        }

        [TestFixture]
        public class SumTests
        {
            [Test]
            public void Sum_EmptyDictionary_ReturnsDefault()
            {
                var dictionary = new FastPersistentDictionary<int, int>();


                var result = dictionary.Sum();


                Assert.AreEqual(default(int), result);
            }

            [Test]
            public void Sum_DictionaryWithValues_ReturnsSumOfValues()
            {
                var dictionary = new FastPersistentDictionary<int, int>();
                dictionary.Add(1, 10);
                dictionary.Add(2, 20);
                dictionary.Add(3, 30);


                var result = dictionary.Sum();


                Assert.AreEqual(60, result);
            }

            [Test]
            public void Sum_DictionaryWithNegativeValues_ReturnsSumOfValues()
            {
                var dictionary = new FastPersistentDictionary<int, int>();
                dictionary.Add(1, -10);
                dictionary.Add(2, -20);
                dictionary.Add(3, -30);


                var result = dictionary.Sum();


                Assert.AreEqual(-60, result);
            }

            [Test]
            public void Sum_DictionaryWithDecimalValues_ReturnsSumOfValues()
            {
                var dictionary = new FastPersistentDictionary<int, decimal>();
                dictionary.Add(1, 1.5m);
                dictionary.Add(2, 2.5m);
                dictionary.Add(3, 3.5m);


                var result = dictionary.Sum();


                Assert.AreEqual(7.5m, result);
            }


            [Test]
            public void Sum_DictionaryWithNullValues_ThrowsNullReferenceException()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                dictionary.Add(1, "10");
                dictionary.Add(2, null);
                dictionary.Add(3, "30");


                Assert.Throws<InvalidOperationException>(() => dictionary.Sum());
            }
        }


        [TestFixture]
        public class GetRandomTests
        {
            [TestFixture]
            public class WhenDictionaryIsEmpty
            {
                [SetUp]
                public void Setup()
                {
                    dictionary = new FastPersistentDictionary<int, string>();
                }

                private FastPersistentDictionary<int, string> dictionary;

                [Test]
                public void ReturnsInvalidOperationException()
                {
                    Assert.Throws<InvalidOperationException>(() => dictionary.GetRandom());
                }
            }

            [TestFixture]
            public class WhenDictionaryHasOneElement
            {
                [SetUp]
                public void Setup()
                {
                    dictionary = new FastPersistentDictionary<int, string>();
                    dictionary.Add(1, "One");
                }

                private FastPersistentDictionary<int, string> dictionary;

                [Test]
                public void ReturnsTheOnlyKeyValuePair()
                {
                    var result = dictionary.GetRandom();


                    Assert.AreEqual(1, result.Key);
                    Assert.AreEqual("One", result.Value);
                }
            }

            [TestFixture]
            public class WhenDictionaryHasMultipleElements
            {
                [SetUp]
                public void Setup()
                {
                    dictionary = new FastPersistentDictionary<int, string>();
                    dictionary.Add(1, "One");
                    dictionary.Add(2, "Two");
                    dictionary.Add(3, "Three");
                }

                private FastPersistentDictionary<int, string> dictionary;

                [Test]
                public void ReturnsRandomKeyValuePairFromDictionary()
                {
                    var result = dictionary.GetRandom();


                    Assert.IsTrue(dictionary.ContainsKey(result.Key));
                    Assert.AreEqual(dictionary[result.Key], result.Value);
                }
            }
        }

        [TestFixture]
        public class ToArrayTests
        {
            [Test]
            public void ToArray_ConvertsDictionaryToKeyValuePairArray()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                dictionary.Add(1, "one");
                dictionary.Add(2, "two");
                dictionary.Add(3, "three");


                var array = dictionary.ToArray();


                Assert.IsInstanceOf<KeyValuePair<int, string>[]>(array);
                Assert.AreEqual(3, array.Length);
            }
        }

        [TestFixture]
        public class ToListTests
        {
            [Test]
            public void ToList_ConvertsDictionaryToListOfKeyValuePairs()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                dictionary.Add(1, "one");
                dictionary.Add(2, "two");
                dictionary.Add(3, "three");


                var list = dictionary.ToList();


                Assert.IsInstanceOf<List<KeyValuePair<int, string>>>(list);
                Assert.AreEqual(3, list.Count);
            }
        }

        [TestFixture]
        public class RenameKeyTests
        {
            [TestFixture]
            public class WhenDictionaryIsEmpty
            {
                [Test]
                public void ShouldThrowInvalidOperationException()
                {
                    var dictionary = new FastPersistentDictionary<int, string>();


                    Assert.Throws<KeyNotFoundException>(() => dictionary.RenameKey(1, 2));
                }
            }

            [TestFixture]
            public class WhenOldKeyDoesNotExist
            {
                [Test]
                public void ShouldThrowKeyNotFoundException()
                {
                    var dictionary = new FastPersistentDictionary<int, string>();
                    dictionary.Add(1, "Value1");
                    dictionary.Add(3, "Value3");


                    Assert.Throws<KeyNotFoundException>(() => dictionary.RenameKey(2, 4));
                }
            }

            [TestFixture]
            public class WhenNewKeyAlreadyExists
            {
                [Test]
                public void ShouldThrowArgumentException()
                {
                    var dictionary = new FastPersistentDictionary<int, string>();
                    dictionary.Add(1, "Value1");
                    dictionary.Add(2, "Value2");
                    dictionary.Add(3, "Value3");
                    dictionary.Add(4, "Value4");

                    dictionary.RenameKey(3, 2);

                    Assert.That(dictionary[2] == "Value3");
                }
            }

            [TestFixture]
            public class WhenOldKeyExistsAndNewKeyDoesNotExist
            {
                [Test]
                public void ShouldRenameKey()
                {
                    var dictionary = new FastPersistentDictionary<int, string>();
                    dictionary.Add(1, "Value1");
                    dictionary.Add(2, "Value2");
                    dictionary.Add(3, "Value3");
                    dictionary.Add(4, "Value4");


                    dictionary.RenameKey(3, 5);


                    Assert.IsFalse(dictionary.ContainsKey(3));
                    Assert.IsTrue(dictionary.ContainsKey(5));
                    Assert.AreEqual("Value3", dictionary[5]);
                    Assert.AreEqual(4, dictionary.Count);
                }
            }

            [TestFixture]
            public class WhenOldKeyExistsAndNewKeyExistsButOverwriteExistingKeysTrue
            {
                [Test]
                public void ShouldRenameKeyAndOverwriteExistingKey()
                {
                    var dictionary = new FastPersistentDictionary<int, string>();
                    dictionary.Add(1, "Value1");
                    dictionary.Add(2, "Value2");
                    dictionary.Add(3, "Value3");
                    dictionary.Add(4, "Value4");


                    dictionary.RenameKey(3, 2);


                    Assert.IsFalse(dictionary.ContainsKey(3));
                    Assert.IsTrue(dictionary.ContainsKey(2));
                    Assert.AreEqual("Value3", dictionary[2]);
                    Assert.AreEqual(3, dictionary.Count);
                }
            }
        }

        [TestFixture]
        public class GetTests
        {
            [SetUp]
            public void Setup()
            {
                _dictionary = new FastPersistentDictionary<int, string>();
            }

            private FastPersistentDictionary<int, string> _dictionary;

            [Test]
            public void Get_ReturnsValueIfExistsInDictionary()
            {
                _dictionary.Add(1, "Value1");


                var result = _dictionary.Get(1);


                Assert.AreEqual("Value1", result);
            }

            [Test]
            public void Get_ReturnsDefaultValueIfKeyDoesNotExist()
            {
                Assert.Throws<KeyNotFoundException>(() => _dictionary.Get(1));
            }


            [Test]
            public void Get_ReturnsValueFromDictionaryWithCustomEqualityComparer()
            {
                _dictionary =
                    new FastPersistentDictionary<int, string>(equalityComparer: new CustomEqualityComparer());

                _dictionary.Add(1, "Value1");


                var result = _dictionary.Get(1);


                Assert.AreEqual("Value1", result);
            }


            [Test]
            public void Get_ReturnsDefaultValueIfKeyDoesNotExistWithCustomEqualityComparer()
            {
                _dictionary =
                    new FastPersistentDictionary<int, string>(equalityComparer: new CustomEqualityComparer());


                Assert.Throws<KeyNotFoundException>(() => _dictionary.Get(1));
            }

            private class CustomEqualityComparer : IEqualityComparer<int>
            {
                public bool Equals(int x, int y)
                {
                    return x.ToString() == y.ToString();
                }

                public int GetHashCode(int obj)
                {
                    return obj.GetHashCode();
                }
            }
        }

        [TestFixture]
        public class GetBulkTests
        {
            [Test]
            public void ReturnsValuesForSpecifiedKeys()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                dictionary.Add(1, "one");
                dictionary.Add(2, "two");
                dictionary.Add(3, "three");
                dictionary.Add(4, "four");


                var result = dictionary.GetBulk(new[] { 1, 3 });


                Assert.IsNotNull(result);
                Assert.AreEqual(2, result.Length);
                Assert.AreEqual("one", result[0]);
                Assert.AreEqual("three", result[1]);
            }

            [Test]
            public void ReturnsEmptyArrayForEmptyKeys()
            {
                var dictionary = new FastPersistentDictionary<int, string>();


                var result = dictionary.GetBulk(new int[] { });


                Assert.IsNotNull(result);
                Assert.AreEqual(0, result.Length);
            }

            [Test]
            public void ReturnsNullValuesForNonExistingKeys()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                dictionary.Add(1, "one");
                dictionary.Add(2, "two");


                Assert.Throws<KeyNotFoundException>(() => dictionary.GetBulk(new[] { 1, 3 }));
            }

            [Test]
            public void ReturnsEmptyArrayForNonExistingKeys()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                dictionary.Add(1, "one");
                dictionary.Add(2, "two");


                Assert.Throws<KeyNotFoundException>(() => dictionary.GetBulk(new[] { 3, 4 }));
            }
        }

        [TestFixture]
        public class ReverseTests
        {
            [Test]
            public void Reverse_ReturnsReversedDictionary()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                dictionary.Add(1, "One");
                dictionary.Add(2, "Two");
                dictionary.Add(3, "Three");


                var expected = new List<KeyValuePair<int, string>>();
                expected.Add(new KeyValuePair<int, string>(3, "Three"));
                expected.Add(new KeyValuePair<int, string>(2, "Two"));
                expected.Add(new KeyValuePair<int, string>(1, "One"));


                var reversedDictionary = dictionary.Reverse();


                CollectionAssert.AreEqual(expected, reversedDictionary.ToList());
            }

            [Test]
            public void Reverse_EmptyDictionary_ReturnsEmptyDictionary()
            {
                var dictionary = new FastPersistentDictionary<int, string>();


                var reversedDictionary = dictionary.Reverse();


                Assert.IsEmpty(reversedDictionary);
            }
        }

        [TestFixture]
        public class AsParallelTests
        {
            [Test]
            public void AsParallel_EnumeratesDictionaryInParallel()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                dictionary.Add(1, "one");
                dictionary.Add(2, "two");
                dictionary.Add(3, "three");


                var values = dictionary.AsParallel()
                    .Select(kvp => kvp.Value)
                    .ToList();


                Assert.Contains("one", values);
                Assert.Contains("two", values);
                Assert.Contains("three", values);
            }
        }

        [TestFixture]
        public class ShuffleTests
        {
            [Test]
            public void ShuffledDictionaryHasSameCount()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                dictionary.Add(1, "One");
                dictionary.Add(2, "Two");
                dictionary.Add(3, "Three");


                dictionary.Shuffle();


                Assert.AreEqual(3, dictionary.Count);
            }

            [Test]
            public void ShuffledDictionaryHasSameKeys()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                dictionary.Add(1, "One");
                dictionary.Add(2, "Two");
                dictionary.Add(3, "Three");


                dictionary.Shuffle();


                CollectionAssert.AreEquivalent(new List<int> { 1, 2, 3 }, dictionary.Keys);
            }

            [Test]
            public void ShuffledDictionaryHasSameValues()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                dictionary.Add(1, "One");
                dictionary.Add(2, "Two");
                dictionary.Add(3, "Three");


                dictionary.Shuffle();


                CollectionAssert.AreEquivalent(new List<string> { "One", "Two", "Three" }, dictionary.Values);
            }

            [Test]
            public void ShuffledDictionaryHasSameKeyValuePairs()
            {
                var dictionary = new FastPersistentDictionary<string, int>();
                dictionary.Add("One", 1);
                dictionary.Add("Two", 2);
                dictionary.Add("Three", 3);


                dictionary.Shuffle();


                Assert.IsTrue(dictionary.Contains(new KeyValuePair<string, int>("One", 1)));
                Assert.IsTrue(dictionary.Contains(new KeyValuePair<string, int>("Two", 2)));
                Assert.IsTrue(dictionary.Contains(new KeyValuePair<string, int>("Three", 3)));
            }


            [Test]
            public void ShuffledDictionaryContainsSameKeyValuePairs()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                dictionary.Add(1, "One");
                dictionary.Add(2, "Two");
                dictionary.Add(3, "Three");


                dictionary.Shuffle();


                CollectionAssert.AreEquivalent(new List<KeyValuePair<int, string>>
                {
                    new KeyValuePair<int, string>(1, "One"),
                    new KeyValuePair<int, string>(2, "Two"),
                    new KeyValuePair<int, string>(3, "Three")
                }, dictionary.ToList());
            }
        }

        [TestFixture]
        public class AverageTests
        {
            [TestFixture]
            public class WhenDictionaryIsEmpty
            {
                [Test]
                public void Average_ReturnsZero()
                {
                    var dictionary = new FastPersistentDictionary<int, int>();


                    Assert.Throws<InvalidOperationException>(() => dictionary.Average(kv => kv.Value));
                }
            }

            [Test]
            public void Average_CalculatesAverageOfValues()
            {
                var dictionary = new FastPersistentDictionary<int, int>();
                dictionary.Add(1, 10);
                dictionary.Add(2, 20);
                dictionary.Add(3, 30);


                var average = dictionary.Average(kvp => kvp.Value);


                Assert.AreEqual(20, average);
            }


            [TestFixture]
            public class WhenDictionaryContainsOnlyNegativeValues
            {
                [SetUp]
                public void SetUp()
                {
                    dictionary = new FastPersistentDictionary<int, int>();
                    dictionary.Add(1, -5);
                    dictionary.Add(2, -10);
                    dictionary.Add(3, -15);
                }

                private FastPersistentDictionary<int, int> dictionary;

                [Test]
                public void Average_ReturnsCorrectAverage()
                {
                    var result = dictionary.Average(kv => kv.Value);


                    Assert.AreEqual(-10, result);
                }
            }

            [TestFixture]
            public class WhenDictionaryContainsOnlyPositiveValues
            {
                [SetUp]
                public void SetUp()
                {
                    dictionary = new FastPersistentDictionary<int, int>();
                    dictionary.Add(1, 5);
                    dictionary.Add(2, 10);
                    dictionary.Add(3, 15);
                }

                private FastPersistentDictionary<int, int> dictionary;

                [Test]
                public void Average_ReturnsCorrectAverage()
                {
                    var result = dictionary.Average(kv => kv.Value);


                    Assert.AreEqual(10, result);
                }
            }

            [TestFixture]
            public class WhenDictionaryContainsPositiveAndNegativeValues
            {
                [SetUp]
                public void SetUp()
                {
                    dictionary = new FastPersistentDictionary<int, int>();
                    dictionary.Add(1, 5);
                    dictionary.Add(2, -10);
                    dictionary.Add(3, 15);
                }

                private FastPersistentDictionary<int, int> dictionary;

                [Test]
                public void Average_ReturnsCorrectAverage()
                {
                    var result = dictionary.Average(kv => kv.Value);


                    Assert.AreEqual(3.3333333333333335, result, 0.0000001);
                }
            }

            [TestFixture]
            public class WhenDictionaryContainsDecimalValues
            {
                [SetUp]
                public void SetUp()
                {
                    dictionary = new FastPersistentDictionary<int, decimal>();
                    dictionary.Add(1, 1.5m);
                    dictionary.Add(2, 2.5m);
                    dictionary.Add(3, 3.5m);
                }

                private FastPersistentDictionary<int, decimal> dictionary;

                [Test]
                public void Average_ReturnsCorrectAverage()
                {
                    var result = dictionary.Average(kv => kv.Value);


                    Assert.AreEqual(2.5m, result);
                }
            }

            [TestFixture]
            public class WhenDictionaryContainsNullValues
            {
                [SetUp]
                public void SetUp()
                {
                    dictionary = new FastPersistentDictionary<int, string>();
                    dictionary.Add(1, "one");
                    dictionary.Add(2, null);
                    dictionary.Add(3, "three");
                }

                private FastPersistentDictionary<int, string> dictionary;

                [Test]
                public void Average_ReturnsAverageOfNonNullValues()
                {
                    var result = dictionary.Average(kv => kv.Value?.Length ?? 0);


                    Assert.AreEqual(2.6666666666666665, result);
                }
            }

            [TestFixture]
            public class WhenDictionaryContainsOnlyOneElement
            {
                [SetUp]
                public void SetUp()
                {
                    dictionary = new FastPersistentDictionary<int, int>();
                    dictionary.Add(1, 5);
                }

                private FastPersistentDictionary<int, int> dictionary;

                [Test]
                public void Average_ReturnsValueOfOnlyElement()
                {
                    var result = dictionary.Average(kv => kv.Value);


                    Assert.AreEqual(5, result);
                }
            }
        }

        [TestFixture]
        public class OrderByTests
        {
            [TestFixture]
            public class WhenOrderByIsCalledWithSelector
            {
                [Test]
                public void OrdersDictionaryByAscendingResultOfSelector()
                {
                    var dictionary = new FastPersistentDictionary<int, string>();
                    dictionary.Add(1, "One");
                    dictionary.Add(4, "Four");
                    dictionary.Add(3, "Three");
                    dictionary.Add(2, "Two");


                    var orderedDictionary = dictionary.OrderBy(kv => kv.Value);


                    Assert.AreEqual("Four", orderedDictionary.First().Value);
                    Assert.AreEqual("One", orderedDictionary.ElementAt(1).Value);
                    Assert.AreEqual("Three", orderedDictionary.ElementAt(2).Value);
                    Assert.AreEqual("Two", orderedDictionary.Last().Value);
                }
            }

            [TestFixture]
            public class WhenOrderByIsCalledWithComparer
            {
                [Test]
                public void OrdersDictionaryByAscendingResultOfComparer()
                {
                    var dictionary = new FastPersistentDictionary<int, string>();
                    dictionary.Add(1, "One");
                    dictionary.Add(4, "Four");
                    dictionary.Add(3, "Three");
                    dictionary.Add(2, "Two");


                    var orderedDictionary = dictionary.OrderBy(kv => kv.Value, StringComparer.OrdinalIgnoreCase);


                    Assert.AreEqual("Four", orderedDictionary.First().Value);
                    Assert.AreEqual("One", orderedDictionary.ElementAt(1).Value);
                    Assert.AreEqual("Three", orderedDictionary.ElementAt(2).Value);
                    Assert.AreEqual("Two", orderedDictionary.Last().Value);
                }
            }

            [TestFixture]
            public class WhenOrderByDescendingIsCalledWithSelector
            {
                [Test]
                public void OrdersDictionaryByDescendingResultOfSelector()
                {
                    var dictionary = new FastPersistentDictionary<int, string>();
                    dictionary.Add(1, "One");
                    dictionary.Add(4, "Four");
                    dictionary.Add(3, "Three");
                    dictionary.Add(2, "Two");


                    var orderedDictionary = dictionary.OrderByDescending(kv => kv.Value);


                    Assert.AreEqual("Two", orderedDictionary.First().Value);
                    Assert.AreEqual("Three", orderedDictionary.ElementAt(1).Value);
                    Assert.AreEqual("One", orderedDictionary.ElementAt(2).Value);
                    Assert.AreEqual("Four", orderedDictionary.Last().Value);
                }
            }

            [TestFixture]
            public class WhenOrderByDescendingIsCalledWithComparer
            {
                [Test]
                public void OrdersDictionaryByDescendingResultOfComparer()
                {
                    var dictionary = new FastPersistentDictionary<int, string>();
                    dictionary.Add(1, "One");
                    dictionary.Add(4, "Four");
                    dictionary.Add(3, "Three");
                    dictionary.Add(2, "Two");


                    var orderedDictionary =
                        dictionary.OrderByDescending(kv => kv.Value, StringComparer.OrdinalIgnoreCase);


                    Assert.AreEqual("Two", orderedDictionary.First().Value);
                    Assert.AreEqual("Three", orderedDictionary.ElementAt(1).Value);
                    Assert.AreEqual("One", orderedDictionary.ElementAt(2).Value);
                    Assert.AreEqual("Four", orderedDictionary.Last().Value);
                }
            }
        }

        [TestFixture]
        public class OrderByDescendingTests
        {
            [SetUp]
            public void SetUp()
            {
                _dictionary = new FastPersistentDictionary<int, string>();
                _dictionary.Add(1, "One");
                _dictionary.Add(2, "Two");
                _dictionary.Add(3, "Three");
            }

            private FastPersistentDictionary<int, string> _dictionary;

            [Test]
            public void OrderByDescending_OrdersDictionaryByDescendingResultOfSelector()
            {
                var result = _dictionary.OrderByDescending(x => x.Key).ToList();


                Assert.AreEqual(3, result.Count);
                Assert.AreEqual(3, result[0].Key);
                Assert.AreEqual(2, result[1].Key);
                Assert.AreEqual(1, result[2].Key);
            }

            [Test]
            public void OrderByDescending_OrdersDictionaryByDescendingResultOfSelectorWithManyItems()
            {
                _dictionary.Add(4, "Four");
                _dictionary.Add(5, "Five");
                _dictionary.Add(6, "Six");


                var result = _dictionary.OrderByDescending(x => x.Key).ToList();


                Assert.AreEqual(6, result.Count);
                Assert.AreEqual(6, result[0].Key);
                Assert.AreEqual(5, result[1].Key);
                Assert.AreEqual(4, result[2].Key);
                Assert.AreEqual(3, result[3].Key);
                Assert.AreEqual(2, result[4].Key);
                Assert.AreEqual(1, result[5].Key);
            }

            [Test]
            public void OrderByDescending_ReturnsEmptyList_IfDictionaryIsEmpty()
            {
                _dictionary.Clear();


                var result = _dictionary.OrderByDescending(x => x.Key).ToList();


                Assert.IsEmpty(result);
            }
        }

        [TestFixture]
        public class AsQueryableTests
        {
            [Test]
            public void AsQueryable_ReturnsQueryableDictionary()
            {
                var dictionary = new FastPersistentDictionary<int, string>();


                var queryableDictionary = dictionary.AsQueryable();


                Assert.IsInstanceOf<IQueryable<KeyValuePair<int, string>>>(queryableDictionary);
            }

            [Test]
            public void AsQueryable_ReturnsDictionaryWithSameItems()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                dictionary.Add(1, "One");
                dictionary.Add(2, "Two");
                dictionary.Add(3, "Three");


                var queryableDictionary = dictionary.AsQueryable();
                var items = queryableDictionary.ToList();


                Assert.AreEqual(3, items.Count);
                Assert.Contains(new KeyValuePair<int, string>(1, "One"), items);
                Assert.Contains(new KeyValuePair<int, string>(2, "Two"), items);
                Assert.Contains(new KeyValuePair<int, string>(3, "Three"), items);
            }

            [Test]
            public void AsQueryable_CanQueryQueryableDictionary()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                dictionary.Add(1, "One");
                dictionary.Add(2, "Two");
                var queryableDictionary = dictionary.AsQueryable();


                var result = queryableDictionary.Where(kv => kv.Value.StartsWith("T"));


                Assert.AreEqual(1, result.Count());
                Assert.AreEqual(2, result.First().Key);
                Assert.AreEqual("Two", result.First().Value);
            }
        }

        [TestFixture]
        public class JoinTests
        {
            [TestFixture]
            public class WhenJoiningWithRegularDictionary
            {
                [Test]
                public void JoinsKeyValuePairsFromOtherDictionary()
                {
                    var dictionary1 = new FastPersistentDictionary<int, string>();
                    dictionary1.Add(1, "Value1");
                    dictionary1.Add(2, "Value2");

                    var dictionary2 = new Dictionary<int, string>();
                    dictionary2.Add(3, "Value3");
                    dictionary2.Add(4, "Value4");


                    dictionary1.Join(dictionary2);


                    Assert.IsTrue(dictionary1.ContainsKey(3));
                    Assert.IsTrue(dictionary1.ContainsKey(4));
                    Assert.AreEqual("Value3", dictionary1[3]);
                    Assert.AreEqual("Value4", dictionary1[4]);
                }

                [Test]
                public void OverwritesExistingKeysIfSpecified()
                {
                    var dictionary1 = new FastPersistentDictionary<int, string>();
                    dictionary1.Add(1, "Value1");
                    dictionary1.Add(2, "Value2");

                    var dictionary2 = new Dictionary<int, string>();
                    dictionary2.Add(2, "NewValue2");
                    dictionary2.Add(3, "Value3");


                    dictionary1.Join(dictionary2, true);


                    Assert.IsTrue(dictionary1.ContainsKey(2));
                    Assert.AreEqual("NewValue2", dictionary1[2]);
                }

                [Test]
                public void DoesNotOverwriteExistingKeysByDefault()
                {
                    var dictionary1 = new FastPersistentDictionary<int, string>();
                    dictionary1.Add(1, "Value1");
                    dictionary1.Add(2, "Value2");

                    var dictionary2 = new Dictionary<int, string>();
                    dictionary2.Add(2, "NewValue2");
                    dictionary2.Add(3, "Value3");


                    dictionary1.Join(dictionary2);


                    Assert.IsTrue(dictionary1.ContainsKey(2));
                    Assert.AreEqual("Value2", dictionary1[2]);
                }
            }

            [TestFixture]
            public class WhenJoiningWithAnotherPersistentDictionaryPro
            {
                [Test]
                public void JoinsKeyValuePairsFromOtherPersistentDictionaryPro()
                {
                    var dictionary1 = new FastPersistentDictionary<int, string>();
                    dictionary1.Add(1, "Value1");
                    dictionary1.Add(2, "Value2");

                    var dictionary2 = new FastPersistentDictionary<int, string>();
                    dictionary2.Add(3, "Value3");
                    dictionary2.Add(4, "Value4");


                    dictionary1.Join(dictionary2);


                    Assert.IsTrue(dictionary1.ContainsKey(3));
                    Assert.IsTrue(dictionary1.ContainsKey(4));
                    Assert.AreEqual("Value3", dictionary1[3]);
                    Assert.AreEqual("Value4", dictionary1[4]);
                }

                [Test]
                public void OverwritesExistingKeysIfSpecified()
                {
                    var dictionary1 = new FastPersistentDictionary<int, string>();
                    dictionary1.Add(1, "Value1");
                    dictionary1.Add(2, "Value2");

                    var dictionary2 = new FastPersistentDictionary<int, string>();
                    dictionary2.Add(2, "NewValue2");
                    dictionary2.Add(3, "Value3");


                    dictionary1.Join(dictionary2, true);


                    Assert.IsTrue(dictionary1.ContainsKey(2));
                    Assert.AreEqual("NewValue2", dictionary1[2]);
                }

                [Test]
                public void DoesNotOverwriteExistingKeysByDefault()
                {
                    var dictionary1 = new FastPersistentDictionary<int, string>();
                    dictionary1.Add(1, "Value1");
                    dictionary1.Add(2, "Value2");

                    var dictionary2 = new FastPersistentDictionary<int, string>();
                    dictionary2.Add(2, "NewValue2");
                    dictionary2.Add(3, "Value3");


                    dictionary1.Join(dictionary2);


                    Assert.IsTrue(dictionary1.ContainsKey(2));
                    Assert.AreEqual("Value2", dictionary1[2]);
                }
            }
        }

        [TestFixture]
        public class ToSortedDictionaryTests
        {
            [Test]
            public void ToSortedDictionary_ConvertsDictionaryToSortedDictionary()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                dictionary.Add(3, "three");
                dictionary.Add(1, "one");
                dictionary.Add(2, "two");


                var sorted = dictionary.ToSortedDictionary();


                Assert.IsInstanceOf<SortedDictionary<int, string>>(sorted);
                Assert.AreEqual(1, sorted.Keys.First());
                Assert.AreEqual(3, sorted.Keys.Last());
            }
        }

        [TestFixture]
        public class ToImmutableListTests
        {
            [Test]
            public void ToImmutableList_ConvertsDictionaryToImmutableList()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                dictionary.Add(1, "one");
                dictionary.Add(2, "two");
                dictionary.Add(3, "three");


                var immutableList = dictionary.ToImmutableList();


                Assert.IsInstanceOf<ImmutableList<KeyValuePair<int, string>>>(immutableList);
                Assert.AreEqual(3, immutableList.Count);
            }
        }

        [TestFixture]
        public class ToImmutableArrayTests
        {
            [Test]
            public void ToImmutableArray_ConvertsDictionaryToImmutableArray()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                dictionary.Add(1, "one");
                dictionary.Add(2, "two");
                dictionary.Add(3, "three");


                var immutableArray = dictionary.ToImmutableArray();


                Assert.IsInstanceOf<ImmutableArray<KeyValuePair<int, string>>>(immutableArray);
                Assert.AreEqual(3, immutableArray.Length);
            }
        }

        [TestFixture]
        public class ToImmutableDictionaryTests
        {
            [Test]
            public void ToImmutableDictionary_ConvertsDictionaryToImmutableDictionary()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                dictionary.Add(1, "one");
                dictionary.Add(2, "two");
                dictionary.Add(3, "three");


                var immutableDictionary = dictionary.ToImmutableDictionary();


                Assert.IsInstanceOf<ImmutableDictionary<int, string>>(immutableDictionary);
                Assert.AreEqual(3, immutableDictionary.Count);
            }
        }

        [TestFixture]
        public class ToImmutableHashSetTests
        {
            [Test]
            public void ToImmutableHashSet_ConvertsDictionaryToImmutableHashSet()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                dictionary.Add(1, "one");
                dictionary.Add(2, "two");
                dictionary.Add(3, "three");


                var immutableHashSet = dictionary.ToImmutableHashSet();


                Assert.IsInstanceOf<ImmutableHashSet<KeyValuePair<int, string>>>(immutableHashSet);
                Assert.AreEqual(3, immutableHashSet.Count);
            }
        }

        [TestFixture]
        public class ToSortedSetTests
        {
            [Test]
            public void ToSortedSet_ConvertsDictionaryToSortedSet()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                dictionary.Add(3, "three");
                dictionary.Add(1, "one");
                dictionary.Add(2, "two");


                var sortedSet = dictionary.ToSortedSet();


                Assert.IsInstanceOf<SortedSet<KeyValuePair<int, string>>>(sortedSet);
                Assert.AreEqual(1, sortedSet.ElementAt(0).Key);
                Assert.AreEqual(3, sortedSet.ElementAt(2).Key);
            }
        }

        [TestFixture]
        public class GetSubsetTests
        {
            [TestFixture]
            public class WhenPredicateReturnsTrue
            {
                [SetUp]
                public void SetUp()
                {
                    dictionary = new FastPersistentDictionary<int, string>();
                    dictionary.Add(1, "One");
                    dictionary.Add(2, "Two");
                    dictionary.Add(3, "Three");
                    dictionary.Add(4, "Four");


                    subset = dictionary.GetSubset((key, value) => value.Length == 3);
                }

                private FastPersistentDictionary<int, string> dictionary;
                private FastPersistentDictionary<int, string> subset;

                [Test]
                public void ReturnsSubsetWithMatchingElements()
                {
                    Assert.AreEqual(2, subset.Count);
                    Assert.IsTrue(subset.ContainsKey(1));
                    Assert.IsTrue(subset.ContainsKey(2));
                    Assert.AreEqual("One", subset[1]);
                    Assert.AreEqual("Two", subset[2]);
                }

                [Test]
                public void OriginalDictionaryUnchanged()
                {
                    Assert.AreEqual(4, dictionary.Count);
                    Assert.IsTrue(dictionary.ContainsKey(1));
                    Assert.IsTrue(dictionary.ContainsKey(2));
                    Assert.IsTrue(dictionary.ContainsKey(3));
                    Assert.IsTrue(dictionary.ContainsKey(4));
                    Assert.AreEqual("One", dictionary[1]);
                    Assert.AreEqual("Two", dictionary[2]);
                    Assert.AreEqual("Three", dictionary[3]);
                    Assert.AreEqual("Four", dictionary[4]);
                }
            }

            [TestFixture]
            public class WhenPredicateReturnsFalse
            {
                [SetUp]
                public void SetUp()
                {
                    dictionary = new FastPersistentDictionary<int, string>();
                    dictionary.Add(1, "One");
                    dictionary.Add(2, "Two");
                    dictionary.Add(3, "Three");
                    dictionary.Add(4, "Four");


                    subset = dictionary.GetSubset((key, value) => value.Contains("X"));
                }

                private FastPersistentDictionary<int, string> dictionary;
                private FastPersistentDictionary<int, string> subset;

                [Test]
                public void ReturnsEmptySubset()
                {
                    Assert.AreEqual(0, subset.Count);
                    Assert.IsFalse(subset.ContainsKey(1));
                    Assert.IsFalse(subset.ContainsKey(2));
                    Assert.IsFalse(subset.ContainsKey(3));
                    Assert.IsFalse(subset.ContainsKey(4));
                }

                [Test]
                public void OriginalDictionaryUnchanged()
                {
                    Assert.AreEqual(4, dictionary.Count);
                    Assert.IsTrue(dictionary.ContainsKey(1));
                    Assert.IsTrue(dictionary.ContainsKey(2));
                    Assert.IsTrue(dictionary.ContainsKey(3));
                    Assert.IsTrue(dictionary.ContainsKey(4));
                    Assert.AreEqual("One", dictionary[1]);
                    Assert.AreEqual("Two", dictionary[2]);
                    Assert.AreEqual("Three", dictionary[3]);
                    Assert.AreEqual("Four", dictionary[4]);
                }
            }

            [TestFixture]
            public class WhenPredicateReturnsTrueForSomeElements
            {
                [SetUp]
                public void SetUp()
                {
                    dictionary = new FastPersistentDictionary<int, string>();
                    dictionary.Add(1, "One");
                    dictionary.Add(2, "Two");
                    dictionary.Add(3, "Three");
                    dictionary.Add(4, "Four");


                    subset = dictionary.GetSubset((key, value) => key % 2 == 0);
                }

                private FastPersistentDictionary<int, string> dictionary;
                private FastPersistentDictionary<int, string> subset;

                [Test]
                public void ReturnsSubsetWithMatchingElements()
                {
                    Assert.AreEqual(2, subset.Count);
                    Assert.IsTrue(subset.ContainsKey(2));
                    Assert.IsTrue(subset.ContainsKey(4));
                    Assert.AreEqual("Two", subset[2]);
                    Assert.AreEqual("Four", subset[4]);
                }

                [Test]
                public void OriginalDictionaryUnchanged()
                {
                    Assert.AreEqual(4, dictionary.Count);
                    Assert.IsTrue(dictionary.ContainsKey(1));
                    Assert.IsTrue(dictionary.ContainsKey(2));
                    Assert.IsTrue(dictionary.ContainsKey(3));
                    Assert.IsTrue(dictionary.ContainsKey(4));
                    Assert.AreEqual("One", dictionary[1]);
                    Assert.AreEqual("Two", dictionary[2]);
                    Assert.AreEqual("Three", dictionary[3]);
                    Assert.AreEqual("Four", dictionary[4]);
                }
            }

            [TestFixture]
            public class WhenPredicateReturnsFalseForAllElements
            {
                [SetUp]
                public void SetUp()
                {
                    dictionary = new FastPersistentDictionary<int, string>();
                    dictionary.Add(1, "One");
                    dictionary.Add(2, "Two");
                    dictionary.Add(3, "Three");
                    dictionary.Add(4, "Four");


                    subset = dictionary.GetSubset((key, value) => key > 5);
                }

                private FastPersistentDictionary<int, string> dictionary;
                private FastPersistentDictionary<int, string> subset;

                [Test]
                public void ReturnsEmptySubset()
                {
                    Assert.AreEqual(0, subset.Count);
                    Assert.IsFalse(subset.ContainsKey(1));
                    Assert.IsFalse(subset.ContainsKey(2));
                    Assert.IsFalse(subset.ContainsKey(3));
                    Assert.IsFalse(subset.ContainsKey(4));
                }

                [Test]
                public void OriginalDictionaryUnchanged()
                {
                    Assert.AreEqual(4, dictionary.Count);
                    Assert.IsTrue(dictionary.ContainsKey(1));
                    Assert.IsTrue(dictionary.ContainsKey(2));
                    Assert.IsTrue(dictionary.ContainsKey(3));
                    Assert.IsTrue(dictionary.ContainsKey(4));
                    Assert.AreEqual("One", dictionary[1]);
                    Assert.AreEqual("Two", dictionary[2]);
                    Assert.AreEqual("Three", dictionary[3]);
                    Assert.AreEqual("Four", dictionary[4]);
                }
            }

            [TestFixture]
            public class WhenPredicateIsNull
            {
                [SetUp]
                public void SetUp()
                {
                    dictionary = new FastPersistentDictionary<int, string>();
                    dictionary.Add(1, "One");
                    dictionary.Add(2, "Two");
                    dictionary.Add(3, "Three");
                    dictionary.Add(4, "Four");


                    subset = dictionary.GetSubset((key, value) => value.Length == 3);
                }

                private FastPersistentDictionary<int, string> dictionary;
                private FastPersistentDictionary<int, string> subset;

                [Test]
                public void OriginalDictionaryUnchanged()
                {
                    Assert.AreEqual(4, dictionary.Count);
                    Assert.IsTrue(dictionary.ContainsKey(1));
                    Assert.IsTrue(dictionary.ContainsKey(2));
                    Assert.IsTrue(dictionary.ContainsKey(3));
                    Assert.IsTrue(dictionary.ContainsKey(4));
                    Assert.AreEqual("One", dictionary[1]);
                    Assert.AreEqual("Two", dictionary[2]);
                    Assert.AreEqual("Three", dictionary[3]);
                    Assert.AreEqual("Four", dictionary[4]);
                }
            }

            [TestFixture]
            public class WhenDictionaryIsEmpty
            {
                [SetUp]
                public void SetUp()
                {
                    dictionary = new FastPersistentDictionary<int, string>();


                    subset = dictionary.GetSubset((key, value) => key > 5);
                }

                private FastPersistentDictionary<int, string> dictionary;
                private FastPersistentDictionary<int, string> subset;

                [Test]
                public void ReturnsEmptySubset()
                {
                    Assert.AreEqual(0, subset.Count);
                    Assert.IsFalse(subset.ContainsKey(1));
                    Assert.IsFalse(subset.ContainsKey(2));
                    Assert.IsFalse(subset.ContainsKey(3));
                    Assert.IsFalse(subset.ContainsKey(4));
                }

                [Test]
                public void OriginalDictionaryUnchanged()
                {
                    Assert.AreEqual(0, dictionary.Count);
                }
            }
        }

        [TestFixture]
        public class SwitchTests
        {
            [Test]
            public void Switch_SwapsValuesOfTwoKeys()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                dictionary.Add(1, "one");
                dictionary.Add(2, "two");


                dictionary.Switch(1, 2);


                Assert.AreEqual("two", dictionary[1]);
                Assert.AreEqual("one", dictionary[2]);
            }
        }

        //[TestFixture]
        //public class UnionTests
        //{
        //    [TestFixture]
        //    public class WhenOtherDictionaryIsEmpty
        //    {
        //        [Test]
        //        public void ReturnsOriginalDictionary()
        //        {
        //           
        //            var originalDictionary = new PersistentDictionaryPro<int, string>();
        //            var otherDictionary = new PersistentDictionaryPro<int, string>();

        //            
        //            var result = originalDictionary.Union(otherDictionary);

        //           
        //            CollectionAssert.AreEquivalent(originalDictionary, result);
        //        }
        //    }

        //    [TestFixture]
        //    public class WhenOriginalDictionaryIsEmpty
        //    {
        //        [TestFixture]
        //        public class AndOtherDictionaryHasValues
        //        {
        //            [Test]
        //            public void ReturnsOtherDictionary()
        //            {
        //               
        //                var originalDictionary = new PersistentDictionaryPro<int, string>();
        //                var otherDictionary = new PersistentDictionaryPro<int, string>();
        //                otherDictionary.Add(1, "One");
        //                otherDictionary.Add(2, "Two");
        //                otherDictionary.Add(3, "Three");

        //                
        //                var result = originalDictionary.Union(otherDictionary);

        //               
        //                CollectionAssert.AreEquivalent(otherDictionary, result);
        //            }
        //        }

        //        [TestFixture]
        //        public class AndOtherDictionaryIsEmpty
        //        {
        //            [Test]
        //            public void ReturnsOriginalDictionary()
        //            {
        //               
        //                var originalDictionary = new PersistentDictionaryPro<int, string>();
        //                var otherDictionary = new PersistentDictionaryPro<int, string>();

        //                
        //                var result = originalDictionary.Union(otherDictionary);

        //               
        //                CollectionAssert.AreEquivalent(originalDictionary, result);
        //            }
        //        }
        //    }

        //    [TestFixture]
        //    public class WhenBothDictionariesHaveCommonKeys
        //    {
        //        [TestFixture]
        //        public class WithOverwriteExistingKeysSetToFalse
        //        {
        //            [Test]
        //            public void CombinesTheDictionariesWithoutOverwritingExistingKeys()
        //            {
        //               
        //                var originalDictionary = new PersistentDictionaryPro<int, string>();
        //                originalDictionary.Add(1, "One");
        //                originalDictionary.Add(2, "Two");
        //                originalDictionary.Add(3, "Three");
        //                originalDictionary.Add(4, "Four");

        //                var otherDictionary = new PersistentDictionaryPro<int, string>();
        //                otherDictionary.Add(3, "NewThree");
        //                otherDictionary.Add(4, "NewFour");
        //                otherDictionary.Add(5, "Five");
        //                otherDictionary.Add(6, "Six");

        //                
        //                var result = originalDictionary.Union(otherDictionary);

        //               
        //                Assert.AreEqual(6, result.Count);
        //                Assert.AreEqual("One", result[1]);
        //                Assert.AreEqual("Two", result[2]);
        //                Assert.AreEqual("Three", result[3]);
        //                Assert.AreEqual("Four", result[4]);
        //                Assert.AreEqual("Five", result[5]);
        //                Assert.AreEqual("Six", result[6]);
        //            }
        //        }

        //    }

        //    [TestFixture]
        //    public class WhenBothDictionariesDoNotHaveCommonKeys
        //    {
        //        [Test]
        //        public void CombinesTheDictionariesWithoutDuplicates()
        //        {
        //           
        //            var originalDictionary = new PersistentDictionaryPro<int, string>();
        //            originalDictionary.Add(1, "One");
        //            originalDictionary.Add(2, "Two");
        //            originalDictionary.Add(3, "Three");
        //            originalDictionary.Add(4, "Four");

        //            var otherDictionary = new PersistentDictionaryPro<int, string>();
        //            otherDictionary.Add(5, "Five");
        //            otherDictionary.Add(6, "Six");
        //            otherDictionary.Add(7, "Seven");
        //            otherDictionary.Add(8, "Eight");

        //            
        //            var result = originalDictionary.Union(otherDictionary);

        //           
        //            Assert.AreEqual(8, result.Count);
        //            Assert.AreEqual("One", result[1]);
        //            Assert.AreEqual("Two", result[2]);
        //            Assert.AreEqual("Three", result[3]);
        //            Assert.AreEqual("Four", result[4]);
        //            Assert.AreEqual("Five", result[5]);
        //            Assert.AreEqual("Six", result[6]);
        //            Assert.AreEqual("Seven", result[7]);
        //            Assert.AreEqual("Eight", result[8]);
        //        }
        //    }
        //}


        [TestFixture]
        public class UpdateValueTests
        {
            [Test]
            public void UpdateValue_KeyExists_UpdateValue()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                dictionary.Add(1, "Value1");


                dictionary.UpdateValue(1, value => value + " Updated");


                Assert.AreEqual("Value1 Updated", dictionary[1]);
            }

            [Test]
            public void UpdateValue_KeyDoesNotExist_DoNothing()
            {
                var dictionary = new FastPersistentDictionary<int, string>();


                Assert.Throws<KeyNotFoundException>(() => dictionary.UpdateValue(1, value => value + " Updated"));
            }

            [Test]
            public void UpdateValue_ConcurrentUpdate_UpdateValue()

            {
                var dictionary = new FastPersistentDictionary<int, int>();
                dictionary[1] = 1;


                Parallel.For(0, 1000, i => { dictionary.UpdateValue(1, value => value + 1); });


                Assert.AreEqual(1001, dictionary[1]);
            }

            [Test]
            public void UpdateValue_CollectionValue_UpdateAllValues()
            {
                var dictionary = new FastPersistentDictionary<int, List<string>>();
                dictionary.Add(1, new List<string> { "Value1" });


                dictionary.UpdateValue(1, values =>
                {
                    values.Add("Updated");
                    return values;
                });


                Assert.AreEqual(2, dictionary[1].Count);
                Assert.Contains("Value1", dictionary[1]);
                Assert.Contains("Updated", dictionary[1]);
            }
        }


        [TestFixture]
        public class CopyTests
        {
            [Test]
            public void Copy_CreatesNewInstanceOfPersistentDictionaryPro()
            {
                var originalDictionary = new FastPersistentDictionary<int, string>();
                originalDictionary.Add(1, "one");
                originalDictionary.Add(2, "two");


                var copiedDictionary = originalDictionary.Copy("path/to/copy.pcd");


                Assert.IsInstanceOf<FastPersistentDictionary<int, string>>(copiedDictionary);
                Assert.AreNotSame(originalDictionary, copiedDictionary);
                Assert.AreEqual(originalDictionary.Count, copiedDictionary.Count);
                Assert.AreEqual(originalDictionary[1], copiedDictionary[1]);
                Assert.AreEqual(originalDictionary[2], copiedDictionary[2]);
            }

            [Test]
            public void Copy_CreatesNewInstanceWithDifferentFilePath()
            {
                var originalDictionary = new FastPersistentDictionary<int, string>();
                originalDictionary.Add(1, "one");


                var copiedDictionary = originalDictionary.Copy("path/to/copy.pcd");


                Assert.AreEqual("path/to/copy.pcd", copiedDictionary.FileLocation);
            }

            [Test]
            public void Copy_DoesNotDisposeOriginalDictionaryFileStream()
            {
                var originalDictionary = new FastPersistentDictionary<int, string>();


                var copiedDictionary = originalDictionary.Copy("path/to/copy.pcd");


                Assert.IsNotNull(originalDictionary.FileStream);
            }

            [Test]
            public void Copy_CopiedDictionaryIsIndependentOfOriginalDictionary()
            {
                var originalDictionary = new FastPersistentDictionary<int, string>();
                originalDictionary.Add(1, "one");
                var copiedDictionary = originalDictionary.Copy("path/to/copy.pcd");


                copiedDictionary.Add(2, "two");


                Assert.AreEqual(1, originalDictionary.Count);
                Assert.AreEqual(2, copiedDictionary.Count);
            }
        }

        [TestFixture]
        public class IntersectTests
        {
            [Test]
            public void Intersect_BothDictionariesEmpty_ReturnsEmptyDictionary()
            {
                var dict1 = new FastPersistentDictionary<int, string>();
                var dict2 = new FastPersistentDictionary<int, string>();


                var result = dict1.Intersect(dict2);


                Assert.IsEmpty(result);
            }

            [Test]
            public void Intersect_FirstDictionaryEmpty_ReturnsEmptyDictionary()
            {
                var dict1 = new FastPersistentDictionary<int, string>();
                var dict2 = new FastPersistentDictionary<int, string>();
                dict2.Add(1, "one");


                var result = dict1.Intersect(dict2);


                Assert.IsEmpty(result);
            }

            [Test]
            public void Intersect_SecondDictionaryEmpty_ReturnsEmptyDictionary()
            {
                var dict1 = new FastPersistentDictionary<int, string>();
                dict1.Add(1, "one");
                var dict2 = new FastPersistentDictionary<int, string>();


                var result = dict1.Intersect(dict2);


                Assert.IsEmpty(result);
            }

            [Test]
            public void Intersect_BothDictionariesHaveSameKeysAndValues_ReturnsDictionaryWithSameKeysAndValues()
            {
                var dict1 = new FastPersistentDictionary<int, string>();
                dict1.Add(1, "one");
                dict1.Add(2, "two");
                dict1.Add(3, "three");
                var dict2 = new FastPersistentDictionary<int, string>();
                dict2.Add(1, "one");
                dict2.Add(2, "two");
                dict2.Add(3, "three");


                var result = dict1.Intersect(dict2);


                Assert.AreEqual(3, result.Count);
                Assert.AreEqual("one", result[1]);
                Assert.AreEqual("two", result[2]);
                Assert.AreEqual("three", result[3]);
            }

            [Test]
            public void Intersect_BothDictionariesHaveDifferentKeysAndValues_ReturnsEmptyDictionary()
            {
                var dict1 = new FastPersistentDictionary<int, string>();
                dict1.Add(1, "one");
                dict1.Add(2, "two");
                dict1.Add(3, "three");
                var dict2 = new FastPersistentDictionary<int, string>();
                dict2.Add(4, "four");
                dict2.Add(5, "five");
                dict2.Add(6, "six");


                var result = dict1.Intersect(dict2);


                Assert.IsEmpty(result);
            }

            [Test]
            public void
                Intersect_FirstDictionaryHasSubsetOfKeysFromSecondDictionary_ReturnsDictionaryWithIntersectingKeysAndValues()
            {
                var dict1 = new FastPersistentDictionary<int, string>();
                dict1.Add(1, "one");
                dict1.Add(3, "three");
                var dict2 = new FastPersistentDictionary<int, string>();
                dict2.Add(1, "one");
                dict2.Add(2, "two");
                dict2.Add(3, "three");


                var result = dict1.Intersect(dict2);


                Assert.AreEqual(2, result.Count);
                Assert.AreEqual("one", result[1]);
                Assert.AreEqual("three", result[3]);
            }

            [Test]
            public void
                Intersect_SecondDictionaryHasSubsetOfKeysFromFirstDictionary_ReturnsDictionaryWithIntersectingKeysAndValues()
            {
                var dict1 = new FastPersistentDictionary<int, string>();
                dict1.Add(1, "one");
                dict1.Add(2, "two");
                var dict2 = new FastPersistentDictionary<int, string>();
                dict2.Add(1, "one");
                dict2.Add(2, "two");
                dict2.Add(3, "three");


                var result = dict1.Intersect(dict2);


                Assert.AreEqual(2, result.Count);
                Assert.AreEqual("one", result[1]);
                Assert.AreEqual("two", result[2]);
            }

            [Test]
            public void
                Intersect_BothDictionariesHaveSameKeysButDifferentValues_ReturnsDictionaryWithIntersectingKeysAndValues()
            {
                var dict1 = new FastPersistentDictionary<int, string>();
                dict1.Add(1, "one");
                dict1.Add(2, "two");
                dict1.Add(3, "three");
                var dict2 = new FastPersistentDictionary<int, string>();
                dict2.Add(1, "1");
                dict2.Add(2, "2");
                dict2.Add(3, "3");


                var result = dict1.Intersect(dict2);


                Assert.AreEqual(3, result.Count);
                Assert.AreEqual("one", result[1]);
                Assert.AreEqual("two", result[2]);
                Assert.AreEqual("three", result[3]);
            }
        }


        [TestFixture]
        public class InvertTests
        {
            [Test]
            public void Invert_ReturnsNewDictionaryWithInvertedKeysAndValues()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                dictionary.Add(1, "one");
                dictionary.Add(2, "two");
                dictionary.Add(3, "three");


                var inverted = dictionary.Invert();


                Assert.IsTrue(inverted.ContainsKey("one"));
                Assert.IsTrue(inverted.ContainsKey("two"));
                Assert.IsTrue(inverted.ContainsKey("three"));
                Assert.AreEqual(1, inverted["one"]);
                Assert.AreEqual(2, inverted["two"]);
                Assert.AreEqual(3, inverted["three"]);
            }
        }

        [TestFixture]
        public class TakeLastTests
        {
            [Test]
            public void TakeLast_ReturnsLastNKeyValuePairs()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                dictionary.Add(1, "one");
                dictionary.Add(2, "two");
                dictionary.Add(3, "three");


                var taken = dictionary.TakeLast(2);


                Assert.AreEqual(2, taken.Count());
                Assert.IsTrue(taken.Any(item => item.Key == 2));
                Assert.IsTrue(taken.Any(item => item.Key == 3));
            }
        }

        [TestFixture]
        public class SaveDictionaryTests
        {
            [SetUp]
            public void SetUp()
            {
                dictionary = new FastPersistentDictionary<int, string>();
            }

            [TearDown]
            public void TearDown()
            {
                if (File.Exists(dictionary.FileLocation)) File.Delete(dictionary.FileLocation);
            }

            private FastPersistentDictionary<int, string> dictionary;

           
            [Test]
            public void SaveDictionary_WithHeader_SavesDictionaryToFileWithGivenHeader()
            {
                var savePath = "./tempdictSave.pcd";
                var header = new DictionaryStructs.DictionarySaveHeader
                {
                    Version = "2.0.0.0",
                    DictionaryCount = 100,
                    Name = "Test Dictionary",
                    Comment = "This is a test dictionary"
                };


                var resultHeader = dictionary.SaveDictionary(savePath, header);


                Assert.IsTrue(File.Exists(savePath));
                Assert.AreEqual(header.Version, resultHeader.Version);
                Assert.AreEqual(header.Name, resultHeader.Name);
                Assert.AreEqual(header.Comment, resultHeader.Comment);
            }

            [Test]
            public void SaveDictionary_SavesDictionary_UpdatesDictionaryChangedEntries()
            {
                var savePath = "./tempdictSave.pcd";


                dictionary.SaveDictionary(savePath);


                //  Assert.AreEqual(dictionary.Count, dictionary.DictionaryChangedEntries.Count);
                //foreach (var entry in dictionary.DictionaryChangedEntries)
                //{
                //    Assert.IsTrue(dictionary.DictionarySerializedCache.ContainsKey(entry.Key));
                //    Assert.IsTrue(dictionary.DictionarySerializedLookup.ContainsKey(entry.Key));
                //}
            }

            [Test]
            public void SaveDictionary_DictionaryIsEmpty_DoesCreateFile()
            {
                var savePath = "./tempdictSave.pcd";
                dictionary.Clear();


                dictionary.SaveDictionary(savePath);


                Assert.IsTrue(File.Exists(savePath));
            }

            [Test]
            public void SaveDictionary_FileAlreadyExists_OverwritesFile()
            {
                var savePath = "./tempdictSave.pcd";
                File.WriteAllText(savePath, "existing data");


                dictionary.SaveDictionary(savePath);


                var savedData = File.ReadAllText(savePath);
                Assert.AreNotEqual("existing data", savedData);
                //Assert.AreEqual(dictionary.Count, dictionary.DictionaryChangedEntries.Count);
            }

            [Test]
            public void SaveDictionary_DirectoryDoesNotExist_CreatesDirectoryAndSavesFile()
            {
                var savePath = "./NewDirectory/tempdictSave.pcd";


                dictionary.SaveDictionary(savePath);


                Assert.IsTrue(File.Exists(savePath));
                // Assert.AreEqual(dictionary.Count, dictionary.DictionaryChangedEntries.Count);

                Directory.Delete("./NewDirectory", true);
            }
        }


        [TestFixture]
        public class SelectTests
        {
            [SetUp]
            public void Setup()
            {
                _dictionary = new FastPersistentDictionary<int, string>();

                _dictionary.Add(1, "One");
                _dictionary.Add(2, "Two");
                _dictionary.Add(3, "Three");
            }

            private FastPersistentDictionary<int, string> _dictionary;

            [Test]
            public void Select_TransformsKeyValuePairToNewType()
            {
                Func<KeyValuePair<int, string>, KeyValuePair<string, int>> selector = kvp =>
                {
                    return new KeyValuePair<string, int>(kvp.Value, kvp.Key);
                };


                var result = _dictionary.Select(selector);


                Assert.IsInstanceOf<IEnumerable<KeyValuePair<string, int>>>(result);
                var resultList = result.ToList();
                Assert.AreEqual(3, resultList.Count);

                Assert.IsTrue(resultList.Any(kvp => kvp.Key == "One" && kvp.Value == 1));
                Assert.IsTrue(resultList.Any(kvp => kvp.Key == "Two" && kvp.Value == 2));
                Assert.IsTrue(resultList.Any(kvp => kvp.Key == "Three" && kvp.Value == 3));
            }

            [Test]
            public void Select_TransformsKeyValuePairToNewType_UsingLambdaExpression()
            {
                var result = _dictionary.Select(kvp => { return new KeyValuePair<string, int>(kvp.Value, kvp.Key); });


                Assert.IsInstanceOf<IEnumerable<KeyValuePair<string, int>>>(result);
                var resultList = result.ToList();
                Assert.AreEqual(3, resultList.Count);

                Assert.IsTrue(resultList.Any(kvp => kvp.Key == "One" && kvp.Value == 1));
                Assert.IsTrue(resultList.Any(kvp => kvp.Key == "Two" && kvp.Value == 2));
                Assert.IsTrue(resultList.Any(kvp => kvp.Key == "Three" && kvp.Value == 3));
            }

            [Test]
            public void Select_ReturnsEmptyEnumerable_WhenDictionaryIsEmpty()
            {
                var emptyDictionary = new FastPersistentDictionary<int, string>();


                var result =
                    emptyDictionary.Select(kvp => { return new KeyValuePair<string, int>(kvp.Value, kvp.Key); });


                Assert.IsEmpty(result);
            }

            [Test]
            public void Select_ReturnsEmptyEnumerable_WhenNoElementsMatchPredicate()
            {
                var result = _dictionary.Select(kvp => { return new KeyValuePair<string, int>(kvp.Value, kvp.Key); })
                    .Where(kvp => kvp.Value == 4);


                Assert.IsEmpty(result);
            }
        }

        [TestFixture]
        public class PackBufferTests
        {
            [Test]
            public void PackBuffer_ReturnsNonEmptyByteArray()
            {
                var dictionary = new FastPersistentDictionary<int, string>();


                var buffer = dictionary.PackBuffer();


                Assert.IsNotNull(buffer);
                Assert.IsNotEmpty(buffer);
            }

            [Test]
            public void PackBuffer_ReturnsByteArrayWithCorrectLength()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                dictionary.Add(1, "One");
                dictionary.Add(2, "Two");
                dictionary.Add(3, "Three");


                var buffer = dictionary.PackBuffer();


                Assert.AreEqual(61, buffer.Length);
            }
        }

        [TestFixture]
        public class PartitionTests
        {
            [SetUp]
            public void SetUp()
            {
                _dictionary = new FastPersistentDictionary<int, string>();
                _dictionary.Add(1, "One");
                _dictionary.Add(2, "Two");
                _dictionary.Add(3, "Three");
                _dictionary.Add(4, "Four");
                _dictionary.Add(5, "Five");
            }

            private FastPersistentDictionary<int, string> _dictionary;

            [Test]
            public void Partition_Predicate_ReturnsTwoPartitions()
            {
                Func<KeyValuePair<int, string>, bool> predicate = kvp => kvp.Key % 2 == 0;


                var partitions = _dictionary.Partition(predicate);


                Assert.AreEqual(2, partitions.Item1.Count());
                Assert.AreEqual(3, partitions.Item2.Count());
            }

            [Test]
            public void Partition_Predicate_OnePartitionIsEmpty()
            {
                Func<KeyValuePair<int, string>, bool> predicate = kvp => kvp.Key > 5;


                var partitions = _dictionary.Partition(predicate);


                Assert.AreEqual(0, partitions.Item1.Count());
                Assert.AreEqual(5, partitions.Item2.Count());
            }

            [Test]
            public void Partition_Predicate_AllPartitionsMatchPredicate()
            {
                Func<KeyValuePair<int, string>, bool> predicate = kvp => kvp.Key > 0;


                var partitions = _dictionary.Partition(predicate);


                Assert.AreEqual(5, partitions.Item1.Count());
                Assert.AreEqual(0, partitions.Item2.Count());
            }

            [Test]
            public void Partition_Predicate_NoPartitionsMatchPredicate()
            {
                Func<KeyValuePair<int, string>, bool> predicate = kvp => kvp.Key > 10;


                var partitions = _dictionary.Partition(predicate);


                Assert.AreEqual(0, partitions.Item1.Count());
                Assert.AreEqual(5, partitions.Item2.Count());
            }

            [Test]
            public void Partition_Predicate_PredicateThrowsException()
            {
                Func<KeyValuePair<int, string>, bool> predicate = kvp =>
                {
                    if (kvp.Key == 3) throw new Exception("Predicate exception");
                    return kvp.Key % 2 == 0;
                };


                Assert.Throws<Exception>(() => _dictionary.Partition(predicate));
            }
        }

        [TestFixture]
        public class ToStringTests
        {
            [Test]
            public void ToString_ReturnsStringRepresentationOfDictionary()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                dictionary.Add(1, "one");
                dictionary.Add(2, "two");
                dictionary.Add(3, "three");


                var result = dictionary.ToString();


                Assert.IsInstanceOf<string>(result);
                Assert.IsTrue(result.Contains("1: one"));
                Assert.IsTrue(result.Contains("2: two"));
                Assert.IsTrue(result.Contains("3: three"));
            }
        }

        [TestFixture]
        public class CountValuesTests
        {
            [Test]
            public void EmptyDictionary_ReturnsZero()
            {
                var dictionary = new FastPersistentDictionary<int, string>();


                var count = dictionary.CountValues("value");


                Assert.AreEqual(0, count);
            }

            [Test]
            public void SingleValueInDictionary_ReturnsOne()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                dictionary.Add(1, "value");


                var count = dictionary.CountValues("value");


                Assert.AreEqual(1, count);
            }

            [Test]
            public void MultipleValuesInDictionary_ReturnsCorrectCount()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                dictionary.Add(1, "value");
                dictionary.Add(2, "value");
                dictionary.Add(3, "value");
                dictionary.Add(4, "otherValue");
                dictionary.Add(5, "value");


                var count = dictionary.CountValues("value");


                Assert.AreEqual(4, count);
            }

            [Test]
            public void NoValuesMatch_ReturnsZero()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                dictionary.Add(1, "value");
                dictionary.Add(2, "value");
                dictionary.Add(3, "value");


                var count = dictionary.CountValues("otherValue");


                Assert.AreEqual(0, count);
            }
        }

        [TestFixture]
        public class InBatchesOfTests
        {
            [TestFixture]
            public class WhenBatchSizeIsGreaterThanDictionaryCount
            {
                [Test]
                public void ReturnsSingleBatchWithAllKeyValuePairs()
                {
                    var dictionary = new FastPersistentDictionary<int, string>();
                    dictionary.Add(1, "One");
                    dictionary.Add(2, "Two");
                    dictionary.Add(3, "Three");


                    var batches = dictionary.InBatchesOf(10).ToList();


                    Assert.AreEqual(1, batches.Count);
                    CollectionAssert.AreEquivalent(dictionary.ToList(), batches.First());
                }
            }

            [TestFixture]
            public class WhenBatchSizeIsEqualToDictionaryCount
            {
                [Test]
                public void ReturnsSingleBatchWithAllKeyValuePairs()
                {
                    var dictionary = new FastPersistentDictionary<int, string>();
                    dictionary.Add(1, "One");
                    dictionary.Add(2, "Two");
                    dictionary.Add(3, "Three");


                    var batches = dictionary.InBatchesOf(3).ToList();


                    Assert.AreEqual(1, batches.Count);
                    CollectionAssert.AreEquivalent(dictionary.ToList(), batches.First());
                }
            }

            [TestFixture]
            public class WhenBatchSizeIsLessThanDictionaryCount
            {
                [Test]
                public void ReturnsMultipleBatchesWithCorrectNumberOfKeyValuePairs()
                {
                    var dictionary = new FastPersistentDictionary<int, string>();
                    dictionary.Add(1, "One");
                    dictionary.Add(2, "Two");
                    dictionary.Add(3, "Three");
                    dictionary.Add(4, "Four");
                    dictionary.Add(5, "Five");


                    var batches = dictionary.InBatchesOf(2).ToList();


                    Assert.AreEqual(3, batches.Count);
                    CollectionAssert.AreEquivalent(
                        new[] { new KeyValuePair<int, string>(1, "One"), new KeyValuePair<int, string>(2, "Two") },
                        batches[0]);
                    CollectionAssert.AreEquivalent(
                        new[] { new KeyValuePair<int, string>(3, "Three"), new KeyValuePair<int, string>(4, "Four") },
                        batches[1]);
                    CollectionAssert.AreEquivalent(new[] { new KeyValuePair<int, string>(5, "Five") }, batches[2]);
                }
            }

            [Test]
            public void ReturnsEmptyEnumerableWhenDictionaryIsEmpty()
            {
                var dictionary = new FastPersistentDictionary<int, string>();


                var batches = dictionary.InBatchesOf(2).ToList();


                Assert.IsFalse(batches.Any());
            }
        }

        [TestFixture]
        public class PopTests
        {
            [Test]
            public void Pop_RemovesAndReturnsValueForGivenKey()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                dictionary.Add(1, "one");
                dictionary.Add(2, "two");


                var value = dictionary.Pop(1);


                Assert.AreEqual("one", value);
                Assert.IsFalse(dictionary.ContainsKey(1));
            }
        }

        [TestFixture]
        public class PushTests
        {
            [Test]
            public void Push_AddsKeyValuePairToDictionary()
            {
                var dictionary = new FastPersistentDictionary<int, string>();


                dictionary.Push(1, "Value");


                Assert.AreEqual("Value", dictionary[1]);
            }


            [Test]
            public void Push_ThrowsArgumentNullException_WhenKeyIsNull()
            {
                var dictionary = new FastPersistentDictionary<string, string>();


                Assert.Throws<ArgumentNullException>(() => dictionary.Push(null, "Value"));
            }
        }

        [TestFixture]
        public class GetHashCodeTests
        {
            [Test]
            public void GetHashCode_ReturnsHashCodeOfDictionary()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                dictionary.Add(1, "one");
                dictionary.Add(2, "two");


                var hashCode = dictionary.GetHashCode();


                Assert.IsNotNull(hashCode);
            }
        }

        [TestFixture]
        public class ElementAtTests
        {
            [Test]
            public void ElementAt_ReturnsElementAtIndex_InDictionaryWithElements()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                dictionary.Add(1, "One");
                dictionary.Add(2, "Two");
                dictionary.Add(3, "Three");


                var result = dictionary.ElementAt(1);


                Assert.AreEqual(new KeyValuePair<int, string>(2, "Two"), result);
            }

            [Test]
            public void ElementAt_ThrowsArgumentOutOfRangeException_WhenIndexIsNegative()
            {
                var dictionary = new FastPersistentDictionary<int, string>();


                Assert.Throws<ArgumentOutOfRangeException>(() => dictionary.ElementAt(-1));
            }

            [Test]
            public void ElementAt_ThrowsArgumentOutOfRangeException_WhenIndexIsGreaterThanCount()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                dictionary.Add(1, "One");
                dictionary.Add(2, "Two");
                dictionary.Add(3, "Three");


                Assert.Throws<ArgumentOutOfRangeException>(() => dictionary.ElementAt(3));
            }

            [Test]
            public void ElementAt_ReturnsElementAtIndex_WhenIndexIsZero()
            {
                var dictionary = new FastPersistentDictionary<int, string>();
                dictionary.Add(1, "One");
                dictionary.Add(2, "Two");
                dictionary.Add(3, "Three");


                var result = dictionary.ElementAt(0);


                Assert.AreEqual(new KeyValuePair<int, string>(1, "One"), result);
            }
        }

        [TestFixture]
        public class ExceptTests
        {
            [Test]
            public void Except_ReturnsDictionaryWithElementsNotPresentInOtherDictionary()
            {
                var dictionary1 = new FastPersistentDictionary<int, string>();
                dictionary1.Add(1, "One");
                dictionary1.Add(2, "Two");
                dictionary1.Add(3, "Three");

                var dictionary2 = new FastPersistentDictionary<int, string>();
                dictionary2.Add(2, "Two");
                dictionary2.Add(4, "Four");
                dictionary2.Add(5, "Five");


                var result = dictionary1.Except(dictionary2);


                Assert.AreEqual(2, result.Count());
                Assert.IsTrue(result.Any(item => item.Key == 1));
                Assert.IsTrue(result.Any(item => item.Key == 3));
                Assert.AreEqual("One", result.ElementAt(0).Value);
                Assert.AreEqual("Three", result.ElementAt(1).Value);
            }

            [Test]
            public void Except_ReturnsEmptyDictionary_WhenOtherDictionaryIsEmpty()
            {
                var dictionary1 = new FastPersistentDictionary<int, string>();
                dictionary1.Add(1, "One");
                dictionary1.Add(2, "Two");

                var dictionary2 = new FastPersistentDictionary<int, string>();


                var result = dictionary1.Except(dictionary2);


                Assert.AreEqual(2, result.Count());
                Assert.IsTrue(result.Any(item => item.Key == 1));
                Assert.IsTrue(result.Any(item => item.Key == 2));
                Assert.AreEqual("One", result.ElementAt(0).Value);
                Assert.AreEqual("Two", result.ElementAt(1).Value);
            }

            [Test]
            public void Except_ReturnsDictionaryWithAllElements_WhenOtherDictionaryDoesNotOverlap()
            {
                var dictionary1 = new FastPersistentDictionary<int, string>();
                dictionary1.Add(1, "One");
                dictionary1.Add(2, "Two");

                var dictionary2 = new FastPersistentDictionary<int, string>();
                dictionary2.Add(3, "Three");
                dictionary2.Add(4, "Four");


                var result = dictionary1.Except(dictionary2);


                Assert.AreEqual(2, result.Count());
                Assert.IsTrue(result.Any(item => item.Key == 1));
                Assert.IsTrue(result.Any(item => item.Key == 2));
                Assert.AreEqual("One", result.ElementAt(0).Value);
                Assert.AreEqual("Two", result.ElementAt(1).Value);
            }

            [Test]
            public void Except_ReturnsEmptyDictionary_WhenBothDictionariesAreEmpty()
            {
                var dictionary1 = new FastPersistentDictionary<int, string>();
                var dictionary2 = new FastPersistentDictionary<int, string>();


                var result = dictionary1.Except(dictionary2);


                Assert.AreEqual(0, result.Count());
            }

            [Test]
            public void Except_ReturnsEmptyDictionary_WhenOtherDictionaryContainsAllElements()
            {
                var dictionary1 = new FastPersistentDictionary<int, string>();
                dictionary1.Add(1, "One");
                dictionary1.Add(2, "Two");

                var dictionary2 = new FastPersistentDictionary<int, string>();
                dictionary2.Add(1, "One");
                dictionary2.Add(2, "Two");


                var result = dictionary1.Except(dictionary2);


                Assert.AreEqual(0, result.Count());
            }

            [Test]
            public void
                Except_ReturnsDictionaryWithElementsMinusTheCommonElements_WhenOtherDictionaryHasSomeCommonElements()
            {
                var dictionary1 = new FastPersistentDictionary<int, string>();
                dictionary1.Add(1, "One");
                dictionary1.Add(2, "Two");
                dictionary1.Add(3, "Three");

                var dictionary2 = new FastPersistentDictionary<int, string>();
                dictionary2.Add(2, "Two");
                dictionary2.Add(4, "Four");


                var result = dictionary1.Except(dictionary2);


                Assert.AreEqual(2, result.Count());
                Assert.IsTrue(result.Any(item => item.Key == 1));
                Assert.IsTrue(result.Any(item => item.Key == 3));
                Assert.AreEqual("One", result.ElementAt(0).Value);
                Assert.AreEqual("Three", result.ElementAt(1).Value);
            }
        }


        [TestFixture]
        public sealed class EqualsTests
        {
            [TestFixture]
            public class WhenComparingWithDifferentType
            {
                [Test]
                public void ReturnsFalse()
                {
                    var dictionary = new FastPersistentDictionary<int, string>();
                    var otherObject = new object();


                    var result = dictionary.Equals(otherObject);


                    Assert.IsFalse(result);
                }
            }

            [TestFixture]
            public class WhenComparingWithItself
            {
                [Test]
                public void ReturnsTrue()
                {
                    var dictionary = new FastPersistentDictionary<int, string>();


                    var result = dictionary.Equals(dictionary);


                    Assert.IsTrue(result);
                }
            }

            [TestFixture]
            public class WhenComparingWithDifferentDictionaryWithSameContents
            {
                [Test]
                public void ReturnsTrue()
                {
                    var dictionary1 = new FastPersistentDictionary<int, string>();
                    dictionary1.Add(1, "One");
                    dictionary1.Add(2, "Two");

                    var dictionary2 = new FastPersistentDictionary<int, string>();
                    dictionary2.Add(1, "One");
                    dictionary2.Add(2, "Two");


                    var result = dictionary1.Equals(dictionary2);


                    Assert.IsTrue(result);
                }
            }

            [TestFixture]
            public class WhenComparingWithDifferentDictionaryWithDifferentContents
            {
                [Test]
                public void ReturnsFalse()
                {
                    var dictionary1 = new FastPersistentDictionary<int, string>();
                    dictionary1.Add(1, "One");
                    dictionary1.Add(2, "Two");

                    var dictionary2 = new FastPersistentDictionary<int, string>();
                    dictionary2.Add(1, "One");
                    dictionary2.Add(3, "Three");


                    var result = dictionary1.Equals(dictionary2);


                    Assert.IsFalse(result);
                }
            }
        }
    }
}
