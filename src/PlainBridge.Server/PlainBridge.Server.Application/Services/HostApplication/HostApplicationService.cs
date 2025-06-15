 
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
        _logger.LogInformation("Getting HostApplication by host: {Host}", host);
        if (string.IsNullOrEmpty(host))
        {
            _logger.LogError("Host parameter is null or empty.");
            throw new ArgumentNullException(nameof(host));
        }

        if (!_cache.TryGetHostApplication(host, out HostApplicationDto hostApplication))
        {
            _logger.LogWarning("Host application not found for host: {Host}", host);
            throw new NotFoundException("Host application not found");
        }
        _logger.LogInformation("Host application found for host: {Host}", host);
        return hostApplication;
    }

    public async Task UpdateHostApplicationAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating host applications from API.");
        var hostApplications = await _plainBridgeApiClientHandler.GetHostApplicationsAsync(CancellationToken.None);

        if (hostApplications == null)
        {
            _logger.LogWarning("No host applications received from API.");
            return;
        }

        foreach (var project in hostApplications.Where(x => x.State == RowStateEnum.Active))
        {
            var projectHost = project.GetProjectHost(_applicationSetting.DefaultDomain);
            _cache.SetHostApplication(projectHost, project);
            _logger.LogInformation("Host application cached: {ProjectHost}", projectHost);
        }
        _logger.LogInformation("Host applications update completed.");
    }
}
