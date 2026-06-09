
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace PlainBridge.Api.Infrastructure.Persistence.Cache;

public class CacheManagement(ILogger<CacheManagement> _logger, HybridCache _hybridCache) : ICacheManagement
{

    public async Task<string?> SetGetTokenPSubAsync(string tokenp, string? value = default, CancellationToken cancellationToken = default!) => 
        await _hybridCache.GetOrCreateAsync($"tokenpsub:{tokenp}", async ct => await Task.FromResult(value), cancellationToken: cancellationToken);


    public async Task<string?> SetGetSubTokenAsync(string sub, string? value = default, CancellationToken cancellationToken = default!) => 
        await
        _hybridCache.GetOrCreateAsync($"subtoken:{sub}", async ct => await Task.FromResult(value), cancellationToken: cancellationToken);


    public async Task<string?> SetGetSubTokenPAsync(string sub, string value = default!, CancellationToken cancellationToken = default!) => 
        await _hybridCache.GetOrCreateAsync($"subtokenp:{sub}", async ct => await Task.FromResult(value), cancellationToken: cancellationToken);


    public async Task<string?> SetGetTokenPTokenAsync(string tokenp, string value = default!, CancellationToken cancellationToken = default!) => 
        await
        _hybridCache.GetOrCreateAsync($"tokenptoken:{tokenp}", async ct => await Task.FromResult(value), cancellationToken: cancellationToken);

    public async Task<string?> SetGetSubIdTokenAsync(string sub, string value = default!, CancellationToken cancellationToken = default!) => 
        await _hybridCache.GetOrCreateAsync($"subidtoken:{sub}", async ct => await Task.FromResult(value), cancellationToken: cancellationToken);

    public async Task<string?> SetGetTokenPRefreshTokenAsync(string tokenp, string value = default!, CancellationToken cancellationToken = default!) => 
        await _hybridCache.GetOrCreateAsync($"tokenprefreshtoken:{tokenp}", async ct => await Task.FromResult(value), cancellationToken: cancellationToken);

}
