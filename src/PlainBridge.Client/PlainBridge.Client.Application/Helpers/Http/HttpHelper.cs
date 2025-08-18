using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using PlainBridge.SharedApplication.DTOs;

namespace PlainBridge.Client.Application.Helpers.Http;

public class HttpHelper : IHttpHelper
{
    public async Task<HttpResponseDto> CreateAndSendRequestAsync(string requestUrl, string method, IEnumerable<KeyValuePair<string, IEnumerable<string>>> headers, byte[] bytes, Uri internalUri)
    {
        var httpRequestMessage = new HttpRequestMessage();

        if (!HttpMethods.IsGet(method) &&
            !HttpMethods.IsHead(method) &&
            !HttpMethods.IsDelete(method) &&
            !HttpMethods.IsTrace(method))
        {
            var stream = new MemoryStream(bytes);
            var streamContent = new StreamContent(stream);
            httpRequestMessage.Content = streamContent;
        }

        var contentHeaders = new string[] { "Accept-Ranges", "Age", "Content-Disposition", "Content-Encoding", "Content-Language", "Content-Length", "Content-Location", "Content-Range", "Content-Type", "Expires", "Last-Modified", "Pragma", "Trailer", "Transfer-Encoding", "Vary", "Via", "Warning" };

        foreach (var header in headers)
        {
            if (header.Key.ToLower() == "Host".ToLower())
            {
                httpRequestMessage.Content?.Headers.TryAddWithoutValidation(header.Key, internalUri.Authority.ToString());
                continue;
            }

            if (header.Key.ToLower() == "Referer".ToLower())
            {
                var baseUri = new Uri($"{internalUri.Scheme}://{internalUri.Authority}");
                var refererUri = new Uri(header.Value.First());
                httpRequestMessage.Content?.Headers.TryAddWithoutValidation(header.Key, new Uri(baseUri, refererUri.PathAndQuery).ToString());
                continue;
            }

            httpRequestMessage.Content?.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());

            if (!contentHeaders.Contains(header.Key) && !header.Key.StartsWith(":") && !header.Key.StartsWith("Accept-"))
                httpRequestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
        }

        var originalUri = new Uri(requestUrl);
        var uri = new Uri(internalUri, originalUri.PathAndQuery);

        httpRequestMessage.RequestUri = uri;
        httpRequestMessage.Headers.Host = internalUri.Authority;
        httpRequestMessage.Method = GetMethod(method);

        var handler = new HttpClientHandler();
        handler.SslProtocols = System.Security.Authentication.SslProtocols.Tls12; // Adjust the SSL/TLS protocol version as needed
        handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
        handler.AllowAutoRedirect = false;

        using var httpClient = new HttpClient(handler);

        var httpResponseModel = new HttpResponseDto();
        using (var responseMessage = await httpClient.SendAsync(httpRequestMessage, HttpCompletionOption.ResponseHeadersRead, CancellationToken.None))
        {
            httpResponseModel.HttpStatusCode = responseMessage.StatusCode;

            var isRedirected = responseMessage.StatusCode == HttpStatusCode.Redirect || responseMessage.StatusCode == HttpStatusCode.MovedPermanently;
            if (isRedirected)
            {
                var redirectUrl = responseMessage.Headers.Location!.ToString();
                httpResponseModel.IsRedirected = true;

                var requestUri = new Uri(requestUrl);
                var newUri = new Uri($"{requestUri.Scheme}://{requestUri.Authority}");
                httpResponseModel.RedirectUrl = new Uri(newUri, redirectUrl).ToString();

                return httpResponseModel;
            }

            var newHeaders = new List<KeyValuePair<string, IEnumerable<string>>>();
            foreach (var header in responseMessage.Content.Headers)
                newHeaders.Add(new KeyValuePair<string, IEnumerable<string>>(header.Key, header.Value));

            foreach (var header in responseMessage.Headers)
            {
                if (!newHeaders.Any(x => x.Key == header.Key))
                    newHeaders.Add(new KeyValuePair<string, IEnumerable<string>>(header.Key, header.Value));
            }

            newHeaders = newHeaders.Where(x => x.Key != "transfer-encoding").ToList();

            httpResponseModel.Headers = newHeaders;

            var content = await responseMessage.Content.ReadAsByteArrayAsync();
            httpResponseModel.Bytes = content;
            if (IsContentOfType(responseMessage, "text/html") || IsContentOfType(responseMessage, "text/javascript"))
            {
                var stringContent = Encoding.UTF8.GetString(content);
                var requestUri = new Uri(requestUrl);
                var newUri = new Uri($"{requestUri.Scheme}://{requestUri.Authority}");
                var newContent = ReplaceUrls(stringContent, internalUri, newUri);
                httpResponseModel.StringContent = newContent;
            }

            httpResponseModel.IsSuccessStatusCode = responseMessage.IsSuccessStatusCode;
        }

        return httpResponseModel;
    }

    public bool IsContentOfType(HttpResponseMessage responseMessage, string type)
    {
        var result = false;

        if (responseMessage.Content?.Headers?.ContentType != null)
        {
            result = responseMessage.Content.Headers.ContentType.MediaType == type;
        }

        return result;
    }

    public HttpMethod GetMethod(string method)
    {
        if (HttpMethods.IsDelete(method)) return HttpMethod.Delete;
        if (HttpMethods.IsGet(method)) return HttpMethod.Get;
        if (HttpMethods.IsHead(method)) return HttpMethod.Head;
        if (HttpMethods.IsOptions(method)) return HttpMethod.Options;
        if (HttpMethods.IsPost(method)) return HttpMethod.Post;
        if (HttpMethods.IsPut(method)) return HttpMethod.Put;
        if (HttpMethods.IsPatch(method)) return HttpMethod.Patch;
        if (HttpMethods.IsTrace(method)) return HttpMethod.Trace;
        return new HttpMethod(method);
    }

    public string ReplaceUrls(string text, Uri oldUri, Uri newUri)
    {
        var newUrl = $"{newUri.Scheme}://{newUri.Authority}";

        var pattern = $"([H|h][T|t][T|t][P|p])://{oldUri.Authority}";
        var newText = Regex.Replace(text, pattern, newUrl);

        pattern = $"([H|h][T|t][T|t][P|p][S|s])://{oldUri.Authority}";
        newText = Regex.Replace(newText, pattern, newUrl);

        if (!oldUri.Authority.StartsWith("www"))
        {
            pattern = $"([H|h][T|t][T|t][P|p])://www.{oldUri.Authority}";
            newText = Regex.Replace(newText, pattern, newUrl);

            pattern = $"([H|h][T|t][T|t][P|p][S|s])://www.{oldUri.Authority}";
            newText = Regex.Replace(newText, pattern, newUrl);
        }

        return newText;
    }
}
