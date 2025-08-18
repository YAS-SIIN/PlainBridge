
using System.Data.Common;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using PlainBridge.Client.Application.DTOs;
using PlainBridge.Client.Application.Management.Cache;
using PlainBridge.SharedApplication.DTOs;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PlainBridge.Client.Application.Services.SharePortSocket;


public class SharePortSocketService(ILogger<SharePortSocketService> _logger, IConnection _connection, ICacheManagement _cacheManagement) : ISharePortSocketService
{


    public async Task InitializeAsync(string username, List<ServerApplicationDto> appProjects, CancellationToken cancellationToken)
    {
        var queueName = $"{username}_client_shareport_network_packets";

        var channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

        await channel.QueueDeclareAsync(queueName, false, false, false, null, cancellationToken: cancellationToken);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            byte[] body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var packetModel = JsonSerializer.Deserialize<PacketDto>(message);

            var useportUsername = Encoding.UTF8.GetString(ea.BasicProperties.Headers["useport_username"] as byte[]);
            var useportPort = int.Parse(ea.BasicProperties.Headers["useport_port"].ToString()!);
            var useportConnectionId = Encoding.UTF8.GetString(ea.BasicProperties.Headers["useport_connectionid"] as byte[]);


            var sharePortCacheModel = await _cacheManagement.SetSharePortModelAsync(useportUsername, useportPort, useportConnectionId.ToString(), cancellationToken: cancellationToken);
            if (sharePortCacheModel is null)
            {
                var client = new TcpClient();
                await client.ConnectAsync(new IPEndPoint(IPAddress.Parse("127.0.0.1"), int.Parse(ea.BasicProperties.Headers["sharedport_port"].ToString()!)));
                var handleTcpClientResponsesTask = Task.Run(async () => await HandleTcpClientResponsesAsync(client, useportUsername, useportPort, useportConnectionId, cancellationToken));

                sharePortCacheModel = new(client, handleTcpClientResponsesTask);
                await _cacheManagement.SetSharePortModelAsync(useportUsername, useportPort, useportConnectionId, sharePortCacheModel, cancellationToken: cancellationToken);
            }

            await sharePortCacheModel.TcpClient.GetStream().WriteAsync(packetModel.Buffer, 0, packetModel.Count, cancellationToken);

            await channel.BasicAckAsync(ea.DeliveryTag, false, cancellationToken: cancellationToken);
        };

        await channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer, cancellationToken: cancellationToken);
    }

    private async Task HandleTcpClientResponsesAsync(TcpClient tcpClient, string useportUsername, int useportPort, string connectionId, CancellationToken cancellationToken)
    {
        var queueName = "server_network_responses";
        var exchangeName = "server_network_responses";

        var channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

        await channel.ExchangeDeclareAsync(exchangeName, "direct", false, false, null, cancellationToken: cancellationToken);
        await channel.QueueDeclareAsync(queueName, false, false, false, null, cancellationToken: cancellationToken);
        await channel.QueueBindAsync(queueName, exchangeName, string.Empty, null, cancellationToken: cancellationToken);

        byte[] buffer = new byte[1024];
        int bytesRead;
        while (true)
        {
            while ((bytesRead = await tcpClient.GetStream().ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                var packetModel = new PacketDto();
                packetModel.Buffer = buffer;
                packetModel.Count = bytesRead;
                var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(packetModel));

                var properties = new BasicProperties();
                properties.Headers = new Dictionary<string, object?>()
                    {
                        { "useport_username", useportUsername },
                        { "useport_port", useportPort },
                        { "useport_connectionid", connectionId }
                    };

                await channel.BasicPublishAsync(exchange: exchangeName,
                    routingKey: string.Empty,
                    mandatory: true,
                    basicProperties: properties,
                    body: body,
                    cancellationToken: cancellationToken);
            }
        }
    }
}