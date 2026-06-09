
namespace PlainBridge.Server.Application.Services.ServerBus
{
    public interface IServerBusService
    {
        Task InitializeConsumerAsync(CancellationToken cancellationToken);
    }
}