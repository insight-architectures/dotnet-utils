using InsightArchitectures.Utilities;

// ReSharper disable once CheckNamespace
namespace System.Collections.Generic;

#pragma warning disable SA1649
/// <summary>
/// A set of extension methods on the <see cref="ICollection{T}"/> type.
/// </summary>
public static class InsightArchitecturesCollectionExtensions
{
    /// <summary>
    /// Adds the items contained in <paramref name="items"/> to the collection <paramref name="collection"/>.
    /// This extensions makes it possible to use a collection initializer with sequences of items.
    /// <code>
    /// var collection = new List&lt;string&gt;
    /// {
    ///     "Foo",
    ///     "Bar",
    ///     new[] { "Hello", "World" }
    /// };
    /// </code>
    /// </summary>
    /// <param name="collection">The collection where items will be added.</param>
    /// <param name="items">The set of items to add to the collection.</param>
    /// <typeparam name="T">The type of the items of the collection.</typeparam>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="collection"/> is null.</exception>
    public static void Add<T>(this ICollection<T> collection, IEnumerable<T> items)
    {
        _ = collection ?? throw new ArgumentNullException(nameof(collection));

        AddRange(collection, items);
    }

    /// <summary>
    /// Adds the items contained in <paramref name="items"/> to the collection <paramref name="collection"/>.
    /// </summary>
    /// <param name="collection">The collection where items will be added.</param>
    /// <param name="items">The set of items to add to the collection.</param>
    /// <typeparam name="T">The type of the items of the collection.</typeparam>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="collection"/> is null.</exception>
    public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
    {
        _ = collection ?? throw new ArgumentNullException(nameof(collection));

        foreach (var item in items.EmptyIfNull())
        {
            collection.Add(item);
        }
    }
}
#pragma warning restore SA1649
