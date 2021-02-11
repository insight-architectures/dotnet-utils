using InsightArchitectures.Utilities;
using NUnit.Framework;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Tests
{
    [TestFixture]
    [TestOf(typeof(HashStringExtensions))]
    public class HashStringExtensionsTests
    {
        [Test]
        [TestCaseSource(nameof(GetAlgorithms))]
        public void Hash_returns_correct_hash_with_specified_encoding(string input, HashAlgorithm algorithm, string expected)
        {
            var result = input.Hash(Encoding.UTF8, algorithm);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        [TestCaseSource(nameof(GetAlgorithms))]
        public void Hash_returns_correct_hash_with_default_encoding(string input, HashAlgorithm algorithm, string expected)
        {
            var result = input.Hash(algorithm);

            Assert.That(result, Is.EqualTo(expected));
        }

        public static IEnumerable<object[]> GetAlgorithms()
        {
            yield return new object[] { "Hello world", MD5.Create(), "3e25960a79dbc69b674cd4ec67a72c62" };

            yield return new object[] { "Hello world", SHA1.Create(), "7b502c3a1f48c8609ae212cdfb639dee39673f5e" };
        }

        [Test]
        public void MD5_returns_correct_hash_with_specified_encoding()
        {
            var result = "Hello world".MD5(Encoding.UTF8);

            const string expected = "3e25960a79dbc69b674cd4ec67a72c62";

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void MD5_returns_correct_hash_with_default_encoding()
        {
            var result = "Hello world".MD5();

            const string expected = "3e25960a79dbc69b674cd4ec67a72c62";

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        [TestCaseSource(nameof(GetEncodings))]
        public void MD5_returns_correct_hash_with_specified_encoding(string input, Encoding encoding, string expected)
        {
            var result = input.MD5(encoding);

            Assert.That(result, Is.EqualTo(expected));
        }

        public static IEnumerable<object[]> GetEncodings()
        {
            yield return new object[] { "string with swedish characters: ö ä å", Encoding.UTF8, "29f815f91399c52782a11e5206ae2b9f" };

            yield return new object[] { "string with swedish characters: ö ä å", Encoding.ASCII, "b65d919f7a01ca0ca25b694c50a77c35" };
        }
    }
}
