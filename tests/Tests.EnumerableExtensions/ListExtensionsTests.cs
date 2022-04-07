using System.Collections.Generic;
using InsightArchitectures.Utilities;
using NUnit.Framework;
// ReSharper disable InvokeAsExtensionMethod

namespace Tests;

[TestFixture]
[TestOf(typeof(ListExtensions))]
public class ListExtensionsTests
{
    [TestFixture]
    [TestOf(nameof(ListExtensions.Add))]
    public class Add
    {
        [Test, CustomAutoData]
        public void Adds_sequence_of_items_to_list(string testString)
        {
            // ReSharper disable once UseObjectOrCollectionInitializer
            var list = new List<string>();

            ListExtensions.Add(list, new[] { testString });
        
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

            ListExtensions.Add(list, null!);

            Assert.Pass();
        }

        [Test, CustomAutoData]
        public void Throws_is_list_is_null(string[] testStrings) => Assert.That(() => ListExtensions.Add(null!, testStrings), Throws.ArgumentNullException);
    }
    
    [TestFixture]
    [TestOf(nameof(ListExtensions.AddRange))]
    public class AddRange
    {
        [Test, CustomAutoData]
        public void Adds_sequence_of_items_to_list(string testString)
        {
            // ReSharper disable once UseObjectOrCollectionInitializer
            var list = new List<string>();

            ListExtensions.AddRange(list, new[] { testString });
        
            Assert.That(list, Contains.Item(testString));
        }

        [Test, CustomAutoData]
        public void Does_nothing_is_sequence_to_add_is_null()
        {
            var list = new List<string>();

            ListExtensions.AddRange(list, null!);

            Assert.Pass();
        }

        [Test, CustomAutoData]
        public void Throws_is_list_is_null(string[] testStrings) => Assert.That(() => ListExtensions.AddRange(null!, testStrings), Throws.ArgumentNullException);
    }
}
