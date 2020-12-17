using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace InsightArchitectures.Utilities
{
    /// <summary>
    /// This class contains a set of extension methods for <see cref="string" />.
    /// </summary>
    public static class HashStringExtensions
    {
        /// <summary>
        /// Calculates an hashed representation of <paramref name="input"/> using the given <paramref name="algorithm"/> and the <paramref name="byteFormat"/>.
        /// </summary>
        /// <param name="input">The <see cref="string"/> to hash.</param>
        /// <param name="algorithm">An instance of <see cref="HashAlgorithm" /> used to calculate the hash <paramref name="input"/>.</param>
        /// <param name="byteFormat">The format string used to convert each byte into a string. Default is <c>x2</c>.</param>
        /// <returns>The hash of input string.</returns>
        public static string Hash(this string input, HashAlgorithm algorithm, string byteFormat = "x2")
        {
            _ = input ?? throw new ArgumentNullException(nameof(input));

            _ = algorithm ?? throw new ArgumentNullException(nameof(algorithm));

            _ = byteFormat ?? throw new ArgumentNullException(nameof(byteFormat));

            var inputBytes = Encoding.UTF8.GetBytes(input);

            var hashBytes = algorithm.ComputeHash(inputBytes);

            var sb = new StringBuilder();

            for (var i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString(byteFormat, CultureInfo.InvariantCulture));
            }

            return sb.ToString();
        }

        /// <summary>
        /// Calculates an hashed representation of <paramref name="input"/> using MD5 and the <paramref name="byteFormat"/>.
        /// </summary>
        /// <param name="input">The <see cref="string"/> to hash.</param>
        /// <param name="byteFormat">The format string used to convert each byte into a string. Default is <c>x2</c>.</param>
        /// <returns>The MD5 hash of input string.</returns>
        public static string MD5(this string input, string byteFormat = "x2")
        {
#pragma warning disable CA5351 // Do Not Use Broken Cryptographic Algorithms
            using var hasher = System.Security.Cryptography.MD5.Create();
#pragma warning restore CA5351 // Do Not Use Broken Cryptographic Algorithms

            return Hash(input, hasher, byteFormat);
        }
    }
}
