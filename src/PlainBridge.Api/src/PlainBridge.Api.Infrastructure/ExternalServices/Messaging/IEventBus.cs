 

namespace PlainBridge.Api.Infrastructure.ExternalServices.Messaging;
 
public interface IEventBus
{
    Task PublishAsync<T>(T message, CancellationToken cancellationToken) where T : class;
}