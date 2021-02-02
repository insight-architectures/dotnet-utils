using AutoFixture.NUnit3;
using InsightArchitectures.Utilities;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    [TestOf(typeof(NullStringExtensions))]
    public class NullStringExtensionsTests
    {
        [Test]
        [TestCase("")]
        public void NullIfEmpty_should_return_null_if_input_is_empty_string(string input)
        {
            var result = NullStringExtensions.NullIfEmpty(input);

            Assert.That(result, Is.Null);
        }

        [Test, AutoData]
        public void NullIfEmpty_should_return_same_string_not_null(string input)
        {
            Assume.That(input, Is.Not.Null);

            var result = NullStringExtensions.NullIfEmpty(input);

            Assert.That(result, Is.EqualTo(input));
        }

        [Test]
        [TestCase(" ")]
        [TestCase("  ")]
        [TestCase("   ")]
        public void NullIfEmpty_should_return_same_string_if_filled_with_whitespaces(string input)
        {
            var result = NullStringExtensions.NullIfEmpty(input);

            Assert.That(result, Is.EqualTo(input));
        }

        [Test]
        [TestCase("")]
        public void NullIfEmptyOrWhiteSpace_should_return_null_if_input_is_empty_string(string input)
        {
            var result = NullStringExtensions.NullIfEmptyOrWhiteSpace(input);

            Assert.That(result, Is.Null);
        }

        [Test, AutoData]
        public void NullIfEmptyOrWhiteSpace_should_return_same_string_not_null(string input)
        {
            Assume.That(input, Is.Not.Null);

            var result = NullStringExtensions.NullIfEmptyOrWhiteSpace(input);

            Assert.That(result, Is.EqualTo(input));
        }

        [Test]
        [TestCase(" ")]
        [TestCase("  ")]
        [TestCase("   ")]
        public void NullIfEmptyOrWhiteSpace_should_return_same_string_if_filled_with_whitespaces(string input)
        {
            var result = NullStringExtensions.NullIfEmptyOrWhiteSpace(input);

            Assert.That(result, Is.Null);
        }
    }
}
