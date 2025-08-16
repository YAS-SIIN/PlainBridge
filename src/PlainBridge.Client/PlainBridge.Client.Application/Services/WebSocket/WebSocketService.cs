

using System.Data.Common;
using System.Text;
using System.Text.Json;
using System.Threading;
using Microsoft.Extensions.Logging;
using PlainBridge.Client.Application.Handler;
using PlainBridge.Client.Application.Management.Cache;
using PlainBridge.Client.Application.Management.WebSocket;
using PlainBridge.SharedApplication.DTOs;
using RabbitMQ.Client;

namespace PlainBridge.Client.Application.Services.WebSocket;


public class WebSocketService(ILogger<WebSocketService> _logger, IWebSocketManagement _webSocketManagement, ICacheManagement _cacheManagement, IConnection _connection) : IWebSocketService
{

    public async Task<IWebSocketManagement> InitializeWebSocketAsync(string host, Uri internalUri, CancellationToken cancellationToken)
    {
        var webSocket = await _cacheManagement.SetGetWebSocketAsync(host, cancellationToken: cancellationToken);
        if (webSocket is not null && webSocket != default)
            return webSocket;

        webSocket = _webSocketManagement;

        var webSocketUri = internalUri;
        var webSocketUriBuilder = default(UriBuilder);
        if (internalUri.Scheme == "https")
        {
            webSocketUriBuilder = new UriBuilder(internalUri)
            {
                Scheme = "wss"
            };
        }
        else
        {
            webSocketUriBuilder = new UriBuilder(internalUri)
            {
                Scheme = "ws"
            };
        }

        await webSocket.ConnectAsync(webSocketUriBuilder.Uri, cancellationToken);
        await _cacheManagement.SetGetWebSocketAsync(host, webSocket, cancellationToken: cancellationToken);

        var task = Task.Run(async () => await InitializeWebSocketReceiverAsync(webSocket, host, cancellationToken));

        return webSocket;
    }

    private async Task InitializeWebSocketReceiverAsync(IWebSocketManagement webSocket, string host, CancellationToken cancellationToken)
    {
        try
        {
            var queueName = "websocket_client_bus";
            var exchangeName = "websocket_bus";
            var channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);
            await channel.QueueDeclareAsync(queueName, false, false, false, null, cancellationToken: cancellationToken);

            var buffer = new byte[1024 * 4];
            do
            {
                var receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (receiveResult.MessageType == System.Net.WebSockets.WebSocketMessageType.Close)
                    break;

                var webSocketData = new WebSocketDto
                {
                    Payload = buffer,
                    PayloadCount = receiveResult.Count,
                    MessageType = receiveResult.MessageType,
                    EndOfMessage = receiveResult.EndOfMessage
                };


                var properties = new BasicProperties
                {
                    Headers = new Dictionary<string, object>
                    {
                        { "IntUrl", "" },
                        { "Host", host }
                    }
                };

                var message = JsonSerializer.Serialize(webSocketData);

                await channel.BasicPublishAsync(exchange: exchangeName,
                    routingKey: queueName,
                    mandatory: false,
                    basicProperties: properties,
                    body: Encoding.UTF8.GetBytes(message),
                    cancellationToken: cancellationToken);

            } while (true);
        }
        finally
        {
            await _cacheManagement.RemoveWebSocketAsync(host);
        }
    }
}