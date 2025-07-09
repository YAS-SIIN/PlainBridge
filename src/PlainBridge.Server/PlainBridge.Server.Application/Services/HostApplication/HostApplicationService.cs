
using Microsoft.Extensions.Logging; 
using PlainBridge.Server.Application.DTOs;
using PlainBridge.Server.Application.Handler.PlainBridgeApiClient;
using PlainBridge.Server.Application.Management.Cache;
using PlainBridge.SharedApplication.DTOs;
using PlainBridge.SharedApplication.Enums;

using System;
namespace PlainBridge.Server.Application.Services.HostApplication;

public class HostApplicationService(ILogger<HostApplicationService> _logger, IPlainBridgeApiClientHandler _plainBridgeApiClientHandler, ICacheManagement _cache) : IHostApplicationService
{


    public HostApplicationDto GetByHost(string host)
    {
        if (string.IsNullOrEmpty(host))
            throw new ArgumentNullException(nameof(host));

        if (!_cache.TryGetHostApplication(host, out HostApplicationDto serverApplicationS)) throw new ApplicationException("Project not found");
        return serverApplicationS;
    }

    public async Task UpdateServerApplicationAsync(CancellationToken cancellationToken)
    {
        var serverApplications = await _plainBridgeApiClientHandler.GetServerApplicationsAsync(CancellationToken.None);

        var serverApplicationDictionary = new Dictionary<string, HostApplicationDto>();
        if (serverApplications is not null)
        {
            foreach (var serverApplication in serverApplications.Where(x => x.State == RowStateEnum.Active))
            {
                _cache.SetServerApplication(serverApplication.UserName, serverApplication.InternalPort, serverApplication);
                _cache.SetServerApplication(serverApplication.AppId, serverApplication);
            }
        }
    }

}
