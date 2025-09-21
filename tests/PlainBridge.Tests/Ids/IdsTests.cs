

using PlainBridge.Tests.Utils;

namespace PlainBridge.Tests.Ids;

 
[Collection("AppHostIntegrationTestRun")]
public class IdsTests : IClassFixture<AppHostIntegrationTestRunFixture>
{
    private readonly AppHostIntegrationTestRunFixture _fixture;

    private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
    public IdsTests(AppHostIntegrationTestRunFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task SendARequestToClientProject_WhenEveryThingIsOk_ShouldReturnData()
    {
        // Arrange
        _cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(60));

        var res = await _fixture.InjectedDistributedApplication.ResourceNotifications
        .WaitForResourceHealthyAsync("ids-endpoint", _cancellationTokenSource.Token);

        // Assert
        Assert.NotNull(res);
    }
}
