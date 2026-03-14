using InsightArchitectures.Utilities.JsonSchema;
using InsightArchitectures.Utilities.JsonSchema.Internal;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    [TestOf(typeof(JsonSchemaGenerator))]
    public class JsonSchemaGeneratorTests
    {
        private const string SimplePersonSchema = @"{
            ""type"": ""object"",
            ""required"": [""id"", ""name""],
            ""properties"": {
                ""id"":   { ""type"": ""string"", ""format"": ""uuid"" },
                ""name"": { ""type"": ""string"" },
                ""age"":  { ""type"": ""integer"" }
            }
        }";

        // ── GetTypeName ──────────────────────────────────────────────────

        [Test]
        public void GetTypeName_uses_file_name_when_title_absent()
        {
            var schema = new SchemaNode();
            var typeName = JsonSchemaGenerator.GetTypeName("/project/Models/Person.schema.json", schema);
            Assert.That(typeName, Is.EqualTo("Person"));
        }

        [Test]
        public void GetTypeName_uses_title_from_schema_when_present()
        {
            var schema = new SchemaNode { Title = "my_event" };
            var typeName = JsonSchemaGenerator.GetTypeName("/project/some-file.schema.json", schema);
            Assert.That(typeName, Is.EqualTo("MyEvent"));
        }

        [Test]
        public void GetTypeName_handles_plain_json_extension()
        {
            var schema = new SchemaNode();
            var typeName = JsonSchemaGenerator.GetTypeName("/project/Order.json", schema);
            Assert.That(typeName, Is.EqualTo("Order"));
        }

        // ── BuildNamespace ───────────────────────────────────────────────

        [Test]
        public void BuildNamespace_with_root_namespace_and_no_subdir()
        {
            var ns = JsonSchemaGenerator.BuildNamespace(
                "/project/Person.schema.json",
                projectDir: "/project/",
                rootNamespace: "My.App");

            Assert.That(ns, Is.EqualTo("My.App"));
        }

        [Test]
        public void BuildNamespace_with_root_namespace_and_subdir()
        {
            var ns = JsonSchemaGenerator.BuildNamespace(
                "/project/Models/Person.schema.json",
                projectDir: "/project/",
                rootNamespace: "My.App");

            Assert.That(ns, Is.EqualTo("My.App.Models"));
        }

        [Test]
        public void BuildNamespace_with_nested_subdir()
        {
            var ns = JsonSchemaGenerator.BuildNamespace(
                "/project/Models/User/Address.schema.json",
                projectDir: "/project/",
                rootNamespace: "My.App");

            Assert.That(ns, Is.EqualTo("My.App.Models.User"));
        }

        [Test]
        public void BuildNamespace_without_root_namespace_uses_dir_only()
        {
            var ns = JsonSchemaGenerator.BuildNamespace(
                "/project/Models/Person.schema.json",
                projectDir: "/project/",
                rootNamespace: null);

            Assert.That(ns, Is.EqualTo("Models"));
        }

        [Test]
        public void BuildNamespace_without_project_dir_keeps_full_relative_path()
        {
            var ns = JsonSchemaGenerator.BuildNamespace(
                "C:/project/Models/Person.schema.json",
                projectDir: null,
                rootNamespace: "My.App");

            Assert.That(ns, Does.Contain("My.App"));
        }

        // ── GenerateSource ───────────────────────────────────────────────

        [Test]
        public void GenerateSource_produces_valid_record_for_simple_schema()
        {
            var source = JsonSchemaGenerator.GenerateSource(
                "/project/Models/Person.schema.json",
                SimplePersonSchema,
                projectDir: "/project/",
                rootNamespace: "My.App");

            Assert.That(source, Does.Contain("namespace My.App.Models"));
            Assert.That(source, Does.Contain("public record Person("));
            Assert.That(source, Does.Contain("global::System.Guid Id"));
            Assert.That(source, Does.Contain("string Name"));
            Assert.That(source, Does.Contain("int? Age"));
        }

        [Test]
        public void GenerateSource_nested_object_produces_additional_record()
        {
            var json = @"{
                ""type"": ""object"",
                ""required"": [""address""],
                ""properties"": {
                    ""address"": {
                        ""type"": ""object"",
                        ""required"": [""street""],
                        ""properties"": {
                            ""street"": { ""type"": ""string"" },
                            ""city"":   { ""type"": ""string"" }
                        }
                    }
                }
            }";

            var source = JsonSchemaGenerator.GenerateSource(
                "/project/Order.schema.json",
                json,
                projectDir: "/project/",
                rootNamespace: "Foo");

            Assert.That(source, Does.Contain("public record Order("));
            Assert.That(source, Does.Contain("OrderAddress Address"));
            Assert.That(source, Does.Contain("public record OrderAddress("));
            Assert.That(source, Does.Contain("string Street"));
            Assert.That(source, Does.Contain("string? City"));
        }

        [Test]
        public void GenerateSource_array_of_strings()
        {
            var json = @"{
                ""type"": ""object"",
                ""properties"": {
                    ""tags"": { ""type"": ""array"", ""items"": { ""type"": ""string"" } }
                }
            }";

            var source = JsonSchemaGenerator.GenerateSource(
                "/project/Doc.schema.json",
                json,
                projectDir: "/project/",
                rootNamespace: "NS");

            Assert.That(source, Does.Contain("string[]? Tags"));
        }

        [Test]
        public void GenerateSource_dictionary_additional_properties()
        {
            var json = @"{
                ""type"": ""object"",
                ""properties"": {
                    ""meta"": {
                        ""type"": ""object"",
                        ""additionalProperties"": { ""type"": ""string"" }
                    }
                }
            }";

            var source = JsonSchemaGenerator.GenerateSource(
                "/project/Doc.schema.json",
                json,
                projectDir: "/project/",
                rootNamespace: "NS");

            Assert.That(source, Does.Contain("global::System.Collections.Generic.Dictionary<string, string>? Meta"));
        }

        [Test]
        public void GenerateSource_nullable_type_array_makes_property_nullable()
        {
            var json = @"{
                ""type"": ""object"",
                ""required"": [""name""],
                ""properties"": {
                    ""name"": { ""type"": [""string"", ""null""] }
                }
            }";

            var source = JsonSchemaGenerator.GenerateSource(
                "/project/Item.schema.json",
                json,
                projectDir: "/project/",
                rootNamespace: "NS");

            Assert.That(source, Does.Contain("string? Name"));
        }

        // ── End-to-end via Roslyn driver ─────────────────────────────────

        [Test]
        public void Generator_driver_produces_output_file()
        {
            var results = GeneratorTestHelper.RunGenerator(
                ("/project/Models/Person.schema.json", SimplePersonSchema));

            Assert.That(results, Has.Count.EqualTo(1));
            Assert.That(results[0], Does.Contain("public record Person("));
        }

        [Test]
        public void Generator_driver_ignores_non_schema_files()
        {
            var results = GeneratorTestHelper.RunGenerator(
                ("/project/config.json", "{}"));

            Assert.That(results, Is.Empty);
        }

        [Test]
        public void Generator_driver_handles_multiple_schema_files()
        {
            var schema2 = @"{""type"": ""object"", ""properties"": {""value"": {""type"": ""number""}}}";

            var results = GeneratorTestHelper.RunGenerator(
                ("/project/Person.schema.json", SimplePersonSchema),
                ("/project/Metric.schema.json", schema2));

            Assert.That(results, Has.Count.EqualTo(2));
        }
    }
}
