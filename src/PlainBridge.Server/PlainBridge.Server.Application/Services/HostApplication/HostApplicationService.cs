

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using PlainBridge.Server.Application.DTOs;
using PlainBridge.Server.Application.Handler.Cache;
using PlainBridge.Server.Application.Handler.PlainBridgeApiClient;
using PlainBridge.SharedApplication.DTOs;
using PlainBridge.SharedApplication.Enums;
using PlainBridge.SharedApplication.Exceptions;

using System;

namespace PlainBridge.Server.Application.Services.HostApplication;

public class HostApplicationService
{
    private readonly ILogger<HostApplicationService> _logger;
    private readonly IPlainBridgeApiClientHandler _plainBridgeApiClientHandler; 
    private readonly ICache _cache; 
    private readonly ApplicationSetting _applicationSetting;
    public HostApplicationService(ILogger<HostApplicationService> logger, IPlainBridgeApiClientHandler plainBridgeApiClientHandler, ApplicationSetting applicationSetting, ICache cache)
    {
        _logger = logger;
        this._plainBridgeApiClientHandler = plainBridgeApiClientHandler;
        _applicationSetting = applicationSetting;
        _cache = cache;
    }


    public HostApplicationDto GetByHost(string host)
    {
        if (string.IsNullOrEmpty(host))
            throw new ArgumentNullException(nameof(host));

        if (!_cache.TryGetHostApplication(host, out HostApplicationDto hostApplication)) throw new NotFoundException("Host application not found");
        return hostApplication;
    }

    public async Task UpdateHostApplicationAsync(CancellationToken cancellationToken)
    {
        var hostApplications = await _plainBridgeApiClientHandler.GetHostApplicationsAsync(CancellationToken.None);

        var hostApplicationsDictionary = new Dictionary<string, HostApplicationDto>();
        foreach (var project in hostApplications.Where(x => x.State == RowStateEnum.Active))
            _cache.SetHostApplication(project.GetProjectHost(_applicationSetting.DefaultDomain), project);
    }


}
