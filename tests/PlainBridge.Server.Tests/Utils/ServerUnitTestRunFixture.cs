using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PlainBridge.Server.Application.DTOs;

namespace PlainBridge.Server.Tests.Utils;

 
public class ServerUnitTestRunFixture : IAsyncLifetime
{
    public IOptions<ApplicationSettings> AppSettings { get; }
    public ServiceCollection Services { get; } = new ServiceCollection();

    public ServerUnitTestRunFixture()
    {
        AppSettings = Options.Create(new ApplicationSettings
        {
            DefaultDomain = ".example.com",
            PlainBridgeApiAddress = "http://api.local",
            PlainBridgeIdsUrl = "http://ids.local",
            PlainBridgeIdsClientId = "client",
            PlainBridgeIdsClientSecret = "secret",
            PlainBridgeIdsScope = "scope",
            PlainBridgeUseHttp = true,
            HybridDistributedCacheExpirationTime = "24:00:00",
            HybridMemoryCacheExpirationTime = "00:30:00",
            HybridCacheMaximumPayloadBytes = 10485760,
            HybridCacheMaximumKeyLength = 512
        }); 
    }

    public async Task InitializeAsync()
    {
        Services.AddHybridCache(); 
        var sp = Services.BuildServiceProvider();

        await Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        Services.Clear();
        await Task.CompletedTask;
    }
}
