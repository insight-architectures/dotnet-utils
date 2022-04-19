using System.Collections.Generic;
using NUnit.Framework;
using DictionaryExtensions = System.Collections.Generic.InsightArchitecturesDictionaryExtensions;

// ReSharper disable InvokeAsExtensionMethod

namespace Tests;

[TestFixture]
[TestOf(typeof(DictionaryExtensions))]
public class DictionaryExtensionsTests
{
    [TestFixture]
    [TestOf(nameof(DictionaryExtensions.Add))]
    public class Add
    {
        [Test, CustomAutoData]
        public void Adds_sequence_of_items_to_list(string testKey, string testValue)
        {
            // ReSharper disable once UseObjectOrCollectionInitializer
            var dictionary = new Dictionary<string, string>();

            DictionaryExtensions.Add(dictionary, new[] { new KeyValuePair<string, string>(testKey, testValue) });
            
            Assert.Multiple(() =>
            {
                Assert.That(dictionary, Contains.Key(testKey));
                Assert.That(dictionary, Contains.Value(testValue));
            });
        }
    
        [Test, CustomAutoData]
        public void Adds_sequence_of_items_to_list_using_collection_initializer(string testKey, string testValue)
        {
            var dictionary = new Dictionary<string, string>
            {
                new[] { new KeyValuePair<string, string>(testKey, testValue) }
            };

            Assert.Multiple(() =>
            {
                Assert.That(dictionary, Contains.Key(testKey));
                Assert.That(dictionary, Contains.Value(testValue));
            });
        }

        [Test, CustomAutoData]
        public void Does_nothing_is_sequence_to_add_is_null()
        {
            var dictionary = new Dictionary<string, string>();

            DictionaryExtensions.Add(dictionary, null!);

            Assert.Pass();
        }

        [Test, CustomAutoData]
        public void Throws_is_list_is_null(KeyValuePair<string, string>[] testItems) => Assert.That(() => DictionaryExtensions.Add(null!, testItems), Throws.ArgumentNullException);
    }
    
    [TestFixture]
    [TestOf(nameof(DictionaryExtensions.AddRange))]
    public class AddRange
    {
        [Test, CustomAutoData]
        public void Adds_sequence_of_items_to_list(string testKey, string testValue)
        {
            // ReSharper disable once UseObjectOrCollectionInitializer
            var dictionary = new Dictionary<string, string>();

            DictionaryExtensions.AddRange(dictionary, new[] { new KeyValuePair<string, string>(testKey, testValue) });
        
            Assert.Multiple(() =>
            {
                Assert.That(dictionary, Contains.Key(testKey));
                Assert.That(dictionary, Contains.Value(testValue));
            });
        }

        [Test, CustomAutoData]
        public void Does_nothing_is_sequence_to_add_is_null()
        {
            var dictionary = new Dictionary<string, string>();

            DictionaryExtensions.AddRange(dictionary, null!);

            Assert.Pass();
        }

        [Test, CustomAutoData]
        public void Throws_is_list_is_null(KeyValuePair<string, string>[] testItems) => Assert.That(() => DictionaryExtensions.AddRange(null!, testItems), Throws.ArgumentNullException);
    }
}
