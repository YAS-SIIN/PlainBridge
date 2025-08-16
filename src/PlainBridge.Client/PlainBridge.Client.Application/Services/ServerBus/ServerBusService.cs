

using System.Text; 
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace PlainBridge.Client.Application.Services.ServerBus;

public class ServerBusService(ILogger<ServerBusService> _logger, IConnection _connection) : IServerBusService
{
    public async Task RequestAppProjects(string username, CancellationToken cancellationToken)
    {
        var exchangeName = "server_bus";
        var queueName = "server_bus";

        var channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

        await channel.ExchangeDeclareAsync(exchangeName, "direct", false, false, null, cancellationToken: cancellationToken);
        await channel.QueueDeclareAsync(queueName, false, false, false, null, cancellationToken: cancellationToken);
        await channel.QueueBindAsync(queueName, exchangeName, string.Empty, null, cancellationToken: cancellationToken);

        var body = Encoding.UTF8.GetBytes("GET_APPPROJECTS");

        var properties = new BasicProperties
        {
            Headers = new Dictionary<string, object?> { { "username", username! } }
        };

        await channel.BasicPublishAsync(exchangeName,
            string.Empty,
            mandatory: false,
            properties,
            body,
            cancellationToken: cancellationToken);
    }
}
