using PlainBridge.Server.Application.Services.ApiExternalBus;
using PlainBridge.Server.Application.Services.HttpRequestProxy;

namespace PlainBridge.Server.ApiEndPoint;

public class Worker
{
    private readonly ILogger<Worker> _logger;
    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }
    public async Task StartAsync(CancellationToken cancellationToken)
    {

        await Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
