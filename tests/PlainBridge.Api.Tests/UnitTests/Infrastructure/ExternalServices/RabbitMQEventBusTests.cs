
using Microsoft.Extensions.Logging;
using Moq;
using PlainBridge.Api.Infrastructure.ExternalServices.Messaging;
using RabbitMQ.Client;

namespace PlainBridge.Api.Tests.UnitTests.Infrastructure.ExternalServices;

[Collection("ApiUnitTestRun")]
public class RabbitMQEventBusTests
{
    private readonly IEventBus eventBus;

    private readonly Mock<ILogger<RabbitMQEventBus>> _mockLogger = new();
    private readonly Mock<IConnection> _mockConnection = new();
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    public RabbitMQEventBusTests()
    {
        _mockLogger = new Mock<ILogger<RabbitMQEventBus>>();
        eventBus = new RabbitMQEventBus(_mockLogger.Object, _mockConnection.Object);
    }

    [Theory]
    [InlineData("test")]
    public async Task PublishAsync_ShouldPublish(string message)
    {
        // Arrange  
        var _mockChannel = new Mock<IChannel>();
        _mockConnection.Setup(m => m.CreateChannelAsync(default, _cancellationToken)).ReturnsAsync(_mockChannel.Object);

        // Act
        await eventBus.PublishAsync(message, _cancellationToken);

        // Assert
        _mockConnection.Verify(m => m.CreateChannelAsync(default, _cancellationToken), Times.Once);
    }
}
