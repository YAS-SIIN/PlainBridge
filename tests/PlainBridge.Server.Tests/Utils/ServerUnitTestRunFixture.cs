 
using Microsoft.Extensions.DependencyInjection; 

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
