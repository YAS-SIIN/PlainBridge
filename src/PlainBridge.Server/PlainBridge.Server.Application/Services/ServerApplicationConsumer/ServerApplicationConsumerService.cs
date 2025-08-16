

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
        var channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);
        await channel.QueueDeclareAsync(queueName, false, false, false, null, cancellationToken: cancellationToken);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            byte[] body = ea.Body.ToArray();
            var props = ea.BasicProperties as BasicProperties;
            props ??= new BasicProperties();
            props.Headers ??= new Dictionary<string, object?>();
 
            
            int usePort = 0;
            if (props.Headers.TryGetValue("user_port", out var portObj) && int.TryParse(portObj?.ToString(), out int usePortInt))
            {
                usePort = usePortInt; 
            }

            var useportUsername = string.Empty;
            if (props.Headers.TryGetValue("useport_username", out var usernameObj) && usernameObj is byte[] usernameBytes)
            {
                useportUsername = Encoding.UTF8.GetString(usernameBytes); 
            }

            var useportConnectionId = string.Empty;
            if (props.Headers.TryGetValue("useport_connectionid", out var connectionIdObj) && connectionIdObj is byte[] connectionIdBytes)
            {
                useportConnectionId = Encoding.UTF8.GetString(connectionIdBytes);
            }

            var serverApplication = await _cacheManagement.SetGetServerApplicationAsync(useportUsername, usePort);
            if (serverApplication is null || serverApplication == default)
            {
                await channel.BasicAckAsync(ea.DeliveryTag, false, cancellationToken: cancellationToken);
                return;
            }

            if (!serverApplication.ServerApplicationAppId.HasValue)
            {
                await channel.BasicAckAsync(ea.DeliveryTag, false, cancellationToken: cancellationToken);
                return;
            }

            var destinationServerApplication = await _cacheManagement.SetGetServerApplicationAsync(serverApplication.ServerApplicationAppId.Value);
            if (destinationServerApplication is null || destinationServerApplication == default)
            {
                await channel.BasicAckAsync(ea.DeliveryTag, false, cancellationToken: cancellationToken);
                return;
            }

            var destinationQueue = $"{destinationServerApplication.UserName}client_shared_port_network_packets";
            await channel.QueueDeclareAsync(destinationQueue, false, false, false, null, cancellationToken: cancellationToken);

            props.Headers["useport_username"] = useportUsername;
            props.Headers["useport_port"] = usePort; 
            props.Headers["useport_connectionid"] = useportConnectionId; 
            props.Headers["shared_port"] = destinationServerApplication.InternalPort;

            await channel.BasicPublishAsync(string.Empty, destinationQueue, false, props, body, cancellationToken: cancellationToken);
            await channel.BasicAckAsync(ea.DeliveryTag, false, cancellationToken: cancellationToken);
        };

        await channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer, cancellationToken: cancellationToken);
    }

    private async Task InitializeResponseConsumerAsync(CancellationToken cancellationToken)
    {
        var queueName = "server_network_responses";
        var channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

        await channel.QueueDeclareAsync(queueName, false, false, false, null, cancellationToken: cancellationToken);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            byte[] body = ea.Body.ToArray();

            var props = ea.BasicProperties as BasicProperties;

            props ??= new BasicProperties();
            props.Headers ??= new Dictionary<string, object?>();
             
            int usePort = 0;
            if (props.Headers.TryGetValue("user_port", out var portObj) && int.TryParse(portObj?.ToString(), out int usePortInt))
            {
                usePort = usePortInt;
            }

            var useportUsername = string.Empty;
            if (props.Headers.TryGetValue("useport_username", out var usernameObj) && usernameObj is byte[] usernameBytes)
            {
                useportUsername = Encoding.UTF8.GetString(usernameBytes);
            }

            var useportConnectionId = string.Empty;
            if (props.Headers.TryGetValue("useport_connectionid", out var connectionIdObj) && connectionIdObj is byte[] connectionIdBytes)
            {
                useportConnectionId = Encoding.UTF8.GetString(connectionIdBytes);
            }
             
            var serverApplication = await _cacheManagement.SetGetServerApplicationAsync(useportUsername, usePort);  
            if (serverApplication is null || serverApplication == default)
            {
                await channel.BasicAckAsync(ea.DeliveryTag, false, cancellationToken: cancellationToken);
                return;
            }

            var destinationQueue = $"{useportUsername}_client_user_port_network_packets";
            await channel.QueueDeclareAsync(destinationQueue, false, false, false, null, cancellationToken: cancellationToken);

            props.Headers["user_port"] = usePort;
            props.Headers["user_connection_id"] = useportConnectionId;
            props.Headers["useport_username"] = useportUsername;

            await channel.BasicPublishAsync(string.Empty, destinationQueue, false, props, body, cancellationToken: cancellationToken);
            await channel.BasicAckAsync(ea.DeliveryTag, false, cancellationToken: cancellationToken);
        };

        await channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer, cancellationToken: cancellationToken);
    }

}
