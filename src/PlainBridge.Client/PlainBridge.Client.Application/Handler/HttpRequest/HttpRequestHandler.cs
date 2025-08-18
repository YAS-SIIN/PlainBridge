
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using PlainBridge.Client.Application.Helpers.Http;
using PlainBridge.SharedApplication.DTOs;
using PlainBridge.SharedApplication.Exceptions;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PlainBridge.Client.Application.Handler.HttpRequest;

public class HttpRequestHandler(ILogger<HttpRequestHandler> _logger, IConnection _connection, IHttpHelper _httpHelper) : IHttpRequestHandler
{
    public async Task InitializeHttpRequestConsumerAsync(string username, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting http request handler ...");

        var responseExchangeName = "response";
        var responseQueueName = "response_bus";
        var requestQueueName = $"{username}_request_bus";

        var channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

        await channel.ExchangeDeclareAsync(exchange: responseExchangeName,
            type: "direct",
            durable: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);

        await channel.QueueDeclareAsync(queue: responseQueueName,
                 durable: false,
                 exclusive: false,
                 autoDelete: false,
                 arguments: null,
            cancellationToken: cancellationToken);

        await channel.QueueBindAsync(queue: responseQueueName,
           exchange: responseExchangeName,
           routingKey: "",
           arguments: null,
            cancellationToken: cancellationToken);

        await channel.QueueDeclareAsync(queue: requestQueueName,
                 durable: false,
                 exclusive: false,
                 autoDelete: false,
                 arguments: null,
            cancellationToken: cancellationToken);

        // Start consuming requests 
        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            await channel.BasicAckAsync(ea.DeliveryTag, false, cancellationToken: cancellationToken);

            try
            {
                var props = ea.BasicProperties as BasicProperties;
                props ??= new BasicProperties();
                var body = Encoding.UTF8.GetString(ea.Body.ToArray());
                var requestModel = JsonSerializer.Deserialize<HttpRequestDto>(body);

                if (!ea.BasicProperties!.Headers!.TryGetValue("IntUrl", out var internalUrlByteArray))
                    throw new NotFoundException("Internal url not found");
                if (!ea.BasicProperties.Headers.TryGetValue("Host", out var hostByteArray))
                    throw new NotFoundException("Host not found");
                var internalUri = new Uri(Encoding.UTF8.GetString((byte[])internalUrlByteArray!));
                var host = Encoding.UTF8.GetString((byte[])hostByteArray!);

                var response = await _httpHelper.CreateAndSendRequestAsync(requestModel!.RequestUrl, requestModel!.Method, requestModel!.Headers, requestModel!.Bytes, internalUri);

                var responseBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(response));

                await channel.BasicPublishAsync(exchange: responseExchangeName,
                    routingKey: "",
                    mandatory: false,
                    basicProperties: props,
                    body: responseBytes,
                    cancellationToken: cancellationToken);


            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        };

        await channel.BasicConsumeAsync(queue: requestQueueName, autoAck: false, consumer: consumer, cancellationToken: cancellationToken);
    }
}
