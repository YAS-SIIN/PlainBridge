

using Microsoft.Extensions.Logging; 
using PlainBridge.Server.Application.Services.HostApplication;
using PlainBridge.Server.Application.Services.ServerApplication; 
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using System.Text;


namespace PlainBridge.Server.Application.Services.ApiExternalBus;

public class ApiExternalBusService(ILogger<ApiExternalBusService> _logger, IConnection _connection, IHostApplicationService _hostApplicationService, IServerApplicationService _serverApplicationService) : IApiExternalBusService
{
    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        var queueName = $"api_to_server_external_bus";
        _logger.LogInformation("Initializing ApiExternalBusService and declaring queue: {QueueName}", queueName);

        using var channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);
        await channel.QueueDeclareAsync(queue: queueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null, 
            cancellationToken: cancellationToken);

        var external_bus_consumer = new AsyncEventingBasicConsumer(channel);
        external_bus_consumer.ReceivedAsync += async (model, ea) =>
        {
            await channel.BasicAckAsync(ea.DeliveryTag, false, cancellationToken: cancellationToken);
            var message = Encoding.UTF8.GetString(ea.Body.ToArray());
            _logger.LogInformation("Received message from external bus: {Message}", message);

            try
            {
                switch (message)
                {
                    case "Host_Application_Created":
                    case "Host_Application_Deleted":
                    case "Host_Application_Updated":
                        _logger.LogInformation("Updating Host Application due to message: {Message}", message);
                        await _hostApplicationService.UpdateServerApplicationAsync(cancellationToken);
                        break;
                    case "Server_Application_Created":
                    case "Server_Application_Deleted":
                    case "Server_Application_Updated":
                        _logger.LogInformation("Updating Server Application due to message: {Message}", message);
                        await _serverApplicationService.UpdateHostApplicationAsync(cancellationToken);
                        break;
                    default:
                        _logger.LogWarning("Received unknown message type: {Message}", message);
                        throw new NotImplementedException();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message: {Message}", message);
                // ignored
            }

            await Task.Yield();
        };

        _logger.LogInformation("Updating Host Application cache on initialization.");
        await _hostApplicationService.UpdateServerApplicationAsync(cancellationToken);

        _logger.LogInformation("Updating Server Application cache on initialization.");
        await _serverApplicationService.UpdateHostApplicationAsync(cancellationToken);

        _logger.LogInformation("Starting to consume messages from queue: {QueueName}", queueName);
        await channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: external_bus_consumer, cancellationToken);
    }
}
