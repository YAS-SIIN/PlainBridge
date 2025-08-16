
using PlainBridge.SharedApplication.DTOs;

namespace PlainBridge.Server.Application.Services.HostApplication;

public interface IHostApplicationService
{
    Task<HostApplicationDto> GetByHostAsync(string host, CancellationToken cancellationToken);
    Task UpdateHostApplicationAsync(CancellationToken cancellationToken);
}