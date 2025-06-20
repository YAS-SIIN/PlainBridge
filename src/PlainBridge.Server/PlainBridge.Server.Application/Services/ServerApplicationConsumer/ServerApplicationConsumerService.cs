

using Microsoft.Extensions.Logging;

using PlainBridge.Server.Application.Management.Cache;

using RabbitMQ.Client;
using RabbitMQ.Client.Events; 
using System.Text; 

namespace PlainBridge.Server.Application.Services.AppProjectConsumer;

public class ServerApplicationConsumerService(ILogger<ServerApplicationConsumerService> _logger, ICacheManagement _cacheManagement, IConnection _connection) : IServerApplicationConsumerService
{
 

    public async Task InitializeConsumerAsync(CancellationToken cancellationToken)
    {
        await InitializeRequestConsumerAsync(cancellationToken);
        await InitializeResponseConsumerAsync(cancellationToken);
    }

    private async Task InitializeRequestConsumerAsync(CancellationToken cancellationToken)
    {
        var queueName = "server_network_requests";
        using var channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);
        await channel.QueueDeclareAsync(queueName, false, false, false, null, cancellationToken: cancellationToken);

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
                await channel.BasicAckAsync(ea.DeliveryTag, false, cancellationToken: cancellationToken);
                return;
            }

            if (!appProject.ServerApplicationViewId.HasValue)
            {
                await channel.BasicAckAsync(ea.DeliveryTag, false, cancellationToken: cancellationToken);
                return;
            }

            if (!_cacheManagement.TryGetServerApplication(appProject.ServerApplicationViewId.Value, out var destinationAppProject))
            {
                await channel.BasicAckAsync(ea.DeliveryTag, false, cancellationToken: cancellationToken);
                return;
            }

            var destinationQueue = $"client_shared_port_network_packets";
            await channel.QueueDeclareAsync(destinationQueue, false, false, false, null, cancellationToken: cancellationToken);



            props.Headers["user_port"] = userPort;
            props.Headers["user_connection_id"] = userConnectionId;
            props.Headers["shared_port"] = destinationAppProject.InternalPort;

            await channel.BasicPublishAsync(string.Empty, destinationQueue, false, props, body, cancellationToken);
            await channel.BasicAckAsync(ea.DeliveryTag, false, cancellationToken: cancellationToken);
        };

        await channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer, cancellationToken: cancellationToken);
    }

    private async Task InitializeResponseConsumerAsync(CancellationToken cancellationToken)
    {
        var queueName = "server_network_responses";
        using var channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

        await channel.QueueDeclareAsync(queueName, false, false, false, null, cancellationToken: cancellationToken);

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
                await channel.BasicAckAsync(ea.DeliveryTag, false, cancellationToken: cancellationToken);
                return;
            }

            var destinationQueue = $"client_user_port_network_packets";
            await channel.QueueDeclareAsync(destinationQueue, false, false, false, null, cancellationToken: cancellationToken);

            props.Headers["user_port"] = userPort;
            props.Headers["user_connection_id"] = userConnectionId;

            await channel.BasicPublishAsync(string.Empty, destinationQueue, false, props, body, cancellationToken: cancellationToken);
            await channel.BasicAckAsync(ea.DeliveryTag, false, cancellationToken: cancellationToken);
        };

        await channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer, cancellationToken: cancellationToken);
    }

}
