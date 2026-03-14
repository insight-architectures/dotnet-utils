using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using InsightArchitectures.Utilities.JsonSchema;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace Tests
{
    internal static class GeneratorTestHelper
    {
        /// <summary>
        /// Runs the <see cref="JsonSchemaGenerator"/> against a set of additional text files
        /// and returns the list of generated source texts.
        /// </summary>
        internal static IReadOnlyList<string> RunGenerator(
            params (string Path, string Content)[] additionalFiles)
        {
            var compilation = CreateCompilation();

            var generator = new JsonSchemaGenerator();
            var additionalTexts = additionalFiles
                .Select(f => (AdditionalText)new InMemoryAdditionalText(f.Path, f.Content))
                .ToImmutableArray();

            var driver = CSharpGeneratorDriver
                .Create(generator)
                .AddAdditionalTexts(additionalTexts)
                .RunGenerators(compilation);

            var result = driver.GetRunResult();
            return result.GeneratedTrees
                .Select(t => t.GetText().ToString())
                .ToList();
        }

        private static CSharpCompilation CreateCompilation()
        {
            return CSharpCompilation.Create(
                assemblyName: "TestAssembly",
                references: new[]
                {
                    MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                });
        }
    }

    /// <summary>Simple in-memory implementation of <see cref="AdditionalText"/>.</summary>
    internal sealed class InMemoryAdditionalText : AdditionalText
    {
        private readonly string _text;

        public InMemoryAdditionalText(string path, string text)
        {
            Path = path;
            _text = text;
        }

        public override string Path { get; }

        public override SourceText? GetText(
            System.Threading.CancellationToken cancellationToken = default) =>
            SourceText.From(_text);
    }
}
