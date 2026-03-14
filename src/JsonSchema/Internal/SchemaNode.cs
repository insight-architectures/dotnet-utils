using System.Collections.Generic;

namespace InsightArchitectures.Utilities.JsonSchema.Internal
{
    /// <summary>
    /// Represents a parsed JSON Schema node.
    /// </summary>
    internal sealed class SchemaNode
    {
        /// <summary>Gets or sets the JSON Schema type (e.g. "string", "integer", "object", "array").</summary>
        public string? Type { get; set; }

        /// <summary>Gets or sets the JSON Schema format hint (e.g. "date-time", "uuid").</summary>
        public string? Format { get; set; }

        /// <summary>Gets or sets the title, used as the generated type name when set.</summary>
        public string? Title { get; set; }

        /// <summary>Gets or sets the description of the schema node.</summary>
        public string? Description { get; set; }

        /// <summary>Gets the list of required property names.</summary>
        public List<string> Required { get; } = new List<string>();

        /// <summary>Gets the map of property name to child schema node.</summary>
        public Dictionary<string, SchemaNode> Properties { get; } = new Dictionary<string, SchemaNode>();

        /// <summary>Gets or sets the schema for array items.</summary>
        public SchemaNode? Items { get; set; }

        /// <summary>
        /// Gets or sets the schema for dictionary values when additionalProperties is an object schema.
        /// Null means the feature is not used; use <see cref="AdditionalPropertiesAllowed"/> to distinguish.
        /// </summary>
        public SchemaNode? AdditionalProperties { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether additional properties are allowed (true) or disallowed (false).
        /// Only meaningful when <see cref="AdditionalProperties"/> is null.
        /// </summary>
        public bool AdditionalPropertiesAllowed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the type is nullable (i.e. the JSON type array includes "null").
        /// </summary>
        public bool IsNullableType { get; set; }
    }
}
