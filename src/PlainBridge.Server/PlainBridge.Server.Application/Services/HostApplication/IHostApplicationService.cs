
namespace PlainBridge.Server.Application.Services.HostApplication;

public interface IHostApplicationService
{
    Task UpdateServerApplicationAsync(CancellationToken cancellationToken);
}