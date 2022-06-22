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

    /// <summary>
    /// Tries to fetch the value by <paramref name="key"/> from <paramref name="dictionary"/>. If the key is not present, <paramref name="fallbackValue" /> is returned.
    /// </summary>
    /// <param name="dictionary">The dictionary where to get the value.</param>
    /// <param name="key">The key whose value to fetch.</param>
    /// <param name="fallbackValue">The fallback value to be returned if the key is not available.</param>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <returns>The value associated with the specified key or the fallback value.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="dictionary"/> is null.</exception>
    public static TValue GetValueOrFallback<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue fallbackValue)
        where TKey : notnull
    {
        _ = dictionary ?? throw new ArgumentNullException(nameof(dictionary));

        if (!dictionary.TryGetValue(key, out var value))
        {
            return fallbackValue;
        }

        return value;
    }

    /// <summary>
    /// Tries to fetch the value by <paramref name="key"/> from <paramref name="dictionary"/>. If the key is not present, <paramref name="fallbackValue" /> is returned.
    /// </summary>
    /// <param name="dictionary">The dictionary where to get the value.</param>
    /// <param name="key">The key whose value to fetch.</param>
    /// <param name="fallbackValue">The fallback value to be returned if the key is not available.</param>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <returns>The value associated with the specified key or the fallback value.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="dictionary"/> is null.</exception>
    public static TValue GetValueOrFallback<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue fallbackValue)
    {
        _ = dictionary ?? throw new ArgumentNullException(nameof(dictionary));

        if (!dictionary.TryGetValue(key, out var value))
        {
            return fallbackValue;
        }

        return value;
    }

    /// <summary>
    /// Tries to fetch the value by <paramref name="key"/> from <paramref name="dictionary"/>. If the key is not present, <paramref name="fallbackValue" /> is returned.
    /// </summary>
    /// <param name="dictionary">The dictionary where to get the value.</param>
    /// <param name="key">The key whose value to fetch.</param>
    /// <param name="fallbackValue">The fallback value to be returned if the key is not available.</param>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <returns>The value associated with the specified key or the fallback value.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="dictionary"/> is null.</exception>
    public static TValue GetValueOrFallback<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key, TValue fallbackValue)
    {
        _ = dictionary ?? throw new ArgumentNullException(nameof(dictionary));

        if (!dictionary.TryGetValue(key, out var value))
        {
            return fallbackValue;
        }

        return value;
    }

    /// <summary>
    /// Tries to fetch the value by <paramref name="key"/> from <paramref name="dictionary"/>. If the key is not present, the default value of <typeparamref name="TValue" /> is returned.
    /// </summary>
    /// <param name="dictionary">The dictionary where to get the value.</param>
    /// <param name="key">The key whose value to fetch.</param>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <returns>The value associated with the specified key or the default value.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="dictionary"/> is null.</exception>
    public static TValue? GetValueOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
        where TKey : notnull
    {
        _ = dictionary ?? throw new ArgumentNullException(nameof(dictionary));

        if (!dictionary.TryGetValue(key, out var value))
        {
            return default;
        }

        return value;
    }

    /// <summary>
    /// Tries to fetch the value by <paramref name="key"/> from <paramref name="dictionary"/>. If the key is not present, the default value of <typeparamref name="TValue" /> is returned.
    /// </summary>
    /// <param name="dictionary">The dictionary where to get the value.</param>
    /// <param name="key">The key whose value to fetch.</param>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <returns>The value associated with the specified key or the default value.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="dictionary"/> is null.</exception>
    public static TValue? GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
    {
        _ = dictionary ?? throw new ArgumentNullException(nameof(dictionary));

        if (!dictionary.TryGetValue(key, out var value))
        {
            return default;
        }

        return value;
    }

    /// <summary>
    /// Tries to fetch the value by <paramref name="key"/> from <paramref name="dictionary"/>. If the key is not present, the default value of <typeparamref name="TValue" /> is returned.
    /// </summary>
    /// <param name="dictionary">The dictionary where to get the value.</param>
    /// <param name="key">The key whose value to fetch.</param>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <returns>The value associated with the specified key or the default value.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="dictionary"/> is null.</exception>
    public static TValue? GetValueOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key)
    {
        _ = dictionary ?? throw new ArgumentNullException(nameof(dictionary));

        if (!dictionary.TryGetValue(key, out var value))
        {
            return default;
        }

        return value;
    }
}
#pragma warning restore SA1649
