
namespace PlainBridge.Client.Application.Handler.HttpRequest;

public interface IHttpRequestHandler
{
    Task InitializeHttpRequestConsumer(string username, CancellationToken cancellationToken);
}