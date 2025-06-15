

using System.Net.WebSockets;

namespace PlainBridge.Server.Application.Management.WebSocketManagement;

public class WebSocketManagement(WebSocket _websocket) : IWebSocketManagement
{
    
    public async Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken) => await _websocket.ReceiveAsync(buffer, cancellationToken);
    public async Task SendAsync(ArraySegment<byte> buffer, WebSocketMessageType messageType, bool endOfMessage, CancellationToken cancellationToken) => await _websocket.SendAsync(buffer, messageType, endOfMessage, cancellationToken);
}
