


using Aspire.Hosting;
using Aspire.Hosting.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace PlainBridge.Tests.Utils;
 
[CollectionDefinition("Infrastructure Collection")]
public class InfrastructureCollection : ICollectionFixture<AppHostIntegrationTestRunFixture> { }

public class AppHostIntegrationTestRunFixture : IAsyncLifetime
{
    private IDistributedApplicationTestingBuilder _distributedApplicationTestingBuilder;
    public DistributedApplication InjectedDistributedApplication;
    public AppHostIntegrationTestRunFixture()
    {
        _distributedApplicationTestingBuilder = default!;
        InjectedDistributedApplication = default!;
    }
    public async Task DisposeAsync()
    {
        await InjectedDistributedApplication.StopAsync();
        await _distributedApplicationTestingBuilder.DisposeAsync();
        await Task.CompletedTask;
    }

    public async Task InitializeAsync()
    {
        Environment.SetEnvironmentVariable("API_PROJECT_PORT", "5001");
        Environment.SetEnvironmentVariable("SERVER_PROJECT_PORT", "5002");
        Environment.SetEnvironmentVariable("IDS_PROJECT_PORT", "5003");
        Environment.SetEnvironmentVariable("CLIENT_PROJECT_PORT", "5005");

        _distributedApplicationTestingBuilder = await DistributedApplicationTestingBuilder.CreateAsync<Projects.PlainBridge_AppHost>();
        _distributedApplicationTestingBuilder.Services.AddLogging(logging => logging
           .AddConsole() // Outputs logs to console
           .AddFilter("Default", LogLevel.Information)
           .AddFilter("Microsoft.AspNetCore", LogLevel.Warning)
           .AddFilter("Aspire.Hosting.Dcp", LogLevel.Warning));
         
        InjectedDistributedApplication = await _distributedApplicationTestingBuilder.BuildAsync();

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(40));

        //await InjectedDistributedApplication.ResourceNotifications.WaitForResourceHealthyAsync(
        //           "api-endpoint",
        //           cts.Token);

        //await InjectedDistributedApplication.ResourceNotifications.WaitForResourceHealthyAsync(
        //           "server-endpoint",
        //           cts.Token);

        //await InjectedDistributedApplication.ResourceNotifications.WaitForResourceHealthyAsync(
        //           "client-endpoint",
        //           cts.Token);

        //await InjectedDistributedApplication.ResourceNotifications.WaitForResourceHealthyAsync(
        //           "identityserver-endpoint",
        //           cts.Token);

        await InjectedDistributedApplication.StartAsync();

    }
}
