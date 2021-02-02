namespace InsightArchitectures.Utilities
{
    /// <summary>
    /// This class contains a set of extension methods for <see cref="string" />.
    /// </summary>
    public static class NullStringExtensions
    {
        /// <summary>
        /// Returns <c>null</c> if the input string is empty.
        /// </summary>
        /// <param name="input">The <see cref="string" /> to test.</param>
        /// <returns>The same string if there is any content. Otherwise, <c>null</c>.</returns>
        public static string? NullIfEmpty(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return null;
            }

            return input;
        }

        /// <summary>
        /// Returns <c>null</c> if the input string is empty or filled with whitespaces.
        /// </summary>
        /// <param name="input">The <see cref="string" /> to test.</param>
        /// <returns>The same string if there is any content. Otherwise, <c>null</c>.</returns>
        public static string? NullIfEmptyOrWhiteSpace(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return null;
            }

            return input;
        }
    }
}
