using System;
using System.Collections.Generic;
using Google.Protobuf.Collections;

namespace InsightArchitectures.Utilities
{
    /// <summary>
    /// This class contains a set of extension methods for collections contained in Google.Protobuf.
    /// </summary>
    public static class ProtobufCollectionExtensions
    {
        /// <summary>
        /// Adds all the items contained in <paramref name="items"/> to the given <see cref="MapField{TKey,TValue}" />.
        /// </summary>
        /// <param name="map">The collections items should be added to.</param>
        /// <param name="items">The items to add to the collection.</param>
        /// <typeparam name="TKey">The type of the key of the map.</typeparam>
        /// <typeparam name="TValue">The type of the values of the map.</typeparam>
        public static void Add<TKey, TValue>(this MapField<TKey, TValue> map, IReadOnlyDictionary<TKey, TValue> items)
        {
            _ = map ?? throw new ArgumentNullException(nameof(map));

            if (items is null)
            {
                return;
            }

            foreach (var item in items)
            {
                map.Add(item.Key, item.Value);
            }
        }
    }
}
