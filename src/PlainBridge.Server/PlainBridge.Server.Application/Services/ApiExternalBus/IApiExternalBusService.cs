
namespace PlainBridge.Server.Application.Services.ApiExternalBus
{
    public interface IApiExternalBusService
    {
        Task InitializeAsync(CancellationToken cancellationToken);
    }
}