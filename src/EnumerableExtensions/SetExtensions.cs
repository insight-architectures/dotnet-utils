using InsightArchitectures.Utilities;

// ReSharper disable once CheckNamespace
namespace System.Collections.Generic;

#pragma warning disable SA1649
/// <summary>
/// A set of extension methods on the <see cref="ISet{T}"/> type.
/// </summary>
public static class InsightArchitecturesSetExtensions
{
    /// <summary>
    /// Adds the items contained in <paramref name="items"/> to the set <paramref name="set"/>.
    /// This extensions makes it possible to use a collection initializer with sequences of items.
    /// <code>
    /// var set = new HashSet&lt;string&gt;
    /// {
    ///     "Foo",
    ///     "Bar",
    ///     new[] { "Hello", "World" }
    /// };
    /// </code>
    /// </summary>
    /// <param name="set">The set where items will be added.</param>
    /// <param name="items">The set of items to add to the set.</param>
    /// <typeparam name="T">The type of the items of the set.</typeparam>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="set"/> is null.</exception>
    public static void Add<T>(this ISet<T> set, IEnumerable<T> items)
    {
        _ = set ?? throw new ArgumentNullException(nameof(set));

        AddRange(set, items);
    }

    /// <summary>
    /// Adds the items contained in <paramref name="items"/> to the set <paramref name="set"/>.
    /// </summary>
    /// <param name="set">The set where items will be added.</param>
    /// <param name="items">The set of items to add to the set.</param>
    /// <typeparam name="T">The type of the items of the set.</typeparam>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="set"/> is null.</exception>
    public static void AddRange<T>(this ISet<T> set, IEnumerable<T> items)
    {
        _ = set ?? throw new ArgumentNullException(nameof(set));

        foreach (var item in items.EmptyIfNull())
        {
            set.Add(item);
        }
    }
}
#pragma warning restore SA1649
