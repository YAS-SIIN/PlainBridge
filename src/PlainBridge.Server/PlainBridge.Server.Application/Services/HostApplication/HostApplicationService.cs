 
using Microsoft.Extensions.Logging;
 
using PlainBridge.Server.Application.DTOs;
using PlainBridge.Server.Application.Handler.PlainBridgeApiClient;
using PlainBridge.Server.Application.Management.Cache;
using PlainBridge.SharedApplication.DTOs;
using PlainBridge.SharedApplication.Enums;
using PlainBridge.SharedApplication.Exceptions;

using System;

namespace PlainBridge.Server.Application.Services.HostApplication;

public class HostApplicationService(ILogger<HostApplicationService> _logger, IPlainBridgeApiClientHandler _plainBridgeApiClientHandler, ApplicationSetting _applicationSetting, ICacheManagement _cache) : IHostApplicationService
{ 

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
