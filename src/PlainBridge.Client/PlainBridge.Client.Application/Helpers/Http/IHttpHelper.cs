using PlainBridge.SharedApplication.DTOs;

namespace PlainBridge.Client.Application.Helpers.Http;

public interface IHttpHelper
{
    Task<HttpResponseDto> CreateAndSendRequestAsync(string requestUrl, string method, IEnumerable<KeyValuePair<string, IEnumerable<string>>> headers, byte[] bytes, Uri internalUri);
    HttpMethod GetMethod(string method);
    bool IsContentOfType(HttpResponseMessage responseMessage, string type);
    string ReplaceUrls(string text, Uri oldUri, Uri newUri);
}