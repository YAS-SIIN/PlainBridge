using PlainBridge.SharedApplication.DTOs;

namespace PlainBridge.Server.Application.Handler.PlainBridgeApiClient
{
    public interface IPlainBridgeApiClientHandler
    {
        Task<IList<ServerApplicationDto>?> GetAppProjectsAsync(CancellationToken cancellationToken);
        Task<IList<HostApplicationDto>?> GetProjectsAsync(CancellationToken cancellationToken);
    }
}