using System.Text.RegularExpressions;

namespace InsightArchitectures.Utilities;

#pragma warning disable CA1308
#pragma warning disable SA1600

internal class UriServiceParser
{
    private readonly Regex _regionalEndpointRegex;

    private static readonly string[] _hostNames = { "appsync", "lambda" };

    public UriServiceParser()
    {
        _regionalEndpointRegex = new Regex(":\\/\\/([aA-zZ0-9\\-]*).[aA-zZ0-9\\-.]*amazonaws.com");
    }

    public string Parse(Uri uri)
    {
        foreach (var service in _hostNames)
        {
            if (uri.Host.ToLowerInvariant().Contains(service))
            {
                return service;
            }
        }

        var match = _regionalEndpointRegex.Match(uri.OriginalString);

        if (match.Groups.Count > 1)
        {
            return match.Groups[1].Value;
        }

        throw new SigV4MessageHandlerException(
            "Could not automatically determine AWS service based on URL. Please provide the service name you are trying to access through the message handler constructor.");
    }
}
