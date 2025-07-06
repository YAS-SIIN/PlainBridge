
using Microsoft.Extensions.Logging;
 

using RabbitMQ.Client;

using System.Threading.Tasks;

namespace PlainBridge.Api.Infrastructure.Messaging;

public class RabbitMqEventBus(ILogger<RabbitMqEventBus> _logger, IConnection _connection) : IEventBus
{
    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken) where TEvent : class
    {
        if (@event == null) throw new ArgumentNullException(typeof(TEvent).Name);
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

        var body = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(@event);

#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
        var properties = new BasicProperties
        {
            Headers = new Dictionary<string, object>
            {
                { "IntUrl", "internalUrl" },
                { "Host", "projectHost" }
            }
        };
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.

        // Specify the type argument explicitly to resolve CS0411
        //await channel.BasicPublishAsync<BasicProperties>(
        //    exchange: exchangeName,
        //    routingKey: routingKey,
        //    mandatory: true,
        //    basicProperties: properties,
        //    body: body,
        //    cancellationToken: cancellationToken);
        try
        {
            await channel.BasicPublishAsync<BasicProperties>(
           exchange: exchangeName,
           routingKey: routingKey,
           mandatory: true,
           basicProperties: properties,
           body: body,
           cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error in publishing message to bus: {MessageType}", routingKey);
        }
        _logger.LogInformation("Message published to bus: {MessageType}", routingKey);
    }
}
