
namespace PlainBridge.Client.Application.Services.ClientBus;

public interface IClientBusService
{
    Task InitializeConsumerAsync(string username, CancellationToken cancellationToken);
}