

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using PlainBridge.Server.Application.DTOs;
using PlainBridge.Server.Application.Management.Cache;
using PlainBridge.Server.Application.Management.WebSocketManagement;
using PlainBridge.SharedApplication.DTOs;
using PlainBridge.SharedApplication.Exceptions;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace PlainBridge.Server.Application.Services.WebSocket;

public class WebSocketService(ILogger<WebSocketService> _logger, ICacheManagement _cacheManagement, IConnection _connection, ApplicationSetting _applicationSetting) : IWebSocketService
{

    public async Task HandleWebSocketAsync(IWebSocketManagement webSocketManagement, HostApplicationDto project, CancellationToken cancellationToken)
    {
        _cacheManagement.SetWebSocket(project.GetProjectHost(_applicationSetting.DefaultDomain), webSocketManagement);
        await InitializeRabbitMQAsync(cancellationToken);


        try
        {
            var buffer = new byte[1024];
            while (webSocketManagement != null && webSocketManagement is not null)
            {
                var result = await webSocketManagement.ReceiveAsync(buffer, cancellationToken);
                if (result.MessageType == WebSocketMessageType.Close)
                    break;

                var webSocketData = new WebSocketDto
                {
                    Payload = buffer,
                    PayloadCount = result.Count,
                    MessageType = result.MessageType,
                    EndOfMessage = result.EndOfMessage
                };

                var message = JsonSerializer.Serialize(webSocketData);
                await PublishWebSocketDataToRabbitMQAsync(project.GetProjectHost(_applicationSetting.DefaultDomain), project.InternalUrl, message, cancellationToken);
            }
        }
        finally
        {
            _cacheManagement.RemoveWebSocket(project.GetProjectHost(_applicationSetting.DefaultDomain));
        }
    }


    public async Task InitializeConsumerAsync(CancellationToken cancellationToken)
    {
        var queueName = "websocket_client_bus";
        using var channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);


        await channel.QueueDeclareAsync(queue: queueName,
                 durable: false,
                 exclusive: false,
                 autoDelete: false,
                 arguments: null, cancellationToken: cancellationToken);

        // Start consuming responses
        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            await channel.BasicAckAsync(ea.DeliveryTag, false, cancellationToken: cancellationToken);

            try
            {
                var response = Encoding.UTF8.GetString(ea.Body.ToArray());

                if (!ea.BasicProperties.Headers.TryGetValue("Host", out var hostByteArray))
                    throw new NotFoundException("Host not found");

                var host = Encoding.UTF8.GetString((byte[])hostByteArray);

                var requestModel = JsonSerializer.Deserialize<WebSocketDto>(response);
                if (!_cacheManagement.TryGetWebSocket(host, out IWebSocketManagement webSocket))
                    throw new NotFoundException("WebSocket not found");

                var arraySegment = new ArraySegment<byte>(requestModel.Payload, 0, requestModel.PayloadCount);
                await webSocket.SendAsync(arraySegment,
                        requestModel.MessageType,
                        requestModel.EndOfMessage,
                        cancellationToken);
            }
            catch
            {
                // ignored
            }
        };

        await channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer, cancellationToken: cancellationToken);
    }

    private async Task InitializeRabbitMQAsync(CancellationToken cancellationToken)
    {
        var queueName = $"websocket_server_bus";
        var exchangeName = "websocket_bus";
        using var channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

        await channel.ExchangeDeclareAsync(exchange: exchangeName,
            type: "direct",
            durable: false,
            autoDelete: false,
            arguments: null, cancellationToken: cancellationToken);

        await channel.QueueDeclareAsync(queue: queueName,
                 durable: false,
                 exclusive: false,
                 autoDelete: false,
                 arguments: null, cancellationToken: cancellationToken);

        await channel.QueueBindAsync(queue: queueName,
            exchange: exchangeName,
            routingKey: queueName,
            arguments: null, cancellationToken: cancellationToken);
    }

    private async Task PublishWebSocketDataToRabbitMQAsync(string projectHost, string internalUrl, string message, CancellationToken cancellationToken)
    {
        var queueName = $"websocket_server_bus";
        var exchangeName = "websocket_bus";
        using var channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

        var properties = new BasicProperties
        {
            Headers = new Dictionary<string, object>
            {
                { "IntUrl", internalUrl },
                { "Host", projectHost }
            }
        };

        await channel.BasicPublishAsync(
            exchange: exchangeName,
            routingKey: queueName,
            mandatory: false, // Add the unnamed argument here
            basicProperties: properties,
            body: Encoding.UTF8.GetBytes(message),
            cancellationToken: cancellationToken
        );
    }
}
