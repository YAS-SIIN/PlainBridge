
 
using Aspire.Hosting.Testing; 
using PlainBridge.Tests.Utils; 

namespace PlainBridge.Tests.Server;

public class ClientTests : IClassFixture<AppHostIntegrationTestRunFixture>
{
    private readonly AppHostIntegrationTestRunFixture _fixture;

    private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
    public ClientTests(AppHostIntegrationTestRunFixture fixture)
    {
        _fixture = fixture;
    }
     
    [Fact]
    public async Task SendARequestToClientProject_WhenEveryThingIsOk_ShouldReturnData()
    {
        // Arrange
        _cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(60));
        var httpClient = _fixture.InjectedDistributedApplication.CreateHttpClient("client-endpoint");

        httpClient.BaseAddress = new Uri("https://localhost:5005");
        // Act
        var result = await httpClient.GetAsync("/", _cancellationTokenSource.Token);
        var response = await result.Content.ReadAsStringAsync(_cancellationTokenSource.Token);
 
        // Assert
        Assert.NotNull(response); 
    }
     
     



}
