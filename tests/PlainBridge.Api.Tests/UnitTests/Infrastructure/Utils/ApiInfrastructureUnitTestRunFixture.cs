 
using Microsoft.Extensions.Options;
using PlainBridge.Api.Infrastructure.DTOs;

namespace PlainBridge.Api.Tests.UnitTests.Infrastructure.Utils;


public class ApiInfrastructureUnitTestRunFixture : IAsyncLifetime
{ 
    public ServiceCollection Services { get; } = new ServiceCollection();

    public ApiInfrastructureUnitTestRunFixture()
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
