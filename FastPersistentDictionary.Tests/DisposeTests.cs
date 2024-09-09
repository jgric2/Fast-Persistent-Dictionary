namespace FastPersistentDictionary.Tests;

public sealed partial class PersistentDictionaryProTests
{
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
}