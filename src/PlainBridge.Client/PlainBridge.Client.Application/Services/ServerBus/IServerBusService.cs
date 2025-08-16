
namespace PlainBridge.Client.Application.Services.ServerBus;

public interface IServerBusService
{
    Task RequestServerApplicationsAsync(string username, CancellationToken cancellationToken);
}