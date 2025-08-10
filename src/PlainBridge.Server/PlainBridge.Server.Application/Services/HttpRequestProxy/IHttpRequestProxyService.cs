
namespace PlainBridge.Server.Application.Services.HttpRequestProxy;

public interface IHttpRequestProxyService
{
    Task InitializeConsumerAsync(CancellationToken cancellationToken);
}