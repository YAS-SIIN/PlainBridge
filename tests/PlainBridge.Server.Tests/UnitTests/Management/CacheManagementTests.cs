using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using PlainBridge.Server.Application.Management.Cache;
using PlainBridge.Server.Tests.UnitTests.Utils;
using PlainBridge.SharedApplication.DTOs;

namespace PlainBridge.Server.Tests.UnitTests.Management;

[Collection("ServerUnitTestRun")]
public class CacheManagementTests : IClassFixture<ServerUnitTestRunFixture>
{
    private readonly ServerUnitTestRunFixture _fixture;
    private readonly HybridCache _hybrid;
    private readonly ICacheManagement _cacheManagement;

    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    public CacheManagementTests(ServerUnitTestRunFixture fixture)
    {
       
        _fixture = fixture;
        _hybrid = _fixture.Services.BuildServiceProvider().GetRequiredService<HybridCache>();
        _cacheManagement = new CacheManagement(new Mock<Microsoft.Extensions.Logging.ILogger<CacheManagement>>().Object, _hybrid);
    }


    [Theory]
    [InlineData("hostApplication.example.com")]
    public async Task SetGetHostApplicationAsync_WhenEveryThinIsOk_ShouldStoreAndRetrieve(string host)
    { 
        var dto = new HostApplicationDto { Domain = "hostApplication", InternalUrl = "http://int", UserName = "john" };

        var first = await _cacheManagement.SetGetHostApplicationAsync(host, dto, _cancellationToken);
        Assert.NotNull(first); 

        var second = await _cacheManagement.SetGetHostApplicationAsync(host, cancellationToken: _cancellationToken);
        Assert.NotNull(second);
        Assert.Equal("hostApplication", second!.Domain);
    }

    [Fact]
    public async Task SetGetServerApplicationAsync_ByAppId_WhenEveryThinIsOk_ShouldStoreAndRetrieve()
    { 
        var app = new ServerApplicationDto { AppId = Guid.NewGuid(), UserName = "u", InternalPort = 1234 };

        var byId = await _cacheManagement.SetGetServerApplicationAsync(app.AppId.ToString(), app, _cancellationToken);
        Assert.NotNull(byId); 

        var byIdAgain = await _cacheManagement.SetGetServerApplicationAsync(app.AppId.ToString(), cancellationToken: _cancellationToken);
        Assert.Equal(app.AppId, byIdAgain!.AppId);

    }

    [Fact]
    public async Task SetGetServerApplicationAsync_ByUserPort_WhenEveryThinIsOk_ShouldStoreAndRetrieve()
    { 
        var app = new ServerApplicationDto { AppId = Guid.NewGuid(), UserName = "u", InternalPort = 1234 };

        var byUserPort = await _cacheManagement.SetGetServerApplicationAsync(app.UserName!, app.InternalPort, app, _cancellationToken);
        Assert.NotNull(byUserPort);

        var byUserPortAgain = await _cacheManagement.SetGetServerApplicationAsync(app.UserName!, app.InternalPort, cancellationToken: _cancellationToken);
        Assert.Equal(app.InternalPort, byUserPortAgain!.InternalPort);
    }

    //[Fact]
    //public async Task SetGetWebSocketAsync_WhenEveryThinIsOk_ShouldStoreAndRetrieve()
    //{ 
    //    var host = "hostApplication.example.com"; 
    //    var wwwss = new Mock<WebSocket>().Object;
    //    IWebSocketManagement wss = new Application.Management.WebSocket.WebSocketManagement(wwwss);

    //    var stored = await _cacheManagement.SetGetWebSocketAsync(host, wss, _cancellationToken);
    //    Assert.NotNull(stored);

    //    var storedAgain = await _cacheManagement.SetGetWebSocketAsync(host, wss, _cancellationToken);
    //    Assert.NotNull(storedAgain);

    //}

    //[Fact]
    //public async Task SetGetWebSocketAsync_Remove_WhenEveryThinIsOk_ShouldStoreAndRetrieve()
    //{ 
    //    var host = "hostApplication.example.com";
    //    var ws = new Mock<IWebSocketManagement>().Object;

    //    var stored = await _cacheManagement.SetGetWebSocketAsync(host, ws, _cancellationToken);
    //    Assert.NotNull(stored);

    //    await _cacheManagement.RemoveWebSocketAsync(host, _cancellationToken);

    //    var afterRemove = await _cacheManagement.SetGetWebSocketAsync(host, cancellationToken: _cancellationToken);
    //    Assert.Null(afterRemove);
    //}


}
