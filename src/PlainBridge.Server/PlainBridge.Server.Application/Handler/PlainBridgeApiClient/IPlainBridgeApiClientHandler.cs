using PlainBridge.SharedApplication.DTOs;

namespace PlainBridge.Server.Application.Handler.PlainBridgeApiClient;

public interface IPlainBridgeApiClientHandler
{
    Task<IList<HostApplicationDto>?> GetHostApplicationsAsync(CancellationToken cancellationToken);
    Task<IList<ServerApplicationDto>?> GetServerApplicationsAsync(CancellationToken cancellationToken);
}