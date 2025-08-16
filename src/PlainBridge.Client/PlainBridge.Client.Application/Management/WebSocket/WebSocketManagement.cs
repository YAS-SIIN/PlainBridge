

using System.Net.WebSockets;
using Microsoft.Extensions.Logging;


namespace PlainBridge.Client.Application.Management.WebSocket;

public class WebSocketManagement(ILogger<WebSocketManagement> _logger, ClientWebSocket _websocket) : IWebSocketManagement
{
    public ClientWebSocketOptions Options => throw new NotImplementedException();

    public async Task ConnectAsync(Uri uri, CancellationToken cancellationToken = default!) => await _websocket.ConnectAsync(uri, cancellationToken);

    public async Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken = default!) => await _websocket.ReceiveAsync(buffer, cancellationToken);

    public async Task SendAsync(ArraySegment<byte> buffer, WebSocketMessageType messageType, bool endOfMessage, CancellationToken cancellationToken = default!) => await _websocket.SendAsync(buffer, messageType, endOfMessage, cancellationToken);
}
