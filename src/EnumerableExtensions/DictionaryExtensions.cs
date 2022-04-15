using InsightArchitectures.Utilities;

// ReSharper disable once CheckNamespace
namespace System.Collections.Generic;

#pragma warning disable SA1649
/// <summary>
/// A set of extension methods on the <see cref="IDictionary{TKey,TValue}"/> type.
/// </summary>
public static class InsightArchitecturesDictionaryExtensions
{
    /// <summary>
    /// Adds the items contained in <paramref name="items"/> to the hash table <paramref name="dictionary"/>.
    /// This extensions makes it possible to use a collection initializer with sequences of items.
    /// <code>
    /// var dictionary = new Dictionary&lt;string, string&gt;
    /// {
    ///     "Foo",
    ///     "Bar",
    ///     new Dictionary&lt;string, string&gt;
    ///     {
    ///         ["Hello"] = "World",
    ///         ["World"] = "Hello"
    ///     }
    /// };
    /// </code>
    /// </summary>
    /// <param name="dictionary">The hash table where items will be added.</param>
    /// <param name="items">The set of items to add to the hash table.</param>
    /// <typeparam name="TKey">The type of the key of the hash table.</typeparam>
    /// <typeparam name="TValue">The type of the items of the hash table.</typeparam>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="dictionary"/> is null.</exception>
    public static void Add<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IEnumerable<KeyValuePair<TKey, TValue>> items)
    {
        _ = dictionary ?? throw new ArgumentNullException(nameof(dictionary));

        AddRange(dictionary, items);
    }

    /// <summary>
    /// Adds the items contained in <paramref name="items"/> to the hash table <paramref name="dictionary"/>.
    /// </summary>
    /// <param name="dictionary">The hash table where items will be added.</param>
    /// <param name="items">The set of items to add to the hash table.</param>
    /// <typeparam name="TKey">The type of the key of the hash table.</typeparam>
    /// <typeparam name="TValue">The type of the items of the hash table.</typeparam>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="dictionary"/> is null.</exception>
    public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IEnumerable<KeyValuePair<TKey, TValue>> items)
    {
        _ = dictionary ?? throw new ArgumentNullException(nameof(dictionary));

        foreach (var item in items.EmptyIfNull())
        {
            dictionary.Add(item.Key, item.Value);
        }
    }
}
#pragma warning restore SA1649
