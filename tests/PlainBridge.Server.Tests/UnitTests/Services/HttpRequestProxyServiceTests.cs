

using Microsoft.Extensions.Logging;
using Moq;
using PlainBridge.Server.Application.Management.ResponseCompletionSources;
using PlainBridge.Server.Application.Services.ApiExternalBus;
using PlainBridge.Server.Application.Services.HostApplication;
using PlainBridge.Server.Application.Services.HttpRequestProxy;
using PlainBridge.Server.Application.Services.ServerApplication;
using RabbitMQ.Client;

namespace PlainBridge.Server.Tests.UnitTests.Services;

[Collection("ServerUnitTestRun")]
public class HttpRequestProxyServiceTests
{
    private readonly IHttpRequestProxyService _httpRequestProxyService;

    private readonly Mock<ILogger<HttpRequestProxyService>> _mockLogger = new();
    private readonly Mock<IApiExternalBusService> _mockApiExternalBusService = new();
    private readonly Mock<IConnection> _mockConnection = new();
    private readonly ResponseCompletionSourcesManagement _responseCompletionSourcesManagement = new();
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    public HttpRequestProxyServiceTests()
    {
        _httpRequestProxyService = new HttpRequestProxyService(
            _mockLogger.Object,
            _mockApiExternalBusService.Object,
            _mockConnection.Object,
            _responseCompletionSourcesManagement
        );
    }

    [Fact]
    public async Task InitializeConsumerAsync_ShouldSetupQueueAndInitializeConsumer()
    {
        // Arrange  
        var _mockChannel = new Mock<IChannel>();
        _mockConnection.Setup(m => m.CreateChannelAsync(default, _cancellationToken)).ReturnsAsync(_mockChannel.Object);
        // Act
        await _httpRequestProxyService.InitializeConsumerAsync(_cancellationToken);
        // Assert
        _mockConnection.Verify(m => m.CreateChannelAsync(default, _cancellationToken), Times.Once);
        _mockApiExternalBusService.Verify(m => m.InitializeAsync(_cancellationToken), Times.Once);  
 
    }   

}
