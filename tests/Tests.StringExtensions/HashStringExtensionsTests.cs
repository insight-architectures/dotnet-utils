using InsightArchitectures.Utilities;
using NUnit.Framework;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Tests
{
    [TestFixture]
    [TestOf(typeof(HashStringExtensions))]
    public class HashStringExtensionsTests
    {
        [Test]
        [TestCaseSource(nameof(GetAlgorithms))]
        public void Hash_returns_correct_hash(HashAlgorithm algorithm, string expected)
        {
            var result = "Hello world".Hash(algorithm);

            Assert.That(result, Is.EqualTo(expected));
        }

        public static IEnumerable<object[]> GetAlgorithms()
        {
            yield return new object[] { MD5.Create(), "3e25960a79dbc69b674cd4ec67a72c62" };

            yield return new object[] { SHA1.Create(), "7b502c3a1f48c8609ae212cdfb639dee39673f5e" };
        }

        [Test]
        public void MD5_returns_correct_hash()
        {
            var result = "Hello world".MD5();

            const string expected = "3e25960a79dbc69b674cd4ec67a72c62";

            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
