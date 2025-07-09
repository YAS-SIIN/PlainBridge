using PlainBridge.SharedApplication.DTOs;

namespace PlainBridge.Server.Application.Services.ServerApplication;

public interface IServerApplicationService
{
    HostApplicationDto GetByHost(string host);
    Task UpdateHostApplicationAsync(CancellationToken cancellationToken);
}