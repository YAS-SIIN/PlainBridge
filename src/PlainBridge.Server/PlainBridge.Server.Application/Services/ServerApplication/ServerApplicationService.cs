
using Microsoft.Extensions.Logging;

using PlainBridge.Api.Domain.Entities; 
using PlainBridge.Server.Application.DTOs;
using PlainBridge.Server.Application.Handler.PlainBridgeApiClient;
using PlainBridge.Server.Application.Management.Cache;
using PlainBridge.SharedApplication.DTOs;
using PlainBridge.SharedApplication.Enums;

using System;
namespace PlainBridge.Server.Application.Services.ServerApplication;

public class ServerApplicationService(ILogger<ServerApplicationService> _logger, IPlainBridgeApiClientHandler _plainBridgeApiClientHandler, ApplicationSetting applicationSetting, ICacheManagement _cache) : IServerApplicationService
{
   

    public async Task UpdateServerApplicationAsync(CancellationToken cancellationToken)
    {
        var serverApplications = await _plainBridgeApiClientHandler.GetServerApplicationsAsync(CancellationToken.None);

        var serverApplicationDictionary = new Dictionary<string, ServerApplicationDto>();
        foreach (var serverApplication in serverApplications.Where(x => x.State == RowStateEnum.Active))
        {
            _cache.SetServerApplication(serverApplication.InternalPort, serverApplication);
            _cache.SetServerApplication(serverApplication.AppId, serverApplication);
        }
    }

}
