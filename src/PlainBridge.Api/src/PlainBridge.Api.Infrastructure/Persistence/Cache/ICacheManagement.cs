namespace PlainBridge.Api.Infrastructure.Persistence.Cache;

public interface ICacheManagement
{

    Task<string?> SetGetTokenPSubAsync(string tokenp, string? value = null, CancellationToken cancellationToken = default);

    Task<string?> SetGetSubTokenAsync(string sub, string? value = null, CancellationToken cancellationToken = default);

    Task<string?> SetGetSubTokenPAsync(string sub, string value = null, CancellationToken cancellationToken = default);

    Task<string?> SetGetTokenPTokenAsync(string tokenp, string value = null, CancellationToken cancellationToken = default);

    Task<string?> SetGetSubIdTokenAsync(string sub, string value = null, CancellationToken cancellationToken = default);

    Task<string?> SetGetTokenPRefreshTokenAsync(string tokenp, string value = null, CancellationToken cancellationToken = default);
}