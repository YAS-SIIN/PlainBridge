
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PlainBridge.Server.Application.Services.AppProjectConsumer;
using PlainBridge.Server.Application.Services.HttpRequestProxy;
using PlainBridge.Server.Application.Services.ServerBus;

namespace PlainBridge.Server.ApiEndPoint;

public class Worker(ILogger<Worker> _logger, IServiceScopeFactory _serviceScopeFactory) : IHostedService
{
  
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        
        var httpRequestProxyService = scope.ServiceProvider.GetRequiredService<IHttpRequestProxyService>();
        var serverBusService = scope.ServiceProvider.GetRequiredService<IServerBusService>();
        var serverApplicationConsumerService = scope.ServiceProvider.GetRequiredService<IServerApplicationConsumerService>();

        await httpRequestProxyService.InitializeConsumerAsync(cancellationToken);
        await serverBusService.InitializeConsumerAsync(cancellationToken);
        await serverApplicationConsumerService.InitializeConsumerAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
     
}
