using PlainBridge.Api.Application.Services.ServerApplication;
using PlainBridge.Server.Application.Services.ApiExternalBus;
using PlainBridge.Server.Application.Services.HttpRequestProxy;
using PlainBridge.Server.Application.Services.ServerBus;

namespace PlainBridge.Server.ApiEndPoint;

public class Worker
{
    private readonly ILogger<Worker> _logger;
    private readonly IHttpRequestProxyService _httpRequestProxyService;
    private readonly IServerBusService _serverBusService;
    private readonly IServerApplicationService _serverApplicationService;
    public Worker(ILogger<Worker> logger, IHttpRequestProxyService httpRequestProxyService, IServerBusService serverBusService, IServerApplicationService serverApplicationService)
    {
        _logger = logger;
        _httpRequestProxyService = httpRequestProxyService;
        _serverBusService = serverBusService;
        _serverApplicationService = serverApplicationService;
    }
    public async Task StartAsync(CancellationToken cancellationToken)
    {

        await _httpRequestProxyService.InitializeConsumerAsync(cancellationToken);
        await _serverBusService.InitializeConsumerAsync(cancellationToken); 
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
