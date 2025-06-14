

using Microsoft.Extensions.Logging;
using PlainBridge.Server.Application.Management.Cache;
using PlainBridge.Server.Application.Services.HostApplication;
using PlainBridge.Server.Application.Services.ServerApplication;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using System.Text;


namespace PlainBridge.Server.Application.Services.ApiExternalBus;

public class ApiExternalBusService : IApiExternalBusService
{
    private readonly ILogger<ApiExternalBusService> _logger;
    private readonly IConnection _connection;
    private readonly IHostApplicationService _hostApplicationService;
    private readonly IServerApplicationService _serverApplicationService;

    public ApiExternalBusService(ILogger<ApiExternalBusService> logger, IConnection connection, IHostApplicationService hostApplicationService, IServerApplicationService serverApplicationService)
    {
        _logger = logger;
        _connection = connection;
        _hostApplicationService = hostApplicationService;
        _serverApplicationService = serverApplicationService;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        var queueName = $"api_to_server_external_bus";

        using var channel = await _connection.CreateChannelAsync();
        await channel.QueueDeclareAsync(queue: queueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var external_bus_consumer = new AsyncEventingBasicConsumer(channel);
        external_bus_consumer.ReceivedAsync += async (model, ea) =>
        {
            await channel.BasicAckAsync(ea.DeliveryTag, false);
            var message = Encoding.UTF8.GetString(ea.Body.ToArray());

            try
            {
                switch (message)
                {
                    case "Host_Application_Created":
                    case "Host_Application_Deleted":
                    case "Host_Application_Updated":
                        await _hostApplicationService.UpdateHostApplicationAsync(cancellationToken);
                        break;
                    case "Server_Application_Created":
                    case "Server_Application_Deleted":
                    case "Server_Application_Updated":
                        await _serverApplicationService.UpdateServerApplicationAsync(cancellationToken);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            catch
            {
                // ignored
            }

            await Task.Yield();
        };

        await _hostApplicationService.UpdateHostApplicationAsync(cancellationToken);
        await _serverApplicationService.UpdateServerApplicationAsync(cancellationToken);

        await channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: external_bus_consumer);
    }
}
