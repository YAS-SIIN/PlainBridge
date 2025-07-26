using System;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http.Extensions;
using PlainBridge.Server.Application.DTOs;
using PlainBridge.Server.Application.Management.ResponseCompletionSources;
using PlainBridge.Server.Application.Services.ServerApplication;
using PlainBridge.SharedApplication.DTOs;
using RabbitMQ.Client; 

namespace PlainBridge.Server.ApiEndPoint.Middlewares;

public class HttpRequestProxyMiddleware(RequestDelegate _next, ILogger<HttpRequestProxyMiddleware> _logger, IServiceProvider _serviceProvider, IConnection _connection)
{ 
    private Dictionary<string, bool> _initializedQueues = new Dictionary<string, bool>();
     
    public async Task Invoke(HttpContext context, CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var hostApplicationService = scope.ServiceProvider.GetRequiredService<IServerApplicationService>();
        var responseCompletionSourcesManagement = scope.ServiceProvider.GetRequiredService<ResponseCompletionSourcesManagement>();
        var applicationSetting = scope.ServiceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<ApplicationSetting>>().Value;
        
        var requestId = Guid.NewGuid().ToString();
        var host = context.Request.Host;

        var hostApplication = hostApplicationService.GetByHost(host.Value);
        var projectHost = $"{hostApplication.Domain}{applicationSetting.DefaultDomain}";

        if (!context.WebSockets.IsWebSocketRequest)
            await HandleHttpRequest(context, requestId, hostApplication, projectHost, responseCompletionSourcesManagement, cancellationToken);
        else
            await _next(context);
    }

    private async Task HandleHttpRequest(HttpContext context, string requestId, HostApplicationDto hostApplication, string projectHost, ResponseCompletionSourcesManagement responseCompletionSourcesManagement, CancellationToken cancellationToken)
    {
        // Create a TaskCompletionSource to await the response
        var responseCompletionSource = new TaskCompletionSource<HttpResponseDto>();

        // Store the response completion source in a dictionary or cache
        StoreResponseCompletionSource(requestId, responseCompletionSource, responseCompletionSourcesManagement);

        // Extract request details
        var requestData = await GetRequestDataAsync(context.Request);

        await PublishRequestToRabbitMQAsync(hostApplication?.UserName ?? string.Empty, projectHost, hostApplication.InternalUrl, requestId, requestData, cancellationToken);

        // Wait for the response or timeout
        var responseTask = responseCompletionSource.Task;
        if (await Task.WhenAny(responseTask, Task.Delay(TimeSpan.FromSeconds(100))) == responseTask)
        {
            var response = await responseTask;
            if (response.IsRedirected)
            {
                context.Response.Redirect(response.RedirectUrl, response.HttpStatusCode == System.Net.HttpStatusCode.PermanentRedirect ? true : false);
                return;
            }

            context.Response.StatusCode = (int)response.HttpStatusCode;

            context.Response.ContentType = response.ContentType;
            context.Response.Headers.Clear();
            var excluded_headers_list = new string[]
            {
                    "transfer-encoding"
            };
            foreach (var header in response.Headers)
            {
                if (excluded_headers_list.Contains(header.Key.ToLower()))
                    continue;

                context.Response.Headers.TryAdd(header.Key, header.Value.ToArray());
            }

            if (!string.IsNullOrEmpty(response.StringContent))
            {
                await context.Response.WriteAsync(response.StringContent, Encoding.UTF8);
            }
            else
            {
                await context.Response.Body.WriteAsync(response.Bytes);
            }
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status504GatewayTimeout;
        }
    }

    private async Task PublishRequestToRabbitMQAsync(string username, string projectHost, string internalUrl, string requestId, string message, CancellationToken cancellationToken = default)
    {
        var queueName = $"{username}_request_bus";
        var exchangeName = "request";

        using var channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);
        if (!_initializedQueues.ContainsKey(username))
        {
            await channel.ExchangeDeclareAsync(exchange: exchangeName,
                type: "direct",
                durable: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: cancellationToken);

           await channel.QueueDeclareAsync(queue: queueName,
                     durable: false,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null,
                cancellationToken: cancellationToken);

           await  channel.QueueBindAsync(queue: queueName,
                exchange: exchangeName,
                routingKey: username,
                arguments: null,
                cancellationToken: cancellationToken);

            _initializedQueues.Add(username, true);
        }

      
        var properties = new BasicProperties
        {
            MessageId = requestId,
            Headers = new Dictionary<string, object>
            {
                { "IntUrl", internalUrl },
                { "Host", projectHost }
            }
        };

         
        await channel.BasicPublishAsync(exchange: exchangeName, 
            routingKey: username,
            false,
            basicProperties: properties, 
            body: Encoding.UTF8.GetBytes(message),
            cancellationToken: cancellationToken);
    }

    private void StoreResponseCompletionSource(string requestID, TaskCompletionSource<HttpResponseDto> responseCompletionSource, ResponseCompletionSourcesManagement responseCompletionSourcesManagement)
    {
        // Store the response completion source in a dictionary or cache based on requestID
        responseCompletionSourcesManagement.Sources.TryAdd(requestID, responseCompletionSource);
    }

    private async Task<string> GetRequestDataAsync(HttpRequest request)
    {
        var requestMethod = request.Method;

        var requestModel = new HttpRequestDto();
        requestModel.RequestUrl = request.GetDisplayUrl();
        requestModel.Method = requestMethod;
        var headers = new List<KeyValuePair<string, IEnumerable<string>>>();
        foreach (var header in request.Headers)
            headers.Add(new KeyValuePair<string, IEnumerable<string>>(header.Key, header.Value));
        requestModel.Headers = headers;

        if (!HttpMethods.IsGet(requestMethod) &&
            !HttpMethods.IsHead(requestMethod) &&
            !HttpMethods.IsDelete(requestMethod) &&
            !HttpMethods.IsTrace(requestMethod))
        {
            requestModel.Bytes = await ReadStreamInBytesAsync(request.Body);
        }

        return JsonSerializer.Serialize(requestModel);
    }

    public static async Task<byte[]> ReadStreamInBytesAsync(Stream input)
    {
        byte[] buffer = new byte[16 * 1024];
        using (MemoryStream ms = new MemoryStream())
        {
            int read;
            while ((read = await input.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                ms.Write(buffer, 0, read);
            }
            return ms.ToArray();
        }
    }
}
