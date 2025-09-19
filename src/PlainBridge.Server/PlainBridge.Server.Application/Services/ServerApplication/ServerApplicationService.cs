
using Microsoft.Extensions.Logging; 
using PlainBridge.Server.Application.Management.Cache;
using PlainBridge.Server.Application.Services.PlainBridgeApiClient;
using PlainBridge.SharedApplication.Enums;

namespace PlainBridge.Server.Application.Services.ServerApplication;

public class ServerApplicationService(ILogger<ServerApplicationService> _logger, 
    IPlainBridgeApiClientService _plainBridgeApiClientHandler,
    ICacheManagement _cache) : IServerApplicationService
{ 
    public async Task UpdateServerApplicationAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating server applications from API.");
        var serverApplications = await _plainBridgeApiClientHandler.GetServerApplicationsAsync(cancellationToken);

        if (serverApplications == null)
        {
            _logger.LogWarning("No host applications received from API.");
            return;
        }

        foreach (var serverApplication in serverApplications.Where(x => x.State == RowStateEnum.Active))
        { 
            await _cache.SetGetServerApplicationAsync(serverApplication!.UserName!, serverApplication.InternalPort, serverApplication, cancellationToken);
            await _cache.SetGetServerApplicationAsync(serverApplication.AppId.ToString(), serverApplication, cancellationToken);

            _logger.LogInformation("Server application cached: {ServerApplication}", serverApplication);
        }
        _logger.LogInformation("Host applications update completed.");
    }

}
