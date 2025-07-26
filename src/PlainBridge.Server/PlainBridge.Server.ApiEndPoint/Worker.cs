
using PlainBridge.Server.Application.Services.AppProjectConsumer;
using PlainBridge.Server.Application.Services.HttpRequestProxy;
using PlainBridge.Server.Application.Services.ServerBus;

namespace PlainBridge.Server.ApiEndPoint;

public class Worker(ILogger<Worker> _logger, IHttpRequestProxyService _httpRequestProxyService, IServerBusService _serverBusService, IServerApplicationConsumerService _serverApplicationConsumerService)
{
  
    public async Task StartAsync(CancellationToken cancellationToken)
    {

        await _httpRequestProxyService.InitializeConsumerAsync(cancellationToken);
        await _serverBusService.InitializeConsumerAsync(cancellationToken);
        await _serverApplicationConsumerService.InitializeConsumerAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
     
}
