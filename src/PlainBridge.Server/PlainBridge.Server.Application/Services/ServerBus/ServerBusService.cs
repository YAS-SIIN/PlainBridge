
using Microsoft.Extensions.Logging;

using PlainBridge.Server.Application.Handler.PlainBridgeApiClient;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using System.Text;
using System.Text.Json;
using System.Threading.Channels;

namespace PlainBridge.Server.Application.Services.ServerBus;

public class ServerBusService : IServerBusService
{
    private readonly ILogger<ServerBusService> _logger;
    private readonly IConnection _connection;
    private readonly IPlainBridgeApiClientHandler _plainBridgeApiClientHandler;
    public ServerBusService(ILogger<ServerBusService> logger, IConnection connection, IPlainBridgeApiClientHandler plainBridgeApiClientHandler)
    {
        _logger = logger;
        _connection = connection;
        _plainBridgeApiClientHandler = plainBridgeApiClientHandler;
    }

    public async Task InitializeConsumerAsync(CancellationToken cancellationToken)
    {
        var queueName = "server_bus";
        using var channel = await _connection.CreateChannelAsync();


        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            var clientExchangeName = "client_bus";
            var clientQueueName = $"client_bus";

            await channel.ExchangeDeclareAsync(clientExchangeName, "direct", false, false, null);
            await channel.QueueDeclareAsync(clientQueueName, false, false, false, null);
            await channel.QueueBindAsync(clientQueueName, clientExchangeName, null);

            var serverApplications = await _plainBridgeApiClientHandler.GetServerApplicationsAsync(cancellationToken);

            var responseBody = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(serverApplications));
            await channel.BasicPublishAsync(clientExchangeName, null, responseBody);

            await channel.BasicAckAsync(ea.DeliveryTag, false);
        };

        await channel.BasicConsumeAsync(queueName, false, consumer);
    }
}
