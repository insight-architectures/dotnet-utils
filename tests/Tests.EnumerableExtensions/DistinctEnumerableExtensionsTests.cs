// ReSharper disable InvokeAsExtensionMethod

using System.Linq;
using AutoFixture.NUnit3;
using InsightArchitectures.Utilities;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    [TestOf(typeof(DistinctEnumerableExtensions))]
    public class DistinctEnumerableExtensionsTests
    {
        [Test]
        public void DistinctBy_returns_empty_sequence_if_source_is_null()
        {
            TypeWithProperty<string>[] source = null!;

            var result = DistinctEnumerableExtensions.DistinctBy(source, item => item.Property);

            Assert.That(result, Is.Empty);
        }

        [Test, CustomAutoData]
        public void DistinctBy_returns_single_item_if_discriminator_is_same([Frozen] string value, TypeWithProperty<string>[] items)
        {
            var result = DistinctEnumerableExtensions.DistinctBy(items, item => item.Property).ToArray();

            Assert.Multiple(() =>
            {
                Assert.That(result, Has.Exactly(1).InstanceOf<TypeWithProperty<string>>());
                Assert.That(result[0].Property, Is.EqualTo(value));
            });
        }

        [Test, CustomAutoData]
        public void DistinctBy_returns_single_item_if_compound_discriminator_is_same([Frozen] string firstValue, [Frozen] int secondValue, TypeWithMultipleProperties<string, int>[] items)
        {
            var result = DistinctEnumerableExtensions.DistinctBy(items, item => (item.FirstProperty, item.SecondProperty)).ToArray();

            Assert.Multiple(() =>
            {
                Assert.That(result, Has.Exactly(1).InstanceOf<TypeWithMultipleProperties<string, int>>());

                Assert.That(result[0].FirstProperty, Is.EqualTo(firstValue));
                Assert.That(result[0].SecondProperty, Is.EqualTo(secondValue));
            });
            
        }
    }
}
