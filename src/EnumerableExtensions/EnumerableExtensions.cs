// ReSharper disable InvokeAsExtensionMethod

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using InsightArchitectures.Utilities.Internal;

namespace System.Collections.Generic
{
#pragma warning disable SA1649
    /// <summary>
    /// A set of extension methods on the <see cref="IEnumerable{T}"/> type that help handling sequences of objects.
    /// </summary>
    public static class InsightArchitecturesEnumerableExtensions
    {
        /// <summary>
        /// Returns an empty sequence if <paramref name="source"/> is <see langword="null" />.
        /// </summary>
        /// <typeparam name="T">The type of items in <paramref name="source"/>.</typeparam>
        /// <param name="source">The sequence to be checked if <see langword="null" />.</param>
        /// <returns>The same <paramref name="source"/> if not <see langword="null" />, otherwise an empty sequence of <typeparamref name="T"/>.</returns>
#if NET5_0_OR_GREATER
        [return: NotNull]
#endif
        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T>? source) => source ?? Array.Empty<T>();

        /// <summary>
        /// Returns the items contained in <paramref name="source" /> paginated into chunks of at most <paramref name="pageSize" />.
        /// </summary>
        /// <param name="source">The sequence to paginate in chunks.</param>
        /// <param name="pageSize">The amount of items each chunk will consist of.</param>
        /// <typeparam name="T">The type of items in <paramref name="source"/>.</typeparam>
        /// <returns>A sequence of pages containing the items contained in <paramref name="source" />.</returns>
        public static IEnumerable<IEnumerable<T>> Paginate<T>(this IEnumerable<T> source, int pageSize)
        {
            if (pageSize <= 1)
            {
                throw new ArgumentOutOfRangeException(nameof(pageSize), $"{nameof(pageSize)} should be greater than 1.");
            }

#if NET6_0_OR_GREATER
            return source.EmptyIfNull().Chunk(pageSize);
#else
            if (source is null)
            {
                yield break;
            }

            using var enumerator = source.GetEnumerator();

            while (enumerator.MoveNext())
            {
                var currentPage = new List<T>(pageSize) { enumerator.Current };

                while (currentPage.Count < pageSize && enumerator.MoveNext())
                {
                    currentPage.Add(enumerator.Current);
                }

                yield return new ReadOnlyCollection<T>(currentPage);
            }
#endif
        }

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
#pragma warning restore SA1649
    }
}

#pragma warning disable SA1403
namespace InsightArchitectures.Utilities
{
    /// <inheritdoc cref="InsightArchitecturesEnumerableExtensions"/>
    public static class EnumerableExtensions
    {
        /// <inheritdoc cref="InsightArchitecturesEnumerableExtensions.EmptyIfNull{T}"/>
        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T>? source) =>
            InsightArchitecturesEnumerableExtensions.EmptyIfNull(source);

        /// <inheritdoc cref="InsightArchitecturesEnumerableExtensions.Paginate{T}"/>
#if NET5_0_OR_GREATER
        [return: NotNull]
#endif
        public static IEnumerable<IEnumerable<T>> Paginate<T>(this IEnumerable<T> source, int pageSize) =>
#if NET6_0_OR_GREATER
            source.EmptyIfNull().Chunk(pageSize);
#else
            InsightArchitecturesEnumerableExtensions.Paginate(source, pageSize);
#endif
    }
}

namespace InsightArchitectures.Utilities
{
    /// <summary>
    /// A set of extension methods on the <see cref="IEnumerable{T}"/> type that help detecting distinct instances of objects.
    /// </summary>
    public static class DistinctEnumerableExtensions
    {
        /// <inheritdoc cref="InsightArchitecturesEnumerableExtensions.DistinctBy{T,TValue}"/>
#if NET5_0_OR_GREATER
        [return: NotNull]
#endif
        public static IEnumerable<T> DistinctBy<T, TValue>(this IEnumerable<T>? source, Func<T, TValue> selector, IEqualityComparer<TValue>? equalityComparer = null) =>
#if NET6_0_OR_GREATER
            Enumerable.DistinctBy(source.EmptyIfNull(), selector, equalityComparer);
#else
            InsightArchitecturesEnumerableExtensions.DistinctBy(source, selector, equalityComparer);
#endif
    }
}
#pragma warning restore SA1403
