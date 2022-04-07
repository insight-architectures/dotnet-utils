// ReSharper disable InvokeAsExtensionMethod

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using InsightArchitectures.Utilities.Internal;

namespace InsightArchitectures.Utilities
{
    /// <summary>
    /// A set of extension methods on the <see cref="IEnumerable{T}"/> type that help detecting distinct instances of objects.
    /// </summary>
    public static class DistinctEnumerableExtensions
    {
        /// <summary>
        /// Returns distinct elements from a sequence using the values produced by the <paramref name="selector"/> to compare them.
        /// </summary>
        /// <typeparam name="T">The type of the objects to compare.</typeparam>
        /// <typeparam name="TValue">The type of the result of the selector to be applied to the objects to compare.</typeparam>
        /// <param name="source">The sequence to remove elements from.</param>
        /// <param name="selector">A delegate used to select which values to be used to compare objects of type <typeparamref name="T"/>.</param>
        /// <param name="equalityComparer">An <see cref="IEqualityComparer{T}"/> to compare values produced by <paramref name="selector"/>.</param>
        /// <returns>An <see cref="IEnumerable{T}" /> that contains distinct elements from the source sequence.</returns>
        /// <remarks>
        /// Best used to extract the distinct instances of a reference type using a property as discriminator.
        /// </remarks>
#if NET5_0_OR_GREATER
        [return: NotNull]
#endif
        public static IEnumerable<T> DistinctBy<T, TValue>(this IEnumerable<T>? source, Func<T, TValue> selector, IEqualityComparer<TValue>? equalityComparer = null)
        {
#if NET6_0_OR_GREATER
            return Enumerable.DistinctBy(source.EmptyIfNull(), selector, equalityComparer);
#else
            _ = selector ?? throw new ArgumentNullException(nameof(selector));

            if (source is null)
            {
                return Array.Empty<T>();
            }

            return Enumerable.Distinct(source, new SelectorEqualityComparer<T, TValue>(selector, equalityComparer ?? EqualityComparer<TValue>.Default));
#endif
        }
    }
}
