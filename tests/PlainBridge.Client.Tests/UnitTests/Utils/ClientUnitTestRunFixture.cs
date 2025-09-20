using Microsoft.Extensions.DependencyInjection;

namespace PlainBridge.Client.Tests.UnitTests.Utils;
 

public class ClientUnitTestRunFixture : IAsyncLifetime
{
    public ServiceCollection Services { get; } = new();

    public ClientUnitTestRunFixture()
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
