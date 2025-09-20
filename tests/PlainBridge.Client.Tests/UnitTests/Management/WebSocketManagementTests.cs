//using Microsoft.Extensions.Logging;
//using Moq;
//using PlainBridge.Client.Application.Management.WebSocket; 
//using System.Net.WebSockets;

//namespace PlainBridge.Client.Tests.UnitTests.Management;

//[Collection("ClientUnitTestRun")]
//public class WebSocketManagementTests
//{
//    private readonly WebSocketManagement webSocketManagement;
//    private readonly Mock<ClientWebSocket> _clientWebSocket;
//    public WebSocketManagementTests()
//    {
//        _clientWebSocket = new Mock<ClientWebSocket>();

//        webSocketManagement = new WebSocketManagement(new Mock<ILogger<WebSocketManagement>>().Object, _clientWebSocket.Object);
//    }

//    #region ConnectAsync

//    [Fact]
//    public async Task ConnectAsync_WhenEveryThingIsOk_ShouldConnect()
//    {
//        _clientWebSocket.Setup(x => x.ConnectAsync(It.IsAny<Uri>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
//        await webSocketManagement.ConnectAsync(new Uri("wss://example.com/socket"), CancellationToken.None);
//        Assert.True(true);
//    }

//    [Fact]
//    public async Task ConnectAsync_WhenNoServer_ShouldThrowException()
//    {
//        _clientWebSocket.Setup(x => x.ConnectAsync(It.IsAny<Uri>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("No server"));
//        await Assert.ThrowsAnyAsync<Exception>(() => webSocketManagement.ConnectAsync(new Uri("wss://example.com/socket"), CancellationToken.None));
//    }

//    #endregion

//    #region ReceiveAsync

//    [Fact]
//    public async Task ReceiveAsync_WhenEveryThingIsOk_ShouldReturnData()
//    {
//        var webSocketReceiveResult = new WebSocketReceiveResult(1, WebSocketMessageType.Text, true);
//        _clientWebSocket.Setup(x => x.ReceiveAsync(It.IsAny<ArraySegment<byte>>(), It.IsAny<CancellationToken>()))
//                        .ReturnsAsync(webSocketReceiveResult);

//        var res = await webSocketManagement.ReceiveAsync(new ArraySegment<byte>(new byte[1]), CancellationToken.None);

//        Assert.NotNull(res);
//    }

//    [Fact]
//    public async Task ReceiveAsync_WhenNoServer_ShouldThrowException()
//    {
//        _clientWebSocket.Setup(x => x.ReceiveAsync(It.IsAny<ArraySegment<byte>>(), It.IsAny<CancellationToken>()))
//                        .ThrowsAsync(new Exception("No server"));

//        await Assert.ThrowsAnyAsync<Exception>(() => webSocketManagement.ReceiveAsync(new ArraySegment<byte>(new byte[1]), CancellationToken.None));
//    }

//    #endregion

//    #region SendAsync

//    [Fact]
//    public async Task SendAsync_WhenEveryThingIsOk_ShouldReturnData()
//    {  
//        _clientWebSocket.Setup(x => x.SendAsync(It.IsAny<ArraySegment<byte>>(), It.IsAny<WebSocketMessageType>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()));

//        await webSocketManagement.SendAsync(new ArraySegment<byte>(new byte[1]), WebSocketMessageType.Text, true, CancellationToken.None);

//        Assert.True(true);
//    }

//    [Fact]
//    public async Task SendAsync_WhenNoServer_ShouldThrowException()
//    {
//        _clientWebSocket.Setup(x => x.SendAsync(It.IsAny<ArraySegment<byte>>(), It.IsAny<WebSocketMessageType>(), It.IsAny<bool>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("No server"));

//        await Assert.ThrowsAnyAsync<Exception>(() => webSocketManagement.SendAsync(new ArraySegment<byte>(new byte[1]), WebSocketMessageType.Text, true, CancellationToken.None));
//    }

//    #endregion

//}
