 
using Microsoft.Extensions.Logging;

using PlainBridge.Api.Domain.Entities;
using PlainBridge.Server.Application.DTOs;
using PlainBridge.Server.Application.Handler.Cache;
using PlainBridge.Server.Application.Handler.PlainBridgeApiClient;
using PlainBridge.SharedApplication.DTOs;
using PlainBridge.SharedApplication.Enums;

using System;
namespace PlainBridge.Server.Application.Services.ServerApplication;

public class ServerApplicationService
{
    private readonly ILogger<ServerApplicationService> _logger; 
  
    private readonly IPlainBridgeApiClientHandler _plainBridgeApiClientHandler;
    private readonly ICache _cache;
    private readonly ApplicationSetting _applicationSetting;
    public ServerApplicationService(ILogger<ServerApplicationService> logger, IPlainBridgeApiClientHandler plainBridgeApiClientHandler, ApplicationSetting applicationSetting, ICache cache)
    {
        _logger = logger;
        this._plainBridgeApiClientHandler = plainBridgeApiClientHandler;
        _applicationSetting = applicationSetting;
        _cache = cache;
    }


    public async Task UpdateAppProjectsAsync(CancellationToken cancellationToken)
    {
        var serverApplications = await _plainBridgeApiClientHandler.GetServerApplicationsAsync(CancellationToken.None);

        var serverApplicationDictionary = new Dictionary<string, ServerApplicationDto>();
        foreach (var serverApplication in serverApplications.Where(x => x.State == RowStateEnum.Active))
        { 
            _cache.SetServerApplication(serverApplication.AppId, serverApplication);
        }
    }

}
