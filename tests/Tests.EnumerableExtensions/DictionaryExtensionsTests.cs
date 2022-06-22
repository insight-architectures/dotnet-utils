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

    [TestFixture]
    [TestOf(nameof(DictionaryExtensions.GetValueOrFallback))]
    public class GetValueOrFallback
    {
        [Test, CustomAutoData]
        public void Returns_value_if_key_is_available_in_dictionary(string key, string value, string fallback)
        {
            var sut = new Dictionary<string, string>
            {
                [key] = value
            };

            var result = DictionaryExtensions.GetValueOrFallback(sut, key, fallback);

            Assert.That(result, Is.EqualTo(value));
        }

        [Test, CustomAutoData]
        public void Returns_fallback_value_if_key_is_not_available_in_dictionary(string key, string fallback)
        {
            var sut = new Dictionary<string, string>();

            var result = DictionaryExtensions.GetValueOrFallback(sut, key, fallback);

            Assert.That(result, Is.EqualTo(fallback));
        }

        [Test, CustomAutoData]
        public void Returns_value_if_key_is_available(string key, string value, string fallback)
        {
            IDictionary<string, string> sut = new Dictionary<string, string>
            {
                [key] = value
            };

            var result = DictionaryExtensions.GetValueOrFallback(sut, key, fallback);

            Assert.That(result, Is.EqualTo(value));
        }

        [Test, CustomAutoData]
        public void Returns_fallback_value_if_key_is_not_available(string key, string fallback)
        {
            IDictionary<string, string> sut = new Dictionary<string, string>();

            var result = DictionaryExtensions.GetValueOrFallback(sut, key, fallback);

            Assert.That(result, Is.EqualTo(fallback));
        }

        [Test, CustomAutoData]
        public void Returns_value_if_key_is_available_in_readonly_dictionary(string key, string value, string fallback)
        {
            IReadOnlyDictionary<string, string> sut = new Dictionary<string, string>
            {
                [key] = value
            };

            var result = DictionaryExtensions.GetValueOrFallback(sut, key, fallback);

            Assert.That(result, Is.EqualTo(value));
        }

        [Test, CustomAutoData]
        public void Returns_fallback_value_if_key_is_not_available_in_readonly_dictionary(string key, string fallback)
        {
            IReadOnlyDictionary<string, string> sut = new Dictionary<string, string>();

            var result = DictionaryExtensions.GetValueOrFallback(sut, key, fallback);

            Assert.That(result, Is.EqualTo(fallback));
        }
    }

    [TestFixture]
    [TestOf(nameof(DictionaryExtensions.GetValueOrFallback))]
    public class GetValueOrDefault
    {
        [Test, CustomAutoData]
        public void Returns_value_if_key_is_available_in_dictionary(string key, string value)
        {
            var sut = new Dictionary<string, string>
            {
                [key] = value
            };

            var result = DictionaryExtensions.GetValueOrDefault(sut, key);

            Assert.That(result, Is.EqualTo(value));
        }

        [Test, CustomAutoData]
        public void Returns_fallback_value_if_key_is_not_available_in_dictionary(string key)
        {
            var sut = new Dictionary<string, string>();

            var result = DictionaryExtensions.GetValueOrDefault(sut, key);

            Assert.That(result, Is.EqualTo(default(string)));
        }

        [Test, CustomAutoData]
        public void Returns_value_if_key_is_available(string key, string value)
        {
            IDictionary<string, string> sut = new Dictionary<string, string>
            {
                [key] = value
            };

            var result = DictionaryExtensions.GetValueOrDefault(sut, key);

            Assert.That(result, Is.EqualTo(value));
        }

        [Test, CustomAutoData]
        public void Returns_fallback_value_if_key_is_not_available(string key)
        {
            IDictionary<string, string> sut = new Dictionary<string, string>();

            var result = DictionaryExtensions.GetValueOrDefault(sut, key);

            Assert.That(result, Is.EqualTo(default(string)));
        }

        [Test, CustomAutoData]
        public void Returns_value_if_key_is_available_in_readonly_dictionary(string key, string value)
        {
            IReadOnlyDictionary<string, string> sut = new Dictionary<string, string>
            {
                [key] = value
            };

            var result = DictionaryExtensions.GetValueOrDefault(sut, key);

            Assert.That(result, Is.EqualTo(value));
        }

        [Test, CustomAutoData]
        public void Returns_fallback_value_if_key_is_not_available_in_readonly_dictionary(string key)
        {
            IReadOnlyDictionary<string, string> sut = new Dictionary<string, string>();

            var result = DictionaryExtensions.GetValueOrDefault(sut, key);

            Assert.That(result, Is.EqualTo(default(string)));
        }
    }
}
