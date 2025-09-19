

using Microsoft.Extensions.Logging;
using Moq;
using PlainBridge.Server.Application.Services.PlainBridgeApiClient;
using PlainBridge.Server.Application.Services.ServerBus;
using RabbitMQ.Client;

namespace PlainBridge.Server.Tests.UnitTests.Services;

[Collection("ServerUnitTestRun")]
public class ServerBusServiceTests
{
    private readonly IServerBusService _serverBusService;

    private readonly Mock<ILogger<ServerBusService>> _mockLogger = new();
    private readonly Mock<IPlainBridgeApiClientService> _mockPlainBridgeApiClientService = new(); 
    private readonly Mock<IConnection> _mockConnection = new();
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public ServerBusServiceTests()
    {
        _serverBusService = new ServerBusService(_mockLogger.Object, _mockConnection.Object, _mockPlainBridgeApiClientService.Object);
    }

    [Fact]
    public async Task InitializeConsumerAsync_ShouldSetupQueueAndInitializeConsumer()
    {
        // Arrange  
        var _mockChannel = new Mock<IChannel>();
        _mockConnection.Setup(m => m.CreateChannelAsync(default, _cancellationToken)).ReturnsAsync(_mockChannel.Object);
        // Act
        await _serverBusService.InitializeConsumerAsync(_cancellationToken);
        // Assert
        _mockConnection.Verify(m => m.CreateChannelAsync(default, _cancellationToken), Times.Once);
    }
}
