
namespace PlainBridge.Client.Application.Services.ServerBus;

public interface IServerBusService
{
    Task RequestAppProjects(string username, CancellationToken cancellationToken);
}