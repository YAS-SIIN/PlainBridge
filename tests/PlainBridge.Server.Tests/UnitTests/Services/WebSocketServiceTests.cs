
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using PlainBridge.Server.Application.DTOs;
using PlainBridge.Server.Application.Management.Cache;
using PlainBridge.Server.Application.Services.WebSocket;
using RabbitMQ.Client;

namespace PlainBridge.Server.Tests.UnitTests.Services;

[Collection("ServerUnitTestRun")]
public class WebSocketServiceTests
{
    private readonly IWebSocketService _webSocketService;

    private readonly Mock<ILogger<WebSocketService>> _mockLogger = new();
    private readonly Mock<ICacheManagement> _mockCacheManagement = new();
    private readonly Mock<IConnection> _mockConnection = new();


    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    public WebSocketServiceTests()
    {
        var appSettings = Options.Create(new ApplicationSettings
        {
            DefaultDomain = ".example.com",
            PlainBridgeApiAddress = "http://api.local",
            PlainBridgeIdsUrl = "http://ids.local",
            PlainBridgeIdsClientId = "client",
            PlainBridgeIdsClientSecret = "secret",
            PlainBridgeIdsScope = "scope",
            PlainBridgeUseHttp = true,
            HybridDistributedCacheExpirationTime = "24:00:00",
            HybridMemoryCacheExpirationTime = "00:30:00",
            HybridCacheMaximumPayloadBytes = 10485760,
            HybridCacheMaximumKeyLength = 512
        });

        _webSocketService = new WebSocketService(_mockLogger.Object,
            _mockCacheManagement.Object, _mockConnection.Object, appSettings);
    }


    [Fact]
    public async Task InitializeConsumerAsync_ShouldSetupWebSocketConnection()
    {
        // Arrange  
        var _mockChannel = new Mock<IChannel>();
        _mockConnection.Setup(m => m.CreateChannelAsync(default, _cancellationToken)).ReturnsAsync(_mockChannel.Object);
        // Act
        await _webSocketService.InitializeConsumerAsync(_cancellationToken);
        // Assert
        _mockConnection.Verify(m => m.CreateChannelAsync(default, _cancellationToken), Times.Once);
    }
}
