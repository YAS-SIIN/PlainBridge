using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using PlainBridge.Server.Application.Management.Cache;
using PlainBridge.Server.Tests.Utils;
using PlainBridge.SharedApplication.DTOs;

namespace PlainBridge.Server.Tests.UnitTests.Management;

[Collection("ServerUnitTestRun")]
public class CacheManagementTests : IClassFixture<ServerUnitTestRunFixture>
{
    private readonly ServerUnitTestRunFixture _fixture;
    private readonly HybridCache _hybrid;
    private readonly ICacheManagement _cacheManagement;

    public CacheManagementTests(ServerUnitTestRunFixture fixture)
    {
       
        _fixture = fixture;
        _hybrid = _fixture.Services.BuildServiceProvider().GetRequiredService<HybridCache>();
        _cacheManagement = new CacheManagement(new Mock<Microsoft.Extensions.Logging.ILogger<CacheManagement>>().Object, _hybrid, _fixture.AppSettings);
    }


    [Fact]
    public async Task SetGetHostApplicationAsync_WhenEveryThinIsOk_ShouldStoreAndRetrieve()
    {
        var host = "hostApplication.example.com";
        var dto = new HostApplicationDto { Domain = "hostApplication", InternalUrl = "http://int", UserName = "john" };

        var first = await _cacheManagement.SetGetHostApplicationAsync(host, dto, CancellationToken.None);
        Assert.NotNull(first); 

        var second = await _cacheManagement.SetGetHostApplicationAsync(host, cancellationToken: CancellationToken.None);
        Assert.NotNull(second);
        Assert.Equal("hostApplication", second!.Domain);
    }

    [Fact]
    public async Task SetGetServerApplication_ByAppId_WhenEveryThinIsOk_ShouldStoreAndRetrieve()
    { 
        var app = new ServerApplicationDto { AppId = Guid.NewGuid(), UserName = "u", InternalPort = 1234 };

        var byId = await _cacheManagement.SetGetServerApplicationAsync(app.AppId.ToString(), app, CancellationToken.None);
        Assert.NotNull(byId); 

        var byIdAgain = await _cacheManagement.SetGetServerApplicationAsync(app.AppId.ToString(), cancellationToken: CancellationToken.None);
        Assert.Equal(app.AppId, byIdAgain!.AppId);

    }

    [Fact]
    public async Task SetGetServerApplication_And_ByUserPort_WhenEveryThinIsOk_ShouldStoreAndRetrieve()
    { 
        var app = new ServerApplicationDto { AppId = Guid.NewGuid(), UserName = "u", InternalPort = 1234 };

        var byUserPort = await _cacheManagement.SetGetServerApplicationAsync(app.UserName!, app.InternalPort, app, CancellationToken.None);
        Assert.NotNull(byUserPort);

        var byUserPortAgain = await _cacheManagement.SetGetServerApplicationAsync(app.UserName!, app.InternalPort, cancellationToken: CancellationToken.None);
        Assert.Equal(app.InternalPort, byUserPortAgain!.InternalPort);
    }

    //[Fact]
    //public async Task SetGetWebSocketAsync_WhenEveryThinIsOk_ShouldStoreAndRetrieve()
    //{ 
    //    var host = "hostApplication.example.com"; 
    //    var wwwss = new Mock<WebSocket>().Object;
    //    IWebSocketManagement wss = new Application.Management.WebSocket.WebSocketManagement(wwwss);

    //    var stored = await _cacheManagement.SetGetWebSocketAsync(host, wss, CancellationToken.None);
    //    Assert.NotNull(stored);

    //    var storedAgain = await _cacheManagement.SetGetWebSocketAsync(host, wss, CancellationToken.None);
    //    Assert.NotNull(storedAgain);

    //}

    //[Fact]
    //public async Task SetGetWebSocketAsync_And_Remove_WhenEveryThinIsOk_ShouldStoreAndRetrieve()
    //{ 
    //    var host = "hostApplication.example.com";
    //    var ws = new Mock<IWebSocketManagement>().Object;

    //    var stored = await _cacheManagement.SetGetWebSocketAsync(host, ws, CancellationToken.None);
    //    Assert.NotNull(stored);

    //    await _cacheManagement.RemoveWebSocketAsync(host, CancellationToken.None);

    //    var afterRemove = await _cacheManagement.SetGetWebSocketAsync(host, cancellationToken: CancellationToken.None);
    //    Assert.Null(afterRemove);
    //}


}
