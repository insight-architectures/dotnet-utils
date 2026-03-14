using System;
using System.IO;
using System.Text;
using InsightArchitectures.Utilities.JsonSchema.Internal;
using Microsoft.CodeAnalysis;

namespace InsightArchitectures.Utilities.JsonSchema
{
    /// <summary>
    /// Roslyn incremental source generator that reads <c>.schema.json</c> additional files
    /// and emits a C# record hierarchy based on the JSON Schema definition and the file's path.
    /// </summary>
    /// <remarks>
    /// <para>To use this generator, add the JSON Schema files to your project as additional files:</para>
    /// <code>
    /// &lt;ItemGroup&gt;
    ///   &lt;AdditionalFiles Include="Models\Person.schema.json" /&gt;
    /// &lt;/ItemGroup&gt;
    /// </code>
    /// <para>
    /// The namespace of the generated types is derived from the root namespace of the consuming project
    /// plus any sub-directory path of the file. The class name is derived from the file name
    /// (without the <c>.schema.json</c> extension).
    /// </para>
    /// </remarks>
    [Generator]
    public sealed class JsonSchemaGenerator : IIncrementalGenerator
    {
        private static readonly DiagnosticDescriptor GenerationError = new DiagnosticDescriptor(
            id: "JSSG001",
            title: "JSON Schema generation error",
            messageFormat: "Failed to generate code from '{0}': {1}",
            category: "JsonSchemaGenerator",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        /// <inheritdoc/>
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var schemaFiles = context.AdditionalTextsProvider
                .Where(static f => f.Path.EndsWith(".schema.json", StringComparison.OrdinalIgnoreCase));

            var combined = schemaFiles.Combine(context.AnalyzerConfigOptionsProvider);

            context.RegisterSourceOutput(combined, static (spc, item) =>
            {
                var (additionalText, optionsProvider) = item;

                var text = additionalText.GetText(spc.CancellationToken);
                if (text is null)
                {
                    return;
                }

                optionsProvider.GlobalOptions.TryGetValue("build_property.projectdir", out var projectDir);
                optionsProvider.GlobalOptions.TryGetValue("build_property.rootnamespace", out var rootNamespace);

#pragma warning disable CA1031
                try
                {
                    var source = GenerateSource(
                        additionalText.Path,
                        text.ToString(),
                        projectDir,
                        rootNamespace);

                    var hintName = BuildHintName(additionalText.Path);
                    spc.AddSource(hintName, source);
                }
                catch (Exception ex)
                {
                    spc.ReportDiagnostic(Diagnostic.Create(
                        GenerationError,
                        location: null,
                        additionalText.Path,
                        ex.Message));
                }
#pragma warning restore CA1031
            });
        }

        /// <summary>Parses the JSON schema text and generates a C# source string.</summary>
        internal static string GenerateSource(
            string filePath,
            string jsonText,
            string? projectDir,
            string? rootNamespace)
        {
            var schema = SchemaParser.Parse(jsonText);

            var typeName = GetTypeName(filePath, schema);
            var ns = BuildNamespace(filePath, projectDir, rootNamespace);

            return CodeEmitter.Emit(ns, typeName, schema);
        }

        /// <summary>Derives the root C# type name from the file path and the schema <c>title</c> field.</summary>
        internal static string GetTypeName(string filePath, SchemaNode schema)
        {
            if (!string.IsNullOrWhiteSpace(schema.Title))
            {
                return CodeEmitter.ToPascalCase(schema.Title!);
            }

            var fileName = Path.GetFileName(filePath);
            if (fileName.EndsWith(".schema.json", StringComparison.OrdinalIgnoreCase))
            {
                fileName = fileName.Substring(0, fileName.Length - ".schema.json".Length);
            }
            else
            {
                fileName = Path.GetFileNameWithoutExtension(fileName);
            }

            return CodeEmitter.ToPascalCase(fileName);
        }

        /// <summary>Builds the target C# namespace from the project root namespace and the file's relative directory.</summary>
        internal static string BuildNamespace(
            string filePath,
            string? projectDir,
            string? rootNamespace)
        {
            var relativeDir = GetRelativeDirectory(filePath, projectDir);

            var ns = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(rootNamespace))
            {
                ns.Append(rootNamespace!.Trim());
            }

            if (!string.IsNullOrWhiteSpace(relativeDir))
            {
                var dirPart = relativeDir!
                    .Replace(Path.DirectorySeparatorChar, '.')
                    .Replace('/', '.')
                    .Trim('.');

                if (dirPart.Length > 0)
                {
                    if (ns.Length > 0)
                    {
                        ns.Append('.');
                    }

                    ns.Append(dirPart);
                }
            }

            return ns.ToString();
        }

        private static string? GetRelativeDirectory(string filePath, string? projectDir)
        {
            var dir = Path.GetDirectoryName(filePath);
            if (dir is null)
            {
                return null;
            }

            if (!string.IsNullOrEmpty(projectDir))
            {
                var normalizedProject = projectDir!.TrimEnd(Path.DirectorySeparatorChar, '/');
                if (dir.StartsWith(normalizedProject, StringComparison.OrdinalIgnoreCase))
                {
                    dir = dir.Substring(normalizedProject.Length).TrimStart(Path.DirectorySeparatorChar, '/');
                }
            }

            return string.IsNullOrEmpty(dir) ? null : dir;
        }

        private static string BuildHintName(string filePath)
        {
            var fileName = Path.GetFileName(filePath);
            if (fileName.EndsWith(".schema.json", StringComparison.OrdinalIgnoreCase))
            {
                fileName = fileName.Substring(0, fileName.Length - ".schema.json".Length);
            }
            else
            {
                fileName = Path.GetFileNameWithoutExtension(fileName);
            }

            return fileName.Replace('.', '_') + ".g.cs";
        }
    }
}
