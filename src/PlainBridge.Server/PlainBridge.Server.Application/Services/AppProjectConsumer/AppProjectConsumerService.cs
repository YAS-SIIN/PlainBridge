

using Microsoft.Extensions.Logging;

using PlainBridge.Server.Application.Management.Cache;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace PlainBridge.Server.Application.Services.AppProjectConsumer;

public class AppProjectConsumerService : IAppProjectConsumerService
{
    private readonly ILogger<AppProjectConsumerService> _logger;
    private readonly ICacheManagement _cacheManagement;
    private readonly IConnection _connection;
    public AppProjectConsumerService(ILogger<AppProjectConsumerService> logger, ICacheManagement cacheManagement, IConnection connection)
    {
        _logger = logger;
        _cacheManagement = cacheManagement;
        _connection = connection;
    }


    public async Task InitializeConsumerAsync()
    {
        await InitializeRequestConsumerAsync();
        await InitializeResponseConsumerAsync();
    }

    private async Task InitializeRequestConsumerAsync()
    {
        var queueName = "server_network_requests";
        using var channel = await _connection.CreateChannelAsync();
        await channel.QueueDeclareAsync(queueName, false, false, false, null);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            byte[] body = ea.Body.ToArray();
            var useportPort = int.Parse(ea.BasicProperties.Headers["useport_port"].ToString()!);
            var useportConnectionId = Encoding.UTF8.GetString(ea.BasicProperties.Headers["useport_connectionid"] as byte[]);

            if (!_cacheManagement.TryGetServerApplication(useportPort, out var appProject))
            {
                await channel.BasicAckAsync(ea.DeliveryTag, false);
                return;
            }

            if (!appProject.AppId.HasValue)
            {
                channel.BasicAckAsync(ea.DeliveryTag, false);
                return;
            }

            if (!_cacheManagement.TryGetServerApplication(appProject.AppId.Value, out var destinationAppProject))
            {
                await channel.BasicAckAsync(ea.DeliveryTag, false);
                return;
            }

            var destinationQueue = $"lient_shareport_network_packets";
            await channel.QueueDeclareAsync(destinationQueue, false, false, false, null);

            var properties = channel.CreateBasicProperties();
            properties.Headers = new Dictionary<string, object>()
                {
                    { "useport_port", useportPort },
                    { "useport_connectionid", useportConnectionId },
                    { "sharedport_port", destinationAppProject.InternalPort }
                };

            await channel.BasicPublishAsync(string.Empty, destinationQueue, properties, body);
            await channel.BasicAckAsync(ea.DeliveryTag, false);
        };

        await channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer);
    }

    private async Task InitializeResponseConsumerAsync()
    {
        var queueName = "server_network_responses";
        using var channel = await _connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(queueName, false, false, false, null);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            byte[] body = ea.Body.ToArray();
            var useportPort = int.Parse(ea.BasicProperties.Headers["useport_port"].ToString()!);
            var useportConnectionId = Encoding.UTF8.GetString(ea.BasicProperties.Headers["useport_connectionid"] as byte[]);

            if (!_cacheManagement.TryGetServerApplication(useportPort, out var appProject))
            {
                await channel.BasicAckAsync(ea.DeliveryTag, false);
                return;
            }

            var destinationQueue = $"client_useport_network_packets";
            await channel.QueueDeclareAsync(destinationQueue, false, false, false, null);

            var properties = channel.CreateBasicProperties();
            properties.Headers = new Dictionary<string, object>()
                {
                    { "useport_port", useportPort },
                    { "useport_connectionid", useportConnectionId }
                };

            await channel.BasicPublishAsync(string.Empty, destinationQueue, properties, body);
            await channel.BasicAckAsync(ea.DeliveryTag, false);
        };

        await channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer);
    }

}
