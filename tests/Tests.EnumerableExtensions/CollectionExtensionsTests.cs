using System.Collections.Generic;
using NUnit.Framework;
using CollectionExtensions = System.Collections.Generic.InsightArchitecturesCollectionExtensions;

// ReSharper disable InvokeAsExtensionMethod

namespace Tests;

[TestFixture]
[TestOf(typeof(CollectionExtensions))]
public class CollectionExtensionsTests
{
    [TestFixture]
    [TestOf(nameof(CollectionExtensions.Add))]
    public class Add
    {
        [Test, CustomAutoData]
        public void Adds_sequence_of_items_to_list(string testString)
        {
            // ReSharper disable once UseObjectOrCollectionInitializer
            var list = new List<string>();

            CollectionExtensions.Add(list, new[] { testString });
        
            Assert.That(list, Contains.Item(testString));
        }
    
        [Test, CustomAutoData]
        public void Adds_sequence_of_items_to_list_using_collection_initializer(string testString)
        {
            var list = new List<string> 
            {
                new[] { testString }
            };

            Assert.That(list, Contains.Item(testString));
        }

        [Test, CustomAutoData]
        public void Does_nothing_is_sequence_to_add_is_null()
        {
            var list = new List<string>();

            CollectionExtensions.Add(list, null!);

            Assert.Pass();
        }

        [Test, CustomAutoData]
        public void Throws_is_list_is_null(string[] testStrings) => Assert.That(() => CollectionExtensions.Add(null!, testStrings), Throws.ArgumentNullException);
    }
    
    [TestFixture]
    [TestOf(nameof(CollectionExtensions.AddRange))]
    public class AddRange
    {
        [Test, CustomAutoData]
        public void Adds_sequence_of_items_to_list(string testString)
        {
            // ReSharper disable once UseObjectOrCollectionInitializer
            var list = new List<string>();

            CollectionExtensions.AddRange(list, new[] { testString });
        
            Assert.That(list, Contains.Item(testString));
        }

        [Test, CustomAutoData]
        public void Does_nothing_is_sequence_to_add_is_null()
        {
            var list = new List<string>();

            CollectionExtensions.AddRange(list, null!);

            Assert.Pass();
        }

        [Test, CustomAutoData]
        public void Throws_is_list_is_null(string[] testStrings) => Assert.That(() => CollectionExtensions.AddRange(null!, testStrings), Throws.ArgumentNullException);
    }
}
