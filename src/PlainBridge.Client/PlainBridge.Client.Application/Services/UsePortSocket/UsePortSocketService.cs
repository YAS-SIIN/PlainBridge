
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using PlainBridge.Client.Application.DTOs;
using PlainBridge.Client.Application.Management.Cache;
using PlainBridge.SharedApplication.DTOs;
using PlainBridge.SharedApplication.Enums;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PlainBridge.Client.Application.Services.UsePortSocket;

public class UsePortSocketService(ILogger<UsePortSocketService> logger, IConnection _connection, ICacheManagement _cacheManagement) : IUsePortSocketService
{

    public async Task InitializeAsync(string username, List<ServerApplicationDto> serverApplications, CancellationToken cancellationToken)
    {
        logger.LogInformation("Initializing UsePortSocketService for user {username}", username);
        foreach (var appProject in serverApplications.Where(x => x.ServerApplicationType == ServerApplicationTypeEnum.UsePort))
        {
            var tcpListener = await _cacheManagement.SetGetTcpListenerAsync(appProject.InternalPort, cancellationToken: cancellationToken);

            if (tcpListener is not null && tcpListener != default) continue;

            tcpListener = new TcpListener(IPAddress.Any, appProject.InternalPort);
            tcpListener.Server.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, false);
            tcpListener.Start();

            await _cacheManagement.SetGetTcpListenerAsync(appProject.InternalPort, tcpListener, cancellationToken: cancellationToken);

            var task = Task.Run(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var client = await tcpListener.AcceptTcpClientAsync(cancellationToken);
                    var clientStream = client.GetStream();
                    var connectionId = Guid.NewGuid().ToString();

                    var handleIncommingRequestsTask = Task.Run(async () => await HandleTcpClientIncommingRequestsAsync(username, appProject.InternalPort, connectionId, clientStream, cancellationToken));

                    await _cacheManagement.SetGetUsePortModelAsync(appProject.InternalPort, connectionId, new(client, handleIncommingRequestsTask), cancellationToken: cancellationToken);
                }
            });
        }

        await HandleSocketResponsesAsync(username, cancellationToken);
    }

    private async Task HandleTcpClientIncommingRequestsAsync(string username, int port, string connectionId, NetworkStream clientStream, CancellationToken cancellationToken)
    {
        var queueName = "server_network_requests";
        var exchangeName = "server_network_requests";
        var channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

        await channel.ExchangeDeclareAsync(exchangeName, "direct", false, false, null, cancellationToken: cancellationToken);
        await channel.QueueDeclareAsync(queueName, false, false, false, null, cancellationToken: cancellationToken);
        await channel.QueueBindAsync(queueName, exchangeName, string.Empty, null, cancellationToken: cancellationToken);

        byte[] buffer = new byte[1024];
        int bytesRead;
        while (true)
        {
            while ((bytesRead = await clientStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
            {
                var packetModel = new PacketDto();
                packetModel.Buffer = buffer;
                packetModel.Count = bytesRead;
                var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(packetModel));

                var properties = new BasicProperties
                {
                    Headers = new Dictionary<string, object?>
                    {
                        { "useport_username", username },
                        { "useport_port", port },
                        { "useport_connectionid", connectionId }
                    }
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

    // Fix for CS0079 and CS1746:
    // Replace incorrect usage of 'channel.BasicAcksAsync' (an event) with the correct method 'channel.BasicAckAsync' to acknowledge messages.

    private async Task HandleSocketResponsesAsync(string username, CancellationToken cancellationToken)
    {
        var queueName = $"{username}_client_useport_network_packets";

        var channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);
        await channel.QueueDeclareAsync(queueName, false, false, false, null, cancellationToken: cancellationToken);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            byte[] body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var packetModel = JsonSerializer.Deserialize<PacketDto>(message);

            var useportUsername = Encoding.UTF8.GetString(ea.BasicProperties!.Headers["useport_username"]! as byte[]);
            var useportPort = int.Parse(ea.BasicProperties!.Headers["useport_port"]!.ToString() ?? string.Empty);
            var useportConnectionId = Encoding.UTF8.GetString(ea.BasicProperties!.Headers["useport_connectionid"]! as byte[]);

            var useportModel = await _cacheManagement.SetGetUsePortModelAsync(useportPort, useportConnectionId, cancellationToken: cancellationToken);
            if (useportModel is null || useportModel == default)
            {
                await channel.BasicAckAsync(ea.DeliveryTag, false, cancellationToken: cancellationToken);
                return;
            }

            if (!useportModel.TcpClient.Connected)
            {
                await channel.BasicAckAsync(ea.DeliveryTag, false, cancellationToken: cancellationToken);
                return;
            }

            try
            {
                await useportModel.TcpClient.GetStream().WriteAsync(packetModel!.Buffer, 0, packetModel!.Count, cancellationToken);
            }
            catch (Exception)
            {
                // ignored
            }

            await channel.BasicAckAsync(ea.DeliveryTag, false, cancellationToken: cancellationToken);
        };

        await channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer, cancellationToken: cancellationToken);
    }
}