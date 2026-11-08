using Aspire.Hosting.Testing;

using PlainBridge.Tests.Utils;

namespace PlainBridge.Tests.Api;

[Collection("AppHostIntegrationTestRun")]
public class ApiTests : IClassFixture<AppHostIntegrationTestRunFixture>
{
    private readonly AppHostIntegrationTestRunFixture _fixture;

    public ApiTests(AppHostIntegrationTestRunFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task SendARequestToApiProject_WhenEveryThingIsOk_ShouldReturnData()
    {
        using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(60));

        await _fixture.InjectedDistributedApplication.ResourceNotifications.WaitForResourceHealthyAsync(
                "api-endpoint", cancellationTokenSource.Token);

        using var httpClient =  _fixture.InjectedDistributedApplication.CreateHttpClient("api-endpoint");

        var response = await httpClient.GetAsync("/health", cancellationTokenSource.Token);

        response.EnsureSuccessStatusCode();
    }
}