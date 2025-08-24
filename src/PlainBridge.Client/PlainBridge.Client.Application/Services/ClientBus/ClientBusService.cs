

using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using PlainBridge.Client.Application.Management.Cache;
using PlainBridge.Client.Application.Services.SharePortSocket;
using PlainBridge.Client.Application.Services.UsePortSocket;
using PlainBridge.SharedApplication.DTOs;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PlainBridge.Client.Application.Services.ClientBus;

public class ClientBusService(ILogger<ClientBusService> _logger, IConnection _connection, ICacheManagement _cacheManagement, IUsePortSocketService _usePortSocketService, ISharePortSocketService _sharePortSocketService) : IClientBusService
{

    public async Task InitializeConsumerAsync(string username, CancellationToken cancellationToken)
    {
        var queueName = $"{username}_client_bus";

        var channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

        await channel.QueueDeclareAsync(
            queue: queueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);


        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            var serverApplications = JsonSerializer.Deserialize<List<ServerApplicationDto>>(message);

            if (serverApplications is null || serverApplications.Count == 0)
            {
                _logger.LogWarning("Received empty or null server applications for user {Username}", username);
                return;
            }

            await _cacheManagement.SetGetServerApplicationsAsync(serverApplications, cancellationToken: cancellationToken);
            await _usePortSocketService.InitializeAsync(username, serverApplications, cancellationToken);
            await _sharePortSocketService.InitializeAsync(username, serverApplications, cancellationToken);

            await channel.BasicAckAsync(ea.DeliveryTag, false, cancellationToken: cancellationToken);
        };
        try
        {

            await channel.BasicConsumeAsync(queueName, false, consumer, cancellationToken: cancellationToken);
        }
        catch (Exception EX)
        {
            var aa = EX;
        }
    }
}