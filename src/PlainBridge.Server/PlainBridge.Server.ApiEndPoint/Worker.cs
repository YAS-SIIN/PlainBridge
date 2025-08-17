
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PlainBridge.Server.Application.Services.AppProjectConsumer;
using PlainBridge.Server.Application.Services.HttpRequestProxy;
using PlainBridge.Server.Application.Services.ServerBus;

namespace PlainBridge.Server.ApiEndPoint;

public class Worker(ILogger<Worker> _logger, IServiceScopeFactory _serviceScopeFactory, IHttpRequestProxyService _httpRequestProxyService, IServerBusService _serverBusService, IServerApplicationConsumerService _serverApplicationConsumerService) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        
        //var _httpRequestProxyService = scope.ServiceProvider.GetRequiredService<IHttpRequestProxyService>();
        //var _serverBusService = scope.ServiceProvider.GetRequiredService<IServerBusService>();
        //var _serverApplicationConsumerService = scope.ServiceProvider.GetRequiredService<IServerApplicationConsumerService>();

        await _httpRequestProxyService.InitializeConsumerAsync(cancellationToken);
        await _serverBusService.InitializeConsumerAsync(cancellationToken);
        await _serverApplicationConsumerService.InitializeConsumerAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
     
}
