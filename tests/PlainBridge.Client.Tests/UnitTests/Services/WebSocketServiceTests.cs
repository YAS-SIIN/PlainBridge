using Microsoft.Extensions.Logging;
using Moq;
using PlainBridge.Client.Application.Management.Cache;
using PlainBridge.Client.Application.Management.WebSocket;
using PlainBridge.Client.Application.Services.WebSocket;
using RabbitMQ.Client;
using System.Net.WebSockets;
using System.Threading;

namespace PlainBridge.Client.Tests.UnitTests.Services;

[Collection("ClientUnitTestRun")]
public class WebSocketServiceTests
{
    private readonly Mock<ILogger<WebSocketService>> _mockLogger = new();
    private readonly Mock<IWebSocketManagement> _mockWebSocketManagement = new();
    private readonly Mock<ICacheManagement> _mockCache = new();
    private readonly Mock<IConnection> _mockConnection = new();
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private readonly IWebSocketService _webSocketService;

    public WebSocketServiceTests()
    {
        _webSocketService = new WebSocketService(_mockLogger.Object, _mockWebSocketManagement.Object, _mockCache.Object, _mockConnection.Object);
    }

    [Theory]
    [InlineData("example.com", "https://internal.example.com")]
    [InlineData("example.com", "http://internal.example.com")]
    public async Task InitializeWebSocketAsync_WhenWebSocketManagementCached_ShouldReturnCached(string host, string internalUrl)
    {
        var internalUri = new Uri(internalUrl);

        _mockCache.Setup(c => c.SetGetWebSocketAsync(host, default!, _cancellationToken))
                  .ReturnsAsync(_mockWebSocketManagement.Object);
         
        var result = await _webSocketService.InitializeWebSocketAsync(host, internalUri, _cancellationToken);
         

        Assert.Same(_mockWebSocketManagement.Object, result);

        _mockWebSocketManagement.Verify(m => m.ConnectAsync(It.IsAny<Uri>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Theory]
    [InlineData("https://internal.example.com", "wss")]
    [InlineData("http://internal.example.com", "ws")]
    public async Task InitializeWebSocketAsynce_WhenWebSocketManagementNotCached_ShouldConnectAndCach(string internalUrl, string expectedScheme)
    {
        var host = "example.com";
        var internalUri = new Uri(internalUrl);

        _mockWebSocketManagement.Setup(m => m.ConnectAsync(It.IsAny<Uri>(), _cancellationToken));

        _mockWebSocketManagement.Setup(m => m.ReceiveAsync(It.IsAny<ArraySegment<byte>>(), _cancellationToken))
            .ReturnsAsync(new WebSocketReceiveResult(1, WebSocketMessageType.Text, true));
         
        _mockCache.Setup(c => c.SetGetWebSocketAsync(host, null!, _cancellationToken))
                  .ReturnsAsync((IWebSocketManagement)null!);

        var _mockChannel = new Mock<IChannel>();
        _mockConnection.Setup(c => c.CreateChannelAsync(default, _cancellationToken))
            .ReturnsAsync(_mockChannel.Object);

        var result = await _webSocketService.InitializeWebSocketAsync(host, internalUri, _cancellationToken);

        Assert.NotNull(result);
        _mockWebSocketManagement.Verify(m => m.ConnectAsync(It.Is<Uri>(u => u.Scheme == expectedScheme), _cancellationToken), Times.Once);
        _mockWebSocketManagement.Verify(m => m.ReceiveAsync(It.IsAny<ArraySegment<byte>>(), _cancellationToken), Times.AtLeast(1));
        _mockCache.Verify(c => c.SetGetWebSocketAsync(host, _mockWebSocketManagement.Object, _cancellationToken), Times.Once);
        _mockConnection.Verify(m => m.CreateChannelAsync(default, _cancellationToken), Times.Once);
    }
}
