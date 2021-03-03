using System;
using System.Collections.Generic;

namespace InsightArchitectures.Utilities
{
    /// <summary>
    /// A set of extension methods on the <see cref="IEnumerable{T}"/> type that help handling sequences of objects.
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Returns an empty sequence if <paramref name="source"/> is <see langword="null" />.
        /// </summary>
        /// <typeparam name="T">The type of items in <paramref name="source"/>.</typeparam>
        /// <param name="source">The sequence to be checked if <see langword="null" />.</param>
        /// <returns>The same <paramref name="source"/> if not <see langword="null" />, otherwise an empty sequence of <typeparamref name="T"/>.</returns>
        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T>? source) => source ?? Array.Empty<T>();
    }
}
