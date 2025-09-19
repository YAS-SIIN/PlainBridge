using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PlainBridge.Server.Application.DTOs;

namespace PlainBridge.Server.Tests.Utils;

 
public class ServerUnitTestRunFixture : IAsyncLifetime
{
    public ServiceCollection Services { get; } = new ServiceCollection();

    public ServerUnitTestRunFixture()
    {
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
