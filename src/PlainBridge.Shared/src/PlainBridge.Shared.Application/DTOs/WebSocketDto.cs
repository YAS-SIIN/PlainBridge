

using System.Net.WebSockets;

namespace PlainBridge.Shared.Application.DTOs;

public record WebSocketDto
{
    public byte[] Payload { get; set; }
    public int PayloadCount { get; set; }
    public WebSocketMessageType MessageType { get; set; }
    public bool EndOfMessage { get; set; }
}
