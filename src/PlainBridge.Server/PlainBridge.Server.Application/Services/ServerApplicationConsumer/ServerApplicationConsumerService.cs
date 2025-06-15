

using Microsoft.Extensions.Logging;

using PlainBridge.Server.Application.Management.Cache;

using RabbitMQ.Client;
using RabbitMQ.Client.Events; 
using System.Text; 

namespace PlainBridge.Server.Application.Services.AppProjectConsumer;

public class ServerApplicationConsumerService : IServerApplicationConsumerService
{
    private readonly ILogger<ServerApplicationConsumerService> _logger;
    private readonly ICacheManagement _cacheManagement;
    private readonly IConnection _connection;
    public ServerApplicationConsumerService(ILogger<ServerApplicationConsumerService> logger, ICacheManagement cacheManagement, IConnection connection)
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
            var props = ea.BasicProperties as BasicProperties;
            props ??= new BasicProperties();
            props.Headers ??= new Dictionary<string, object?>();

            var userPort = int.Parse(props.Headers["user_port"]?.ToString()!);
            string userConnectionId = string.Empty;
            if (props.Headers.TryGetValue("user_connection_id", out var connectionIdObj) && connectionIdObj is byte[] connectionIdBytes)
            {
                userConnectionId = Encoding.UTF8.GetString(connectionIdBytes);
            }

            if (!_cacheManagement.TryGetServerApplication(userPort, out var appProject))
            {
                await channel.BasicAckAsync(ea.DeliveryTag, false);
                return;
            }

            if (!appProject.ServerApplicationViewId.HasValue)
            {
                await channel.BasicAckAsync(ea.DeliveryTag, false);
                return;
            }

            if (!_cacheManagement.TryGetServerApplication(appProject.ServerApplicationViewId.Value, out var destinationAppProject))
            {
                await channel.BasicAckAsync(ea.DeliveryTag, false);
                return;
            }

            var destinationQueue = $"client_shared_port_network_packets"; // Fixed spelling error: changed "sharedport" to "shared_port"
            await channel.QueueDeclareAsync(destinationQueue, false, false, false, null);



            props.Headers["user_port"] = userPort;
            props.Headers["user_connection_id"] = userConnectionId;
            props.Headers["shared_port"] = destinationAppProject.InternalPort; // Fixed spelling error: changed "sharedport_port" to "shared_port"

            await channel.BasicPublishAsync(string.Empty, destinationQueue, false, props, body);
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

            var props = ea.BasicProperties as BasicProperties;

            props ??= new BasicProperties();
            props.Headers ??= new Dictionary<string, object?>();

            var userPort = int.Parse(props.Headers["user_port"]?.ToString()!);
            string userConnectionId = string.Empty;
            if (props.Headers.TryGetValue("user_connection_id", out var connectionIdObj) && connectionIdObj is byte[] connectionIdBytes)
            {
                userConnectionId = Encoding.UTF8.GetString(connectionIdBytes);
            }


            if (!_cacheManagement.TryGetServerApplication(userPort, out var appProject))
            {
                await channel.BasicAckAsync(ea.DeliveryTag, false);
                return;
            }

            var destinationQueue = $"client_user_port_network_packets"; // Fixed spelling error: changed "userport" to "user_port"
            await channel.QueueDeclareAsync(destinationQueue, false, false, false, null);

            props.Headers["user_port"] = userPort;
            props.Headers["user_connection_id"] = userConnectionId; // Fixed spelling error: changed "user_connectionid" to "user_connection_id"

            await channel.BasicPublishAsync(string.Empty, destinationQueue, false, props, body);
            await channel.BasicAckAsync(ea.DeliveryTag, false);
        };

        await channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer);
    }

}
