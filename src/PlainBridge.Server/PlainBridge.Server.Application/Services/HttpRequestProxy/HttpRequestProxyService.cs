

using Microsoft.Extensions.Logging;

using PlainBridge.Server.Application.DTOs;
using PlainBridge.Server.Application.Management.ResponseCompletionSources;
using PlainBridge.Server.Application.Services.ApiExternalBus;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using System.Text;
using System.Text.Json;

namespace PlainBridge.Server.Application.Services.HttpRequestProxy;


public class HttpRequestProxyService
{
    private readonly ILogger<HttpRequestProxyService> _logger;
    private readonly IApiExternalBusService _apiExternalBusService;
    private readonly IConnection _connection;
    private readonly ResponseCompletionSourcesManagement _responseCompletionSourcesManagement;

    public HttpRequestProxyService(ILogger<HttpRequestProxyService> logger, IApiExternalBusService apiExternalBusService, IConnection connection, ResponseCompletionSourcesManagement responseCompletionSourcesManagement)
    {
        _logger = logger;
        _apiExternalBusService = apiExternalBusService;
        _connection = connection;
        _responseCompletionSourcesManagement = responseCompletionSourcesManagement;
    }

    public async Task InitializeConsumerAsync(CancellationToken cancellationToken)
    {
        await _apiExternalBusService.InitializeAsync(cancellationToken);

        var queueName = "response_bus";
        var exchangeName = "response";

        using var channel = await _connection.CreateChannelAsync();

        await channel.ExchangeDeclareAsync(exchange: exchangeName,
            type: "direct",
            durable: false,
        autoDelete: false,
            arguments: null);

       await  channel.QueueDeclareAsync(queue: queueName,
                 durable: false,
                 exclusive: false,
                 autoDelete: false,
                 arguments: null);

        await channel.QueueBindAsync(queue: queueName,
           exchange: exchangeName,
           routingKey: "",
           arguments: null);

        // Start consuming responses
        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            await channel.BasicAckAsync(ea.DeliveryTag, false);

            try
            {
                var responseID = ea.BasicProperties.MessageId;
                //var response = Encoding.UTF8.GetString(ea.Body.ToArray());

                // Retrieve the response completion source and complete it
                if (RetrieveResponseCompletionSource(responseID, out var responseCompletionSource))
                {
                    var httpResponse = JsonSerializer.Deserialize<HttpResponseDto>(Encoding.UTF8.GetString(ea.Body.ToArray()));
                    responseCompletionSource.SetResult(httpResponse);
                }
            }
            catch
            {
                // ignored
            }
        };

        await channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer);
    }

    private bool RetrieveResponseCompletionSource(string requestID, out TaskCompletionSource<HttpResponseDto> responseCompletionSource)
    {
        // Retrieve the response completion source from the dictionary or cache based on requestID
        // Return true if the response completion source is found, false otherwise
        // You need to implement the appropriate logic for retrieving and removing the completion source
        // This is just a placeholder method to illustrate the concept

        // Example implementation using a dictionary
        return _responseCompletionSourcesManagement.Sources.TryRemove(requestID, out responseCompletionSource);
    }
}