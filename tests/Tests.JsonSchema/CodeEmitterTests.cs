using InsightArchitectures.Utilities.JsonSchema.Internal;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    [TestOf(typeof(CodeEmitter))]
    public class CodeEmitterTests
    {
        [TestCase("firstName", "FirstName")]
        [TestCase("first_name", "FirstName")]
        [TestCase("first-name", "FirstName")]
        [TestCase("name", "Name")]
        [TestCase("", "")]
        public void ToPascalCase_converts_correctly(string input, string expected)
        {
            Assert.That(CodeEmitter.ToPascalCase(input), Is.EqualTo(expected));
        }

        [Test]
        public void Emit_generates_record_with_namespace()
        {
            var schema = new SchemaNode { Type = "object" };
            schema.Properties.Add("id", new SchemaNode { Type = "string", Format = "uuid" });
            schema.Properties.Add("name", new SchemaNode { Type = "string" });
            schema.Required.Add("id");
            schema.Required.Add("name");

            var source = CodeEmitter.Emit("My.Namespace", "Person", schema);

            Assert.That(source, Does.Contain("namespace My.Namespace"));
            Assert.That(source, Does.Contain("public record Person("));
            Assert.That(source, Does.Contain("global::System.Guid Id"));
            Assert.That(source, Does.Contain("string Name"));
            Assert.That(source, Does.Contain("#nullable enable"));
        }

        [Test]
        public void Emit_makes_optional_properties_nullable()
        {
            var schema = new SchemaNode { Type = "object" };
            schema.Properties.Add("required_prop", new SchemaNode { Type = "string" });
            schema.Properties.Add("optional_prop", new SchemaNode { Type = "integer" });
            schema.Required.Add("required_prop");

            var source = CodeEmitter.Emit("NS", "Item", schema);

            Assert.That(source, Does.Contain("string RequiredProp"));
            Assert.That(source, Does.Contain("int? OptionalProp"));
        }

        [Test]
        public void Emit_generates_array_property()
        {
            var schema = new SchemaNode { Type = "object" };
            schema.Properties.Add("scores", new SchemaNode
            {
                Type = "array",
                Items = new SchemaNode { Type = "number" },
            });

            var source = CodeEmitter.Emit("NS", "Result", schema);

            Assert.That(source, Does.Contain("double[]? Scores"));
        }

        [Test]
        public void Emit_generates_dictionary_for_additional_properties()
        {
            var schema = new SchemaNode { Type = "object" };
            schema.Properties.Add("meta", new SchemaNode
            {
                Type = "object",
                AdditionalProperties = new SchemaNode { Type = "string" },
            });

            var source = CodeEmitter.Emit("NS", "Doc", schema);

            Assert.That(source, Does.Contain("global::System.Collections.Generic.Dictionary<string, string>? Meta"));
        }

        [Test]
        public void Emit_generates_nested_record_for_object_property()
        {
            var addressSchema = new SchemaNode { Type = "object" };
            addressSchema.Properties.Add("street", new SchemaNode { Type = "string" });
            addressSchema.Properties.Add("city", new SchemaNode { Type = "string" });
            addressSchema.Required.Add("street");

            var schema = new SchemaNode { Type = "object" };
            schema.Properties.Add("address", addressSchema);
            schema.Required.Add("address");

            var source = CodeEmitter.Emit("NS", "Person", schema);

            Assert.That(source, Does.Contain("PersonAddress Address"));
            Assert.That(source, Does.Contain("public record PersonAddress("));
            Assert.That(source, Does.Contain("string Street"));
            Assert.That(source, Does.Contain("string? City"));
        }

        [Test]
        public void Emit_uses_title_as_type_name_in_description()
        {
            var schema = new SchemaNode
            {
                Type = "object",
                Description = "A simple item.",
            };

            var source = CodeEmitter.Emit("NS", "MyItem", schema);

            Assert.That(source, Does.Contain("/// <summary>A simple item.</summary>"));
        }

        [Test]
        public void Emit_maps_date_time_format()
        {
            var schema = new SchemaNode { Type = "object" };
            schema.Properties.Add("created_at", new SchemaNode { Type = "string", Format = "date-time" });
            schema.Required.Add("created_at");

            var source = CodeEmitter.Emit("NS", "Event", schema);

            Assert.That(source, Does.Contain("global::System.DateTimeOffset CreatedAt"));
        }

        [Test]
        public void Emit_maps_integer_type()
        {
            var schema = new SchemaNode { Type = "object" };
            schema.Properties.Add("count", new SchemaNode { Type = "integer" });
            schema.Properties.Add("big", new SchemaNode { Type = "integer", Format = "int64" });

            var source = CodeEmitter.Emit("NS", "Counter", schema);

            Assert.That(source, Does.Contain("int? Count"));
            Assert.That(source, Does.Contain("long? Big"));
        }

        [Test]
        public void Emit_maps_boolean_type()
        {
            var schema = new SchemaNode { Type = "object" };
            schema.Properties.Add("active", new SchemaNode { Type = "boolean" });

            var source = CodeEmitter.Emit("NS", "Entity", schema);

            Assert.That(source, Does.Contain("bool? Active"));
        }

        [Test]
        public void Emit_marks_nullable_type_array_property_as_nullable()
        {
            var schema = new SchemaNode { Type = "object" };
            var propSchema = new SchemaNode { Type = "string", IsNullableType = true };
            schema.Properties.Add("name", propSchema);
            schema.Required.Add("name");

            var source = CodeEmitter.Emit("NS", "Thing", schema);

            Assert.That(source, Does.Contain("string? Name"));
        }

        [Test]
        public void Emit_empty_schema_generates_empty_record()
        {
            var schema = new SchemaNode();

            var source = CodeEmitter.Emit("NS", "Empty", schema);

            Assert.That(source, Does.Contain("public record Empty();"));
        }
    }
}
