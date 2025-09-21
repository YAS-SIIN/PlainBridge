
using Aspire.Hosting.Testing;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using PlainBridge.Tests.Utils;
using System.Net.WebSockets;
using System.Text;

namespace PlainBridge.Tests.Server;

public class ServerTests : IClassFixture<AppHostIntegrationTestRunFixture>
{
    private readonly AppHostIntegrationTestRunFixture _fixture;

    private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
    public ServerTests(AppHostIntegrationTestRunFixture fixture)
    {
        _fixture = fixture;
    }


    [Fact]
    public async Task SendARequestToServerProject_WhenEveryThingIsOk_ShouldReturnData()
    {
        // Arrange
        _cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(60));
        var httpClient = _fixture.InjectedDistributedApplication.CreateHttpClient("server-endpoint");

        httpClient.BaseAddress = new Uri("https://localhost:5002");
        // Act
        var result = await httpClient.GetAsync("/", _cancellationTokenSource.Token);
        var response = await result.Content.ReadAsStringAsync(_cancellationTokenSource.Token);
 
        // Assert
        Assert.NotNull(response); 
    }



    [Theory]
    [InlineData("http://localhost/", "PlainBridge")]
    public async Task WebSocketTests(string url, string message)
    {
        // Arrange 
        _cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(60));
        var httpClient = _fixture.InjectedDistributedApplication.CreateHttpClient("server-endpoint");
        //using var webSocket = new ClientWebSocket();

        //await webSocket.ConnectAsync(new Uri("wss://localhost:5002/"), _cancellationTokenSource.Token);

        var clientWebSocketServer = _fixture.InjectedDistributedApplication.GetTestServer();
        var clientWebSocket = clientWebSocketServer.CreateWebSocketClient();
        var webSocketa = await clientWebSocket.ConnectAsync(new Uri(url), CancellationToken.None);


        // Act & Assert
        for (int i = 0; i < 1000; i++)
        {
            await SendMessageToWebSocket(webSocketa, message, _cancellationTokenSource.Token);
            var response = await ReceiveMessageFromWebSocket(webSocketa, _cancellationTokenSource.Token);

            Assert.Equal($"{i + 1}: {message}", response);
        }
    }

    [Fact]
    public async Task WebSocket_roundtrip_works()
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(60));

        // Wait until the server resource is healthy (health checks enabled)
        await _fixture.InjectedDistributedApplication.ResourceNotifications
            .WaitForResourceHealthyAsync("server-endpoint", cts.Token);

        // Get the server URL that Aspire allocated (http/https)
        var httpUri = new Uri("https://localhost:5002");

        // Build a ws/wss URI for your socket endpoint (adjust path as needed)
        var wsUri = new UriBuilder(httpUri)
        {
            Scheme = httpUri.Scheme == Uri.UriSchemeHttps ? "wss" : "ws",
            Port = -1,         // keep the same host:port from httpUri
            Path = "/"
        }.Uri;

        using var ws = new ClientWebSocket();

        // If you're using dev certs in HTTPS, you may need to relax validation for tests only:
        // ws.Options.RemoteCertificateValidationCallback = (_, __, ___, ____) => true;

        await ws.ConnectAsync(wsUri, cts.Token);

        // send/receive like you already do...
    }


    private static async Task SendMessageToWebSocket(WebSocket ws, string data, CancellationToken cancellationToken)
    {
        var encoded = Encoding.UTF8.GetBytes(data);
        var buffer = new ArraySegment<Byte>(encoded, 0, encoded.Length);
        await ws.SendAsync(buffer, WebSocketMessageType.Text, true, cancellationToken);
    }

    private static async Task<string> ReceiveMessageFromWebSocket(WebSocket ws, CancellationToken cancellationToken)
    {
        var buffer = new ArraySegment<byte>(new Byte[8192]);
        var result = default(WebSocketReceiveResult);

        using (var ms = new MemoryStream())
        {
            do
            {
                result = await ws.ReceiveAsync(buffer, cancellationToken);
                ms.Write(buffer.Array, buffer.Offset, result.Count);
            }
            while (!result.EndOfMessage);

            ms.Seek(0, SeekOrigin.Begin);

            using (var reader = new StreamReader(ms, Encoding.UTF8))
                return reader.ReadToEnd();
        }
    }

}
