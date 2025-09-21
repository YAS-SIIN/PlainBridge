

using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using PlainBridge.SharedApplication.DTOs;
using PlainBridge.Server.ApiEndPoint;
namespace PlainBridge.IdentityServer.Tests.IntegrationTests.Utils;


public class IntegrationTestRunFixture : IAsyncLifetime
{
    public WebApplicationFactory<Program> WebApplicationFactory { get; }
    public HttpClient InjectedHttpClient { get; }
    public IntegrationTestRunFixture()
    {
        WebApplicationFactory = new WebApplicationFactory<Program>();
        InjectedHttpClient = WebApplicationFactory.CreateDefaultClient();
    }

    public async Task InitializeAsync()
    {
       
        await Task.CompletedTask;
    }
    public Task DisposeAsync()
    {
        InjectedHttpClient.Dispose();
        return Task.CompletedTask;
    }

}
