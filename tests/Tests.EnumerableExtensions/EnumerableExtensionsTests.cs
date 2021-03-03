// ReSharper disable InvokeAsExtensionMethod

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
    }
}
