
using Microsoft.AspNetCore.Mvc.Testing;
using PlainBridge.IdentityServer.Tests.IntegrationTests.Utils;
using System.Net.WebSockets;
using System.Text;

namespace PlainBridge.Server.Tests.IntegrationTests.Application.Services;

[Collection("ServerIntegrationTestRun")]
public class WebSocketTests : IClassFixture<IntegrationTestRunFixture>
{
    private readonly IntegrationTestRunFixture _fixture;
    public WebSocketTests(IntegrationTestRunFixture fixture) => _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));



    [Fact]
    public async Task WebSocketTests_WhenEveryThingIsOk()
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(60));
         
        var clientWebSocket = _fixture.WebApplicationFactory.Server.CreateWebSocketClient();
        var webSocket = await clientWebSocket.ConnectAsync(new Uri("http://localhost/"), CancellationToken.None);

        // Act & Assert
        for (int i = 0; i < 1000; i++)
        {
            await SendMessageToWebSocket(webSocket, "PlainBridge", cancellationTokenSource.Token);
            var response = await ReceiveMessageFromWebSocket(webSocket, cancellationTokenSource.Token);

            Assert.Equal($"{i + 1}: PlainBridge", response);
        }
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


