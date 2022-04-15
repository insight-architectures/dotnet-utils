using System.Collections.Generic;
using NUnit.Framework;
using SetExtensions = System.Collections.Generic.InsightArchitecturesSetExtensions;

// ReSharper disable InvokeAsExtensionMethod

namespace Tests;

[TestFixture]
[TestOf(typeof(SetExtensions))]
public class SetExtensionsTests
{
    [TestFixture]
    [TestOf(nameof(SetExtensions.Add))]
    public class Add
    {
        [Test, CustomAutoData]
        public void Adds_sequence_of_items_to_list(string testString)
        {
            // ReSharper disable once UseObjectOrCollectionInitializer
            var set = new HashSet<string>();

            SetExtensions.Add(set, new[] { testString });
        
            Assert.That(set, Contains.Item(testString));
        }
    
        [Test, CustomAutoData]
        public void Adds_sequence_of_items_to_list_using_collection_initializer(string testString)
        {
            var set = new HashSet<string> 
            {
                new[] { testString }
            };

            Assert.That(set, Contains.Item(testString));
        }

        [Test, CustomAutoData]
        public void Does_nothing_is_sequence_to_add_is_null()
        {
            var set = new HashSet<string>();

            SetExtensions.Add(set, null!);

            Assert.Pass();
        }

        [Test, CustomAutoData]
        public void Throws_is_list_is_null(string[] testStrings) => Assert.That(() => SetExtensions.Add(null!, testStrings), Throws.ArgumentNullException);
    }
    
    [TestFixture]
    [TestOf(nameof(SetExtensions.AddRange))]
    public class AddRange
    {
        [Test, CustomAutoData]
        public void Adds_sequence_of_items_to_list(string testString)
        {
            // ReSharper disable once UseObjectOrCollectionInitializer
            var set = new HashSet<string>();

            SetExtensions.AddRange(set, new[] { testString });
        
            Assert.That(set, Contains.Item(testString));
        }

        [Test, CustomAutoData]
        public void Does_nothing_is_sequence_to_add_is_null()
        {
            var set = new HashSet<string>();

            SetExtensions.AddRange(set, null!);

            Assert.Pass();
        }

        [Test, CustomAutoData]
        public void Throws_is_list_is_null(string[] testStrings) => Assert.That(() => SetExtensions.AddRange(null!, testStrings), Throws.ArgumentNullException);
    }
}
