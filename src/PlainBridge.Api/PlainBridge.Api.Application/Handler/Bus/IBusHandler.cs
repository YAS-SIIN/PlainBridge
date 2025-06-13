
namespace PlainBridge.Api.Application.Handler.Bus
{
    public interface IBusHandler
    {
        Task PublishAsync<T>(T message, CancellationToken cancellationToken) where T : class;
    }
}