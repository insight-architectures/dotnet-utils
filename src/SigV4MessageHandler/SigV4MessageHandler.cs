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
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using Amazon;
using Amazon.Runtime;

namespace InsightArchitectures.Utilities;

#pragma warning disable CA1308

/// <summary>
/// A message handler that can be added to <see cref="HttpClient"/> and overrides the Authorize header. This will sign the request using SigV4, which can be used for AWS IAM secured URLs.
/// </summary>
public class SigV4MessageHandler : DelegatingHandler
{
    private readonly AWSCredentials _credentials;
    private readonly RegionEndpoint _region;
    private readonly string? _service;
    private const string AuthHeader = "Authorization";
    private const string Iso8601BasicDateTimeFormat = "yyyyMMddTHHmmssZ";
    private const string Terminator = "aws4_request";
    private const string Scheme = "AWS4";
    private const string Algorithm = "HMAC-SHA256";

    private readonly Regex _compressWhitespaceRegex;
    private readonly UriServiceParser _parser;

    /// <summary>
    /// Needs <paramref name="credentials"/> and <paramref name="region"/>, which can be provided by e.g. the AWSConfig object.
    /// </summary>
    /// <param name="credentials"></param>
    /// <param name="region"></param>
    /// <param name="service"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public SigV4MessageHandler(AWSCredentials credentials, RegionEndpoint region, string? service = null)
    {
        _credentials = credentials ?? throw new ArgumentNullException(nameof(credentials));
        _region = region ?? throw new ArgumentNullException(nameof(region));
        _service = service;

        _parser = new UriServiceParser();
        _compressWhitespaceRegex = new Regex("\\s+");
    }

    /// <inheritdoc />
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage? request,
        CancellationToken cancellationToken)
    {
        if (request == null)
        {
            return await base.SendAsync(request!, cancellationToken).ConfigureAwait(false);
        }

        request.Headers.Remove(AuthHeader);

        var dt = DateTime.UtcNow;
        var dateTime = dt.ToString(Iso8601BasicDateTimeFormat, CultureInfo.InvariantCulture);
        var dateStamp = dt.ToString("yyyyMMdd", CultureInfo.InvariantCulture);

        request.Headers.Add("X-Amz-Date", dateTime);

        if (!request.Headers.Contains("Host"))
        {
            var hostHeader = request.RequestUri!.Host;
            if (!request.RequestUri.IsDefaultPort)
            {
                hostHeader += ":" + request.RequestUri.Port;
            }

            request.Headers.Host = hostHeader;
        }

        var region = _region.SystemName;
        var service = GetService(request.RequestUri);

        var scope = string.Format(CultureInfo.InvariantCulture, "{0}/{1}/{2}/{3}", dateStamp, region, service,
            Terminator);

        var headersToSign = GetHeadersForSigning(request.Headers);

        var canonicalRequest = await GenerateCanonical(request, headersToSign).ConfigureAwait(false);

        var stringToSignBuilder = new StringBuilder();
        stringToSignBuilder.Append($"{Scheme}-{Algorithm}\n{dateTime}\n{scope}\n");

        var canonicalRequestHashBytes = Crypto.Hash_SHA256(Encoding.UTF8.GetBytes(canonicalRequest));
        stringToSignBuilder.Append(Utils.ToHex(canonicalRequestHashBytes, true));

        var credentials = await _credentials.GetCredentialsAsync().ConfigureAwait(false);

        var hashKey = ComposeSigningKey(credentials.SecretKey, region, dateStamp, service);
        var stringToSign = stringToSignBuilder.ToString();
        var signature = Crypto.SignHMAC_SHA256(Encoding.UTF8.GetBytes(stringToSign), hashKey);

        const string _Scheme = $"{Scheme}-{Algorithm}";

        var authorizationHeader = new StringBuilder();
        authorizationHeader.Append($"Credential={credentials.AccessKey}/{scope}, ");
        authorizationHeader.Append($"SignedHeaders={GetSignedHeaders(headersToSign)}, ");
        authorizationHeader.Append($"Signature={Utils.ToHex(signature, true)}");

        request.Headers.Authorization = new AuthenticationHeaderValue(_Scheme, authorizationHeader.ToString());

        return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }

    private static byte[] ComposeSigningKey(string awsSecretAccessKey, string region, string date, string service)
    {
        const string KsecretPrefix = "AWS4";
        char[]? ksecret = null;

        try
        {
            ksecret = (KsecretPrefix + awsSecretAccessKey).ToCharArray();

            var hashDate = Crypto.SignHMAC_SHA256(Encoding.UTF8.GetBytes(date), Encoding.UTF8.GetBytes(ksecret));
            var hashRegion = Crypto.SignHMAC_SHA256(Encoding.UTF8.GetBytes(region), hashDate);
            var hashService = Crypto.SignHMAC_SHA256(Encoding.UTF8.GetBytes(service), hashRegion);
            return Crypto.SignHMAC_SHA256(Encoding.UTF8.GetBytes(Terminator), hashService);
        }
        finally
        {
            // clean up all secrets, regardless of how initially seeded (for simplicity)
            if (ksecret != null)
            {
                Array.Clear(ksecret, 0, ksecret.Length);
            }
        }
    }

    private string GetService(Uri uri)
    {
        if (!string.IsNullOrWhiteSpace(_service))
        {
            return _service!;
        }

        var service = _parser.Parse(uri);

        return service;
    }

    private async Task<string> GenerateCanonical(HttpRequestMessage request, IReadOnlyList<string> headersToSign)
    {
        var queryString = request.Method == HttpMethod.Get
            ? Utils.GetParametersAsString(request.RequestUri!)
            : string.Empty;

        var cr = new StringBuilder();
        cr.Append($"{request.Method.Method}\n");
        cr.Append($"{GetCanonicalizedResourcePath(request.RequestUri!)}\n");
        cr.Append($"{queryString}\n");

        cr.Append($"{GetCanonicalizedHeaders(headersToSign, request.Headers)}\n");
        cr.Append($"{GetSignedHeaders(headersToSign)}\n");

        string requestBody;
        if (request.Content != null)
        {
            requestBody = await request.Content.ReadAsStringAsync().ConfigureAwait(false);
        }
        else
        {
            requestBody = Utils.GetParametersAsString(request.RequestUri!);
        }

        var payload = Crypto.Hash_SHA256(Encoding.UTF8.GetBytes(requestBody));
        cr.Append(Utils.ToHex(payload, true));

        return cr.ToString();
    }

    private static string GetSignedHeaders(IEnumerable<string> headersToSign) =>
        string.Join(";", headersToSign.Select(x => x.ToLowerInvariant()));

    private string GetCanonicalizedHeaders(IReadOnlyList<string> headersToSign, HttpRequestHeaders allHeaders)
    {
        if (headersToSign.Count == 0)
        {
            return string.Empty;
        }

        headersToSign = headersToSign.Select(x => x.ToLowerInvariant()).ToArray();

        var sortedHeaderMap = new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var entry in allHeaders)
        {
            if (headersToSign.Contains(entry.Key.ToLowerInvariant()))
            {
                sortedHeaderMap[entry.Key] = entry.Value.First();
            }
        }

        var builder = new StringBuilder();
        foreach (var entry in sortedHeaderMap)
        {
            builder.Append(entry.Key.ToLowerInvariant());
            builder.Append(':');
            builder.Append(CompressSpaces(entry.Value));
            builder.Append('\n');
        }

        string CompressSpaces(string data)
        {
            if (string.IsNullOrEmpty(data) && data != " ")
            {
                return data;
            }

            var compressed = _compressWhitespaceRegex.Replace(data, " ");
            return compressed;
        }

        return builder.ToString();
    }

    private static string GetCanonicalizedResourcePath(Uri endpoint)
    {
        var uri = endpoint.AbsolutePath;
        return string.IsNullOrEmpty(uri) ? "/" : Utils.UrlEncode(uri, true);
    }

    private static IReadOnlyList<string> GetHeadersForSigning(HttpRequestHeaders requestHeaders)
    {
        var headersToSign = requestHeaders.Select(entry => entry.Key).ToList();
        headersToSign.Sort(StringComparer.OrdinalIgnoreCase);

        return headersToSign;
    }
}
#pragma warning restore CA1308
