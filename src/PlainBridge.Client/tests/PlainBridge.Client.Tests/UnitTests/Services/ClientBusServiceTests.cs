using Microsoft.Extensions.Logging;
using Moq;
using PlainBridge.Client.Application.Services.ClientBus;
using RabbitMQ.Client;
using System.Threading;

namespace PlainBridge.Client.Tests.UnitTests.Services;

[Collection("ClientUnitTestRun")]
public class ClientBusServiceTests
{
    private readonly Mock<ILogger<ClientBusService>> _mockLogger = new();
    private readonly Mock<IConnection> _mockConnection = new();
    private readonly Mock<IChannel> _mockChannel = new();

    private readonly Mock<Application.Management.Cache.ICacheManagement> _mockCache = new();
    private readonly Mock<Application.Services.UsePortSocket.IUsePortSocketService> _mockUsePort = new();
    private readonly Mock<Application.Services.SharePortSocket.ISharePortSocketService> _mockSharePort = new();
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private readonly IClientBusService _clientBusService;

    public ClientBusServiceTests()
    { 
        _clientBusService = new ClientBusService(_mockLogger.Object, _mockConnection.Object, _mockCache.Object, _mockUsePort.Object, _mockSharePort.Object);
    }

    [Theory]
    [InlineData("user1")]
    public async Task InitializeConsumerAsync_ShouldDeclareQueueAndConsume(string username)
    {
        // Arrange  
        var _mockChannel = new Mock<IChannel>();
        _mockConnection.Setup(m => m.CreateChannelAsync(default, _cancellationToken)).ReturnsAsync(_mockChannel.Object);

        // Act
        await _clientBusService.InitializeConsumerAsync(username, _cancellationToken);

        // Assert
        _mockConnection.Verify(m => m.CreateChannelAsync(default, _cancellationToken), Times.Once);

    }
}
