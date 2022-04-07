using System;
using System.Collections.Generic;

namespace InsightArchitectures.Utilities;

/// <summary>
/// A set of extension methods on the <see cref="IList{T}"/> type.
/// </summary>
public static class ListExtensions
{
    /// <summary>
    /// Adds the items contained in <paramref name="items"/> to the list <paramref name="list"/>.
    /// This extensions makes it possible to use a collection initializer with sequences of items.
    /// <code>
    /// var list = new List&lt;string&gt;
    /// {
    ///     "Foo",
    ///     "Bar",
    ///     new[] { "Hello", "World" }
    /// };
    /// </code>
    /// </summary>
    /// <param name="list">The list where items will be added.</param>
    /// <param name="items">The set of items to add to the list.</param>
    /// <typeparam name="T">The type of the list and its items.</typeparam>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="list"/> is null.</exception>
    public static void Add<T>(this IList<T> list, IEnumerable<T> items)
    {
        _ = list ?? throw new ArgumentNullException(nameof(list));

        AddRange(list, items);
    }

    /// <summary>
    /// Adds the items contained in <paramref name="items"/> to the list <paramref name="list"/>.
    /// </summary>
    /// <param name="list">The list where items will be added.</param>
    /// <param name="items">The set of items to add to the list.</param>
    /// <typeparam name="T">The type of the list and its items.</typeparam>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="list"/> is null.</exception>
    public static void AddRange<T>(this IList<T> list, IEnumerable<T> items)
    {
        _ = list ?? throw new ArgumentNullException(nameof(list));
        
        foreach (var item in items.EmptyIfNull())
        {
            list.Add(item);
        }
    }
}
