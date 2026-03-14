using System;
using System.Collections.Generic;

namespace InsightArchitectures.Utilities.JsonSchema.Internal
{
    /// <summary>
    /// Minimal recursive-descent parser for JSON Schema documents.
    /// Only the properties relevant to code generation are extracted.
    /// </summary>
    internal sealed class SchemaParser
    {
        private readonly string _json;
        private int _pos;

        private SchemaParser(string json)
        {
            _json = json;
            _pos = 0;
        }

        /// <summary>Parses a JSON Schema string and returns the root <see cref="SchemaNode"/>.</summary>
        public static SchemaNode Parse(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return new SchemaNode();
            }

            var parser = new SchemaParser(json);
            parser.SkipWhitespace();
            return parser.ParseSchemaObject();
        }

        private char Current() => _pos < _json.Length ? _json[_pos] : '\0';

        private void SkipWhitespace()
        {
            while (_pos < _json.Length && char.IsWhiteSpace(_json[_pos]))
            {
                _pos++;
            }
        }

        private void Expect(char c)
        {
            SkipWhitespace();
            if (_pos >= _json.Length || _json[_pos] != c)
            {
                throw new FormatException(
                    $"Expected '{c}' at position {_pos} but found '{Current()}'.");
            }

            _pos++;
        }

        private string ParseString()
        {
            Expect('"');
            var sb = new System.Text.StringBuilder();

            while (_pos < _json.Length && _json[_pos] != '"')
            {
                if (_json[_pos] == '\\')
                {
                    _pos++; // skip backslash
                    if (_pos < _json.Length)
                    {
                        switch (_json[_pos])
                        {
                            case '"': sb.Append('"'); break;
                            case '\\': sb.Append('\\'); break;
                            case '/': sb.Append('/'); break;
                            case 'n': sb.Append('\n'); break;
                            case 'r': sb.Append('\r'); break;
                            case 't': sb.Append('\t'); break;
                            default: sb.Append(_json[_pos]); break;
                        }

                        _pos++;
                    }
                }
                else
                {
                    sb.Append(_json[_pos]);
                    _pos++;
                }
            }

            Expect('"');
            return sb.ToString();
        }

        private bool ParseBool()
        {
            SkipWhitespace();
            if (_pos + 4 <= _json.Length && _json.Substring(_pos, 4) == "true")
            {
                _pos += 4;
                return true;
            }

            if (_pos + 5 <= _json.Length && _json.Substring(_pos, 5) == "false")
            {
                _pos += 5;
                return false;
            }

            throw new FormatException($"Expected boolean at position {_pos}.");
        }

        private List<string> ParseStringArray()
        {
            var list = new List<string>();
            Expect('[');
            SkipWhitespace();

            if (Current() == ']')
            {
                _pos++;
                return list;
            }

            while (true)
            {
                SkipWhitespace();
                list.Add(ParseString());
                SkipWhitespace();

                if (Current() == ']')
                {
                    _pos++;
                    break;
                }

                Expect(',');
            }

            return list;
        }

        /// <summary>Skips any JSON value (string, number, object, array, bool, null).</summary>
        private void SkipValue()
        {
            SkipWhitespace();
            var c = Current();

            if (c == '"')
            {
                ParseString(); // read and discard
            }
            else if (c == '{')
            {
                SkipObject();
            }
            else if (c == '[')
            {
                SkipArray();
            }
            else if (c == 't' || c == 'f')
            {
                // bool
                ParseBool();
            }
            else if (c == 'n')
            {
                // null
                _pos += 4;
            }
            else
            {
                // number – read until delimiter
                while (_pos < _json.Length)
                {
                    var ch = _json[_pos];
                    if (ch == ',' || ch == '}' || ch == ']' || char.IsWhiteSpace(ch))
                    {
                        break;
                    }

                    _pos++;
                }
            }
        }

        private void SkipObject()
        {
            Expect('{');
            SkipWhitespace();

            if (Current() == '}')
            {
                _pos++;
                return;
            }

            while (true)
            {
                SkipWhitespace();
                ParseString(); // key
                SkipWhitespace();
                Expect(':');
                SkipValue();
                SkipWhitespace();

                if (Current() == '}')
                {
                    _pos++;
                    break;
                }

                Expect(',');
            }
        }

        private void SkipArray()
        {
            Expect('[');
            SkipWhitespace();

            if (Current() == ']')
            {
                _pos++;
                return;
            }

            while (true)
            {
                SkipValue();
                SkipWhitespace();

                if (Current() == ']')
                {
                    _pos++;
                    break;
                }

                Expect(',');
            }
        }

        /// <summary>Parses a JSON object as a <see cref="SchemaNode"/>.</summary>
        private SchemaNode ParseSchemaObject()
        {
            var node = new SchemaNode();
            Expect('{');
            SkipWhitespace();

            if (Current() == '}')
            {
                _pos++;
                return node;
            }

            while (true)
            {
                SkipWhitespace();
                var key = ParseString();
                SkipWhitespace();
                Expect(':');
                SkipWhitespace();

                switch (key)
                {
                    case "type":
                        ParseTypeValue(node);
                        break;

                    case "format":
                        node.Format = ParseString();
                        break;

                    case "title":
                        node.Title = ParseString();
                        break;

                    case "description":
                        node.Description = ParseString();
                        break;

                    case "required":
                        foreach (var r in ParseStringArray())
                        {
                            node.Required.Add(r);
                        }

                        break;

                    case "properties":
                        ParsePropertiesObject(node);
                        break;

                    case "items":
                        node.Items = Current() == '{' ? ParseSchemaObject() : null;
                        if (node.Items == null)
                        {
                            SkipValue();
                        }

                        break;

                    case "additionalProperties":
                        if (Current() == '{')
                        {
                            node.AdditionalProperties = ParseSchemaObject();
                        }
                        else
                        {
                            node.AdditionalPropertiesAllowed = ParseBool();
                        }

                        break;

                    default:
                        SkipValue();
                        break;
                }

                SkipWhitespace();

                if (Current() == '}')
                {
                    _pos++;
                    break;
                }

                Expect(',');
            }

            return node;
        }

        /// <summary>
        /// Handles the "type" field which can be either a string or an array of strings
        /// (e.g. ["string","null"] for nullable types in JSON Schema draft-07).
        /// </summary>
        private void ParseTypeValue(SchemaNode node)
        {
            if (Current() == '[')
            {
                var types = ParseStringArray();
                foreach (var t in types)
                {
                    if (t == "null")
                    {
                        node.IsNullableType = true;
                    }
                    else
                    {
                        node.Type = t;
                    }
                }
            }
            else
            {
                node.Type = ParseString();
            }
        }

        /// <summary>Parses the "properties" map and populates <see cref="SchemaNode.Properties"/>.</summary>
        private void ParsePropertiesObject(SchemaNode node)
        {
            Expect('{');
            SkipWhitespace();

            if (Current() == '}')
            {
                _pos++;
                return;
            }

            while (true)
            {
                SkipWhitespace();
                var propName = ParseString();
                SkipWhitespace();
                Expect(':');
                SkipWhitespace();

                node.Properties[propName] = ParseSchemaObject();
                SkipWhitespace();

                if (Current() == '}')
                {
                    _pos++;
                    break;
                }

                Expect(',');
            }
        }
    }
}
