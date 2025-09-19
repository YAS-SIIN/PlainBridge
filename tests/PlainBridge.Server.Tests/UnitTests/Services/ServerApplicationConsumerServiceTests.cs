

using Microsoft.Extensions.Logging;
using Moq;
using PlainBridge.Server.Application.Management.Cache;
using PlainBridge.Server.Application.Services.AppProjectConsumer;
using PlainBridge.Server.Application.Services.PlainBridgeApiClient;
using PlainBridge.Server.Application.Services.ServerApplication;
using RabbitMQ.Client;

namespace PlainBridge.Server.Tests.UnitTests.Services;

[Collection("ServerUnitTestRun")]
public class ServerApplicationConsumerServiceTests
{
    private readonly IServerApplicationConsumerService _serverApplicationConsumerService;

    private readonly Mock<ILogger<ServerApplicationConsumerService>> _mockLogger = new();
    private readonly Mock<ICacheManagement> _mockCacheManagement = new();
    private readonly Mock<IConnection> _mockConnection = new();
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public ServerApplicationConsumerServiceTests()
    {
        _serverApplicationConsumerService = new ServerApplicationConsumerService(_mockLogger.Object,
            _mockCacheManagement.Object, _mockConnection.Object);
    }

    [Fact]
    public async Task InitializeConsumerAsync_ShouldSetupQueueAndInitializeConsumer()
    {
        // Arrange  
        var _mockChannel = new Mock<IChannel>(); 

        _mockConnection.Setup(m => m.CreateChannelAsync(default, _cancellationToken)).ReturnsAsync(_mockChannel.Object);

        // Act
        await _serverApplicationConsumerService.InitializeConsumerAsync(_cancellationToken);
        // Assert 
        _mockConnection.Verify(m => m.CreateChannelAsync(default, _cancellationToken), Times.Exactly(2));
    }
}
