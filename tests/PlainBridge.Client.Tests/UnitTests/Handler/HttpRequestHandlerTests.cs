using Moq;
using Microsoft.Extensions.Logging;
using PlainBridge.Client.Application.Handler.HttpRequest;
using PlainBridge.Client.Application.Helpers.Http;
using RabbitMQ.Client;

namespace PlainBridge.Client.Tests.UnitTests.Handler;

[Collection("ClientUnitTestRun")]
public class HttpRequestHandlerTests
{
    private readonly Mock<ILogger<HttpRequestHandler>> _mockLogger = new();
    private readonly Mock<IConnection> _mockConnection = new(); 
    private readonly Mock<IHttpHelper> _mockHttpHelper = new();

    private readonly IHttpRequestHandler _httpRequestHandler;

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public HttpRequestHandlerTests()
    { 
        _httpRequestHandler = new HttpRequestHandler(_mockLogger.Object, _mockConnection.Object, _mockHttpHelper.Object);
    }

    [Theory]
    [InlineData("user1")]   
    public async Task InitializeHttpRequestConsumerAsync_ShouldDeclareAndConsume(string username)
    {
        // Arrange  
        var _mockChannel = new Mock<IChannel>();
        _mockConnection.Setup(m => m.CreateChannelAsync(default, _cancellationToken)).ReturnsAsync(_mockChannel.Object);

        // Act
        await _httpRequestHandler.InitializeHttpRequestConsumerAsync(username, _cancellationToken);

        // Assert
        _mockConnection.Verify(m => m.CreateChannelAsync(default, _cancellationToken), Times.Once);
         
    }
}
