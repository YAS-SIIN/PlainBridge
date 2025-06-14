using PlainBridge.SharedApplication.DTOs;

namespace PlainBridge.Server.Application.Services.HostApplication
{
    public interface IHostApplicationService
    {
        HostApplicationDto GetByHost(string host);
        Task UpdateHostApplicationAsync(CancellationToken cancellationToken);
    }
}