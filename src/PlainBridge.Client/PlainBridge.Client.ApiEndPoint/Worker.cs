using System.Text.Json;
using PlainBridge.Client.Application.Handler.HttpRequest;
using PlainBridge.Client.Application.Handler.WebSocket;
using PlainBridge.Client.Application.Services.ClientBus;
using PlainBridge.Client.Application.Services.ServerBus;
using PlainBridge.Client.Application.Services.Signal;
using PlainBridge.SharedApplication.DTOs;

namespace PlainBridge.Client.ApiEndPoint;

public class Worker(ILogger<Worker> _logger, ISignalService _signalService, IHttpRequestHandler _httpRequestHandler, IWebSocketHandler _webSocketHandler, IClientBusService _clientBusService, IServerBusService _serverBusService) : BackgroundService
{
 
    protected override Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting service ...");

        var _ = Task.Run(async () =>
        {
            var fileName = "profile";
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Test")
                fileName = "profile.test";

            if (!File.Exists(fileName))
                _signalService.WaitOne();

            var content = File.ReadAllText(fileName);
            var profile = JsonSerializer.Deserialize<UserProfileViewDto>(content);
             
            await _httpRequestHandler.InitializeHttpRequestConsumerAsync(profile!.Username, cancellationToken);
            await _webSocketHandler.InitializeWebSocketConsumerAsync(profile!.Username, cancellationToken);
            await _clientBusService.InitializeConsumerAsync(profile.Username, cancellationToken);
            await _serverBusService.RequestServerApplicationsAsync(profile.Username, cancellationToken);
             
            await Task.Delay(Timeout.Infinite, cancellationToken);
        });

        return Task.CompletedTask;
    }
}
