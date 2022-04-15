using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

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

        /// <summary>
        /// Returns the items contained in <paramref name="source" /> paginated into chunks of at most <paramref name="pageSize" />.
        /// </summary>
        /// <param name="source">The sequence to paginate in chunks.</param>
        /// <param name="pageSize">The amount of items each chunk will consist of.</param>
        /// <typeparam name="T">The type of items in <paramref name="source"/>.</typeparam>
        /// <returns>A sequence of pages containing the items contained in <paramref name="source" />.</returns>
        public static IEnumerable<IEnumerable<T>> Paginate<T>(this IEnumerable<T> source, int pageSize)
        {
            if (pageSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(pageSize), $"{nameof(pageSize)} should be greater than 0.");
            }

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
        }
    }
}
