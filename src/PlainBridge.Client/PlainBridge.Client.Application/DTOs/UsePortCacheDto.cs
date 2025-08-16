using System.Net.Sockets;

namespace PlainBridge.Client.Application.DTOs;


public record UsePortCacheDto (TcpClient TcpClient, Task HandleIncommingRequestsTask);
