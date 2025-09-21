 
using Aspire.Hosting.Testing; 
using PlainBridge.Tests.Utils; 

namespace PlainBridge.Tests.Api;

[Collection("AppHostIntegrationTestRun")]
public class ApiTests : IClassFixture<AppHostIntegrationTestRunFixture>
{
    private readonly AppHostIntegrationTestRunFixture _fixture;

    private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
    public ApiTests(AppHostIntegrationTestRunFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task SendARequestToApiProject_WhenEveryThingIsOk_ShouldReturnData()
    {
        // Arrange
        _cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(60));

        var res = await _fixture.InjectedDistributedApplication.ResourceNotifications
       .WaitForResourceHealthyAsync("api-endpoint", _cancellationTokenSource.Token);

        var httpClient = _fixture.InjectedDistributedApplication.CreateHttpClient("api-endpoint");

        var port = int.Parse(Environment.GetEnvironmentVariable("API_PROJECT_PORT") ?? "5001");
        // Get the server URL that Aspire allocated (http/https) 

        httpClient.BaseAddress = new Uri($"https://localhost:{port}");
        // Act
        var result = await httpClient.GetAsync("/", _cancellationTokenSource.Token);
        var response = await result.Content.ReadAsStringAsync(_cancellationTokenSource.Token);
 
        // Assert
        Assert.NotNull(response); 
    }
      

}
