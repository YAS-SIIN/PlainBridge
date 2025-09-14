using PlainBridge.SharedApplication.DTOs;

namespace PlainBridge.Server.Application.Services.PlainBridgeApiClient;

public interface IPlainBridgeApiClientService
{
    Task<IList<HostApplicationDto>?> GetHostApplicationsAsync(CancellationToken cancellationToken);
    Task<IList<ServerApplicationDto>?> GetServerApplicationsAsync(CancellationToken cancellationToken);
}