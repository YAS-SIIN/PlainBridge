
namespace PlainBridge.Client.Application.Handler.HttpRequest;

public interface IHttpRequestHandler
{
    Task InitializeHttpRequestConsumerAsync(string username, CancellationToken cancellationToken);
}