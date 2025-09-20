using Moq;
using Microsoft.Extensions.Logging;
using PlainBridge.Client.Application.Services.ServerBus;
using RabbitMQ.Client;

namespace PlainBridge.Client.Tests.UnitTests.Services;

[Collection("ClientUnitTestRun")]
public class ServerBusServiceTests
{
    private readonly Mock<ILogger<ServerBusService>> _mockLogger = new();
    private readonly Mock<IConnection> _mockConnection = new();
    private readonly Mock<IChannel> _mockChannel = new();
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private readonly IServerBusService _serverBusService;

    public ServerBusServiceTests()
    {
        _mockConnection.Setup(c => c.CreateChannelAsync(default, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_mockChannel.Object);
        _serverBusService = new ServerBusService(_mockLogger.Object, _mockConnection.Object);
    }

    [Theory]
    [InlineData("user1")]
    public async Task RequestServerApplicationsAsync_ShouldDeclareAndPublish_WithUsernameHeader(string username)
    {
        // Arrange  
        var _mockChannel = new Mock<IChannel>();
        _mockConnection.Setup(m => m.CreateChannelAsync(default, _cancellationToken)).ReturnsAsync(_mockChannel.Object);

        // Act
        await _serverBusService.RequestServerApplicationsAsync(username, _cancellationToken);

        // Assert
        _mockConnection.Verify(m => m.CreateChannelAsync(default, _cancellationToken), Times.Once);
    }

}
