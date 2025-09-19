 
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PlainBridge.Server.Application.DTOs;
using PlainBridge.Server.Application.Management.Cache;
using PlainBridge.Server.Application.Services.PlainBridgeApiClient;
using PlainBridge.SharedApplication.DTOs;
using PlainBridge.SharedApplication.Enums;
using PlainBridge.SharedApplication.Exceptions;
namespace PlainBridge.Server.Application.Services.HostApplication;

public class HostApplicationService(ILogger<HostApplicationService> _logger, 
    IPlainBridgeApiClientService _plainBridgeApiClientHandler, 
    ICacheManagement _cache, 
    IOptions<ApplicationSettings> _appSettings) : IHostApplicationService
{


    public async Task<HostApplicationDto> GetByHostAsync(string host, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(host))
            throw new ArgumentNullException(nameof(host));

        var hostApplication = await _cache.SetGetHostApplicationAsync(host, cancellationToken: cancellationToken);
        if (hostApplication is null || hostApplication == default)
        {
            _logger.LogWarning("Host application not found for host: {Host}", host);
            throw new NotFoundException("Host application not found");
        }

        return hostApplication;
    }

    public async Task UpdateHostApplicationAsync(CancellationToken cancellationToken)
    {
        var hostApplications = await _plainBridgeApiClientHandler.GetHostApplicationsAsync(cancellationToken);
         
        if (hostApplications is not null && hostApplications.Any())
        {
            foreach (var hostApplication in hostApplications.Where(x => x.State == RowStateEnum.Active))
            {
                await _cache.SetGetHostApplicationAsync(hostApplication.GetProjectHost(_appSettings.Value.DefaultDomain), hostApplication, cancellationToken); 
            }
        }
    }

}
