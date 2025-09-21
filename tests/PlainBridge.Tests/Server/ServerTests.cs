
using Aspire.Hosting.Testing;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using PlainBridge.Tests.Utils;
using System.Net.WebSockets;
using System.Text;

namespace PlainBridge.Tests.Server;

public class ServerTests : IClassFixture<AppHostIntegrationTestRunFixture>
{
    private readonly AppHostIntegrationTestRunFixture _fixture;

    private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
    public ServerTests(AppHostIntegrationTestRunFixture fixture)
    {
        _fixture = fixture;
    }


    [Fact]
    public async Task SendARequestToServerProject_WhenEveryThingIsOk_ShouldReturnData()
    {
        // Arrange
        _cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(60));

        var res = await _fixture.InjectedDistributedApplication.ResourceNotifications
        .WaitForResourceHealthyAsync("api-endpoint", _cancellationTokenSource.Token);

        // Assert
        Assert.NotNull(res);
    }

     

}
