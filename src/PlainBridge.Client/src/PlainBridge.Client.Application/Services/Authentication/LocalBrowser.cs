
using Duende.IdentityModel.OidcClient.Browser;

using System.Diagnostics;
using System.Net;
namespace PlainBridge.Client.Application.Services.Authentication;

public sealed class LocalBrowser : IBrowser
{
    public int Port { get; } = GetAvailablePort();
    public string RedirectUri => $"http://127.0.0.1:{Port}/callback/";
    public async Task<BrowserResult> InvokeAsync(BrowserOptions options, CancellationToken cancellationToken = default)
    {
        using var listener = new HttpListener();

        listener.Prefixes.Add(RedirectUri);
        listener.Start();

        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = options.StartUrl,
                UseShellExecute = true
            });

            var context = await listener.GetContextAsync().WaitAsync(cancellationToken);

            var response = context.Response;

            var html = """
                <!DOCTYPE html>
                <html>
                <head><title>PlainBridge</title></head>
                <body>
                    <h2>Authentication completed.</h2>
                    <p>You can close this browser window.</p>
                </body>
                </html>
                """;

            var buffer = System.Text.Encoding.UTF8.GetBytes(html);

            response.ContentType = "text/html";
            response.ContentLength64 = buffer.Length;

            await response.OutputStream.WriteAsync(buffer, cancellationToken);

            response.Close();

            return new BrowserResult
            {
                Response = context.Request.Url!.ToString(),
                ResultType = BrowserResultType.Success
            };
        }
        catch (Exception ex)
        {
            return new BrowserResult
            {
                ResultType = BrowserResultType.UnknownError,
                Error = ex.Message
            };
        }
    }
    private static int GetAvailablePort()
    {
        using var socket = new System.Net.Sockets.TcpListener(IPAddress.Loopback, 0);

        socket.Start();

        return ((IPEndPoint)socket.LocalEndpoint).Port;
    }
}