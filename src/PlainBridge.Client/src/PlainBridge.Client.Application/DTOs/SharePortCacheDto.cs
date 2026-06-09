using System.Net.Sockets;

namespace PlainBridge.Client.Application.DTOs;


public record SharePortCacheDto(TcpClient TcpClient, Task HandleTcpClientResponsesTask);
