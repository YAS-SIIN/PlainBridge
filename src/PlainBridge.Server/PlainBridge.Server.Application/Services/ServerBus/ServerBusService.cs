
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using PlainBridge.Server.Application.Services.PlainBridgeApiClient;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PlainBridge.Server.Application.Services.ServerBus;

public class ServerBusService(ILogger<ServerBusService> _logger, IConnection _connection, IPlainBridgeApiClientService _plainBridgeApiClientHandler) : IServerBusService
{ 

    public async Task InitializeConsumerAsync(CancellationToken cancellationToken)
    {
        var queueName = "server_bus";
        var channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

        await channel.QueueDeclareAsync(queueName, false, false, false, null, cancellationToken: cancellationToken);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body); 
            var username = string.Empty;
            if (ea.BasicProperties!.Headers!.TryGetValue("username", out var usernameObj) && usernameObj is byte[] usernameBytes)
            {
                username = Encoding.UTF8.GetString(usernameBytes);
            }

            var clientExchangeName = "client_bus";
            var clientQueueName = $"{username}_client_bus";

            await channel.ExchangeDeclareAsync(clientExchangeName, "direct", false, false, null, cancellationToken: cancellationToken);
            await channel.QueueDeclareAsync(clientQueueName, false, false, false, null, cancellationToken: cancellationToken);
            await channel.QueueBindAsync(clientQueueName, clientExchangeName, null, cancellationToken: cancellationToken);

            var serverApplications = await _plainBridgeApiClientHandler.GetServerApplicationsAsync(cancellationToken);

            var responseBody = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(serverApplications));
            await channel.BasicPublishAsync(clientExchangeName, string.Empty, responseBody, cancellationToken: cancellationToken);

            await channel.BasicAckAsync(ea.DeliveryTag, false, cancellationToken: cancellationToken);
        };

        await channel.BasicConsumeAsync(queueName, false, consumer, cancellationToken: cancellationToken);
    }
}
