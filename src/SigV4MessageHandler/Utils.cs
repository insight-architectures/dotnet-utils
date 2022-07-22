/*
 * This code is partially based on code of the AWS SDK for .NET, which
 * is licensed under the Apache License, Version 2.0 (the "License").
 * You may not use this file except in compliance with the License.
 * A copy of the License is located at
 *
 *  http://aws.amazon.com/apache2.0
 *
 * or in the "license" file accompanying this file.
 */
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace InsightArchitectures.Utilities;

#pragma warning disable SA1600
internal static class Utils
{
    private static readonly string _validPathCharacters = DetermineValidPathCharacters();
    private const string ValidUrlCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";
    private const string BasePathCharacters = "/:'()!*[]$";

    public static string UrlEncode(string data, bool path)
    {
        var encoded = new StringBuilder(data.Length * 2);
        var unreservedChars = string.Concat(ValidUrlCharacters, path ? _validPathCharacters : string.Empty);

        foreach (var b in Encoding.UTF8.GetBytes(data))
        {
            var symbol = (char)b;
            if (unreservedChars.IndexOf(symbol) != -1)
            {
                encoded.Append(symbol);
            }
            else
            {
                encoded.Append('%').Append(string.Format(CultureInfo.InvariantCulture, "{0:X2}", (int)symbol));
            }
        }

        return encoded.ToString();
    }

    public static string ToHex(IEnumerable<byte> data, bool lowercase)
    {
        var sb = new StringBuilder();

        foreach (var t in data)
        {
            sb.Append(t.ToString(lowercase ? "x2" : "X2", CultureInfo.InvariantCulture));
        }

        return sb.ToString();
    }

    private static string DetermineValidPathCharacters()
    {
        var sb = new StringBuilder();
        foreach (var c in BasePathCharacters)
        {
            var escaped = Uri.EscapeDataString(c.ToString());
            if (escaped.Length == 1 && escaped[0] == c)
            {
                sb.Append(c);
            }
        }
        return sb.ToString();
    }

    public static string GetParametersAsString(Uri uri)
    {
        var queryDictionary = Regex.Matches(uri.Query, "([^?=&]+)(=([^&]*))?").Cast<Match>()
            .ToDictionary(x => x.Groups[1].Value, x => x.Groups[3].Value);
        var data = new StringBuilder();

        foreach (var kvp in queryDictionary)
        {
            if (string.IsNullOrWhiteSpace(kvp.Key))
            {
                continue;
            }

            data.Append(kvp.Key);
            data.Append('=');
            data.Append(UrlEncode(kvp.Value, false));
            data.Append('&');
        }

        var result = data.ToString();

        return string.IsNullOrWhiteSpace(result) ? string.Empty : result.Remove(result.Length - 1);
    }
}
#pragma warning restore SA1600
