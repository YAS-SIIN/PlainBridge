

using System.Net.WebSockets;

namespace PlainBridge.Server.Application.Management.WebSocket;

public class WebSocketManagement(System.Net.WebSockets.WebSocket _websocket) : IWebSocketManagement
{ 

    public Task ConnectAsync(Uri uri, CancellationToken cancellationToken = default!) => throw new NotImplementedException();

    public async Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken = default!) => await _websocket.ReceiveAsync(buffer, cancellationToken);

    public async Task SendAsync(ArraySegment<byte> buffer, WebSocketMessageType messageType, bool endOfMessage, CancellationToken cancellationToken = default!) => await _websocket.SendAsync(buffer, messageType, endOfMessage, cancellationToken);
}
