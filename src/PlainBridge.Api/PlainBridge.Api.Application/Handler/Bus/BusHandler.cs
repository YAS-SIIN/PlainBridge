
using Microsoft.Extensions.Logging;

using RabbitMQ.Client;

using System.Threading.Tasks;

namespace PlainBridge.Api.Application.Handler.Bus;

public class BusHandler(ILogger<BusHandler> _logger, IConnection _connection) : IBusHandler
{
    public async Task PublishAsync<T>(T message, CancellationToken cancellationToken) where T : class
    {
        if (message == null) throw new ArgumentNullException(nameof(message));
        using var channel = await _connection.CreateChannelAsync();

        var exchangeName = "external_bus";
        var queueName = $"api_to_server_external_bus";
        var routingKey = "api_to_server";

        await channel.ExchangeDeclareAsync(exchange: exchangeName,
            type: "direct",
            durable: false,
            autoDelete: false,
            arguments: null);

        await channel.QueueDeclareAsync(queue: queueName,
                 durable: false,
                 exclusive: false,
                 autoDelete: false,
                 arguments: null);

        await channel.QueueBindAsync(queue: queueName,
            exchange: exchangeName,
            routingKey: routingKey,
            arguments: null);

        var body = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(message);

        // Specify the type argument explicitly to resolve CS0411
        await channel.BasicPublishAsync<BasicProperties>(
            exchange: exchangeName,
            routingKey: routingKey,
            mandatory: true,
            basicProperties: null,
            body: body,
            cancellationToken: cancellationToken);

        _logger.LogInformation("Message published to bus: {MessageType}", routingKey);
    }
}
