using System;
using InsightArchitectures.Utilities.Http;
using NUnit.Framework;

namespace Tests;

[TestFixture]
[TestOf(typeof(UriServiceParser))]
public class UriServiceParserTests
{
    [TestCase("https://ganjlefnaosldfn.lambda-url.eu-west-1.on.aws/", "lambda")]
    [TestCase("https://amplify.us-west-2.amazonaws.com/", "amplify")]
    [TestCase("https://aisdhfoasdf.appsync-url.eu-west-1.amazonaws.com/", "appsync")]
    [TestCase("https://sqs.us-east-2.amazonaws.com/123456789012/MyQueue", "sqs")]
    public void Adding_valid_url_should_return_service_name(string url, string service)
    {
        var sut = new UriServiceParser();
        
        var actual = sut.Parse(new Uri(url));
        
        Assert.AreEqual(service, actual);
    }
}
