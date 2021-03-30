// ReSharper disable InvokeAsExtensionMethod

using System;
using System.Collections;
using System.Linq;
using AutoFixture;
using InsightArchitectures.Utilities;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    [TestOf(typeof(EnumerableExtensions))]
    public class EnumerableExtensionsTests
    {
        [Test, CustomAutoData]
        public void EmptyIfNull_returns_same_sequence_if_not_null(TypeWithProperty<string>[] items)
        {
            var result = EnumerableExtensions.EmptyIfNull(items);

            Assert.That(result, Is.SameAs(items));
        }

        [Test, CustomAutoData]
        public void EmptyIfNull_returns_same_sequence_if_null()
        {
            var result = EnumerableExtensions.EmptyIfNull<TypeWithProperty<string>>(null!);

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.Empty);
            });
        }

        [Test]
        [CustomInlineAutoData(0)]
        [CustomInlineAutoData(-1)]
        public void Paginate_throws_if_page_is_less_or_equal_than_zero(int pageSize, string[] items)
        {
            Assert.That(() => EnumerableExtensions.Paginate(items, pageSize).ToArray(), Throws.InstanceOf<ArgumentOutOfRangeException>());
        }

        [Test, CustomAutoData]
        public void Paginate_returns_empty_sequence_if_source_sequence_is_null(int pageSize)
        {
            var result = EnumerableExtensions.Paginate<string>(null!, pageSize).ToArray();

            Assert.That(result, Is.Not.Null.And.Empty);
        }

        [Test, CustomAutoData]
        public void Paginate_returns_list_of_pages_never_longer_than_pageSize(int pageSize, Generator<string> generator, int numberOfItems)
        {
            var items = generator.Take(numberOfItems);

            var pages = EnumerableExtensions.Paginate(items, pageSize);

            Assert.Multiple(() =>
            {
                foreach (var page in pages)
                {
                    Assert.That(page.Count(), Is.LessThanOrEqualTo(pageSize));
                }
            });
        }
    }
}
