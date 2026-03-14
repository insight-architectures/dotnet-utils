using InsightArchitectures.Utilities.JsonSchema.Internal;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    [TestOf(typeof(SchemaParser))]
    public class SchemaParserTests
    {
        [Test]
        public void Parse_empty_schema_returns_empty_node()
        {
            var node = SchemaParser.Parse("{}");

            Assert.That(node.Type, Is.Null);
            Assert.That(node.Properties, Is.Empty);
            Assert.That(node.Required, Is.Empty);
        }

        [Test]
        public void Parse_type_string_field()
        {
            var node = SchemaParser.Parse(@"{ ""type"": ""string"" }");

            Assert.That(node.Type, Is.EqualTo("string"));
        }

        [Test]
        public void Parse_type_with_format()
        {
            var node = SchemaParser.Parse(@"{ ""type"": ""string"", ""format"": ""date-time"" }");

            Assert.That(node.Type, Is.EqualTo("string"));
            Assert.That(node.Format, Is.EqualTo("date-time"));
        }

        [Test]
        public void Parse_title_and_description()
        {
            var node = SchemaParser.Parse(@"{ ""title"": ""Person"", ""description"": ""A person schema"" }");

            Assert.That(node.Title, Is.EqualTo("Person"));
            Assert.That(node.Description, Is.EqualTo("A person schema"));
        }

        [Test]
        public void Parse_required_list()
        {
            var node = SchemaParser.Parse(@"{ ""required"": [""id"", ""name""] }");

            Assert.That(node.Required, Is.EquivalentTo(new[] { "id", "name" }));
        }

        [Test]
        public void Parse_object_with_properties()
        {
            var json = @"{
                ""type"": ""object"",
                ""properties"": {
                    ""id"": { ""type"": ""string"" },
                    ""count"": { ""type"": ""integer"" }
                }
            }";

            var node = SchemaParser.Parse(json);

            Assert.That(node.Type, Is.EqualTo("object"));
            Assert.That(node.Properties, Has.Count.EqualTo(2));
            Assert.That(node.Properties["id"].Type, Is.EqualTo("string"));
            Assert.That(node.Properties["count"].Type, Is.EqualTo("integer"));
        }

        [Test]
        public void Parse_array_with_items()
        {
            var json = @"{ ""type"": ""array"", ""items"": { ""type"": ""number"" } }";

            var node = SchemaParser.Parse(json);

            Assert.That(node.Type, Is.EqualTo("array"));
            Assert.That(node.Items, Is.Not.Null);
            Assert.That(node.Items!.Type, Is.EqualTo("number"));
        }

        [Test]
        public void Parse_additional_properties_as_schema()
        {
            var json = @"{ ""type"": ""object"", ""additionalProperties"": { ""type"": ""string"" } }";

            var node = SchemaParser.Parse(json);

            Assert.That(node.AdditionalProperties, Is.Not.Null);
            Assert.That(node.AdditionalProperties!.Type, Is.EqualTo("string"));
        }

        [Test]
        public void Parse_additional_properties_as_false_bool()
        {
            var json = @"{ ""type"": ""object"", ""additionalProperties"": false }";

            var node = SchemaParser.Parse(json);

            Assert.That(node.AdditionalProperties, Is.Null);
            Assert.That(node.AdditionalPropertiesAllowed, Is.False);
        }

        [Test]
        public void Parse_nullable_type_array()
        {
            var json = @"{ ""type"": [""string"", ""null""] }";

            var node = SchemaParser.Parse(json);

            Assert.That(node.Type, Is.EqualTo("string"));
            Assert.That(node.IsNullableType, Is.True);
        }

        [Test]
        public void Parse_ignores_unknown_fields()
        {
            var json = @"{ ""type"": ""string"", ""\$schema"": ""http://json-schema.org/draft-07/schema"" }";

            Assert.DoesNotThrow(() => SchemaParser.Parse(json));
        }

        [Test]
        public void Parse_empty_or_whitespace_returns_empty_node()
        {
            Assert.That(SchemaParser.Parse(string.Empty).Type, Is.Null);
            Assert.That(SchemaParser.Parse("  ").Type, Is.Null);
        }
    }
}
