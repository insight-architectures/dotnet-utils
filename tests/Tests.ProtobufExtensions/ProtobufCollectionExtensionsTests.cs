using System.Collections.Generic;
using Google.Protobuf.Collections;
using InsightArchitectures.Utilities;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class ProtobufCollectionExtensionsTests
    {
        [Test, CustomAutoData]
        public void MapField_Add_throws_if_map_is_null(IReadOnlyDictionary<string, string> items)
        {
            Assert.That(() => ProtobufCollectionExtensions.Add(null!, items), Throws.ArgumentNullException);
        }

        [Test, CustomAutoData]
        public void MapField_Add_does_not_throw_if_items_is_null(MapField<string, string> map)
        {
            Assert.That(() => ProtobufCollectionExtensions.Add(map, null), Throws.Nothing);
        }

        [Test, CustomAutoData]
        public void MapField_Add_adds_items_to_map(MapField<string, string> map, IReadOnlyDictionary<string, string> items)
        {
            ProtobufCollectionExtensions.Add(map, items);

            Assert.That(map, Is.SupersetOf(items));
        }
    }
}
