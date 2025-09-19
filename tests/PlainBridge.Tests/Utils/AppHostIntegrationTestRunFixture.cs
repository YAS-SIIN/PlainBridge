


using Aspire.Hosting;
using Aspire.Hosting.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace PlainBridge.Tests.Utils;

public class AppHostIntegrationTestRunFixture : IAsyncLifetime
{
    private IDistributedApplicationTestingBuilder _distributedApplicationTestingBuilder;
    private DistributedApplication _distributedApplication;
    public AppHostIntegrationTestRunFixture()
    {
        _distributedApplicationTestingBuilder = default!;
        _distributedApplication = default!;
    }
    public async Task DisposeAsync()
    {
        await _distributedApplication.StopAsync();
        await _distributedApplicationTestingBuilder.DisposeAsync();
        await Task.CompletedTask;
    }

    public async Task InitializeAsync()
    {
        _distributedApplicationTestingBuilder = await DistributedApplicationTestingBuilder.CreateAsync<Projects.PlainBridge_AppHost>();
        _distributedApplicationTestingBuilder.Services.AddLogging(logging => logging
           .AddConsole() // Outputs logs to console
           .AddFilter("Default", LogLevel.Information)
           .AddFilter("Microsoft.AspNetCore", LogLevel.Warning)
           .AddFilter("Aspire.Hosting.Dcp", LogLevel.Warning));

         _distributedApplication = await _distributedApplicationTestingBuilder.BuildAsync();

        await _distributedApplication.StartAsync();

    }
}
