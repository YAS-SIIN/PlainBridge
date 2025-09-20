using System.Net;
using System.Net.Http.Headers;
using PlainBridge.Client.Application.Helpers.Http;

namespace PlainBridge.Client.Tests.UnitTests.Helpers;

[Collection("ClientUnitTestRun")]
public class HttpHelperTests
{
    private readonly IHttpHelper _httpHelper = new HttpHelper();

    [Theory]
    [InlineData("GET", "Get")]
    [InlineData("POST", "Post")]
    [InlineData("PUT", "Put")]
    [InlineData("DELETE", "Delete")]
    [InlineData("PATCH", "Patch")]
    [InlineData("OPTIONS", "Options")]
    [InlineData("HEAD", "Head")]
    [InlineData("TRACE", "Trace")]
    public void GetMethod_ShouldMapToHttpMethod(string input, string expectedName)
    {
        var method = _httpHelper.GetMethod(input);
        Assert.Equal(expectedName.ToUpperInvariant(), method.Method.ToUpperInvariant());
    }

    [Fact]
    public void IsContentOfType_ShouldDetectMatchingContentType()
    {
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("test")
        };
        response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");

        var result = _httpHelper.IsContentOfType(response, "text/html");

        Assert.True(result);
    }

    [Fact]
    public void ReplaceUrls_ShouldReplaceHttpHttpsAndWwwVariants()
    {
        var oldUri = new Uri("http://old.example.com");
        var newUri = new Uri("https://new.example.com");
        var input = "http://old.example.com/path script src=\"https://old.example.com/app.js\" img src=\"http://www.old.example.com/a.png\"";

        var output = _httpHelper.ReplaceUrls(input, oldUri, newUri);

        Assert.DoesNotContain("old.example.com", output);
        Assert.Contains("https://new.example.com/path", output);
        Assert.Contains("https://new.example.com/app.js", output);
        Assert.Contains("https://new.example.com/a.png", output);
    }
}
