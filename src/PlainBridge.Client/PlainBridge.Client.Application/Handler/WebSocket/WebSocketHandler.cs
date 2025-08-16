
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using PlainBridge.Client.Application.Services.WebSocket;
using PlainBridge.SharedApplication.DTOs;
using PlainBridge.SharedApplication.Exceptions;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PlainBridge.Client.Application.Handler.WebSocket;

public class WebSocketHandler(ILogger<WebSocketHandler> _logger, IWebSocketService _webSocketService, IConnection _connection) : IWebSocketHandler
{

    public async Task InitializeWebSocketConsumerAsync(string username, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting websocket request handler ...");

        var serverBusQueueName = $"{username}_websocket_server_bus";
         
        var channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

        await channel.QueueDeclareAsync(queue: serverBusQueueName,
                 durable: false,
                 exclusive: false,
                 autoDelete: false,
                 arguments: null);

        // Start consuming requests
        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            await channel.BasicAckAsync(ea.DeliveryTag, false);

            try
            {
                var requestID = ea.BasicProperties.MessageId;
                var body = Encoding.UTF8.GetString(ea.Body.ToArray());
                var requestModel = JsonSerializer.Deserialize<WebSocketDto>(body);

                if (!ea.BasicProperties!.Headers!.TryGetValue("IntUrl", out var internalUrlByteArray))
                    throw new NotFoundException("Internal url not found");
                if (!ea.BasicProperties.Headers.TryGetValue("Host", out var hostByteArray))
                    throw new NotFoundException("Host not found");
                var internalUri = new Uri(Encoding.UTF8.GetString((byte[])internalUrlByteArray!));
                var host = Encoding.UTF8.GetString((byte[])hostByteArray!);

                var webSocket = await _webSocketService.InitializeWebSocketAsync(host, internalUri, cancellationToken);
                var arraySegment = new ArraySegment<byte>(requestModel!.Payload, 0, requestModel!.PayloadCount);
                await webSocket.SendAsync(arraySegment,
                    requestModel.MessageType,
                    requestModel.EndOfMessage,
                    CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        };

        // Initialize publisher
        var queueName = "websocket_client_bus";
        var exchangeName = "websocket_bus";

        await channel.ExchangeDeclareAsync(exchange: exchangeName,
            type: "direct",
            durable: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);

        await channel.QueueDeclareAsync(queue: queueName,
                 durable: false,
                 exclusive: false,
                 autoDelete: false,
                 arguments: null,
                 cancellationToken: cancellationToken);

        await channel.QueueBindAsync(queue: queueName,
            exchange: exchangeName,
            routingKey: queueName,
            arguments: null, 
            cancellationToken: cancellationToken);

        // Start consumer
        await channel.BasicConsumeAsync(queue: serverBusQueueName, 
            autoAck: false, 
            consumer: consumer, 
            cancellationToken: cancellationToken);
    }
}
