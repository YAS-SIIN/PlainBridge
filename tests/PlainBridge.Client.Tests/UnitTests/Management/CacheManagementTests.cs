 
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection;
using Moq; 
using PlainBridge.Client.Application.Management.Cache; 
using PlainBridge.Client.Tests.UnitTests.Utils;
using PlainBridge.SharedApplication.DTOs; 

namespace PlainBridge.Client.Tests.UnitTests.Management;


[Collection("ClientUnitTestRun")]
public class CacheManagementTests : IClassFixture<ClientUnitTestRunFixture>
{
    private readonly ClientUnitTestRunFixture _fixture;
    private readonly ICacheManagement _cacheManagement;

    private readonly HybridCache _hybrid;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public CacheManagementTests(ClientUnitTestRunFixture fixture)
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


    [Fact]
    public async Task SetGetServerApplicationAsync_ByListServerApplication_WhenEveryThinIsOk_ShouldStoreAndRetrieve()
    {
        var apps = new List<ServerApplicationDto> { new ServerApplicationDto 
            { AppId = Guid.NewGuid(), UserName = "u", InternalPort = 1234 }
        };

        var byUserPort = await _cacheManagement.SetGetServerApplicationsAsync(apps, _cancellationToken);
        Assert.NotNull(byUserPort);

        var byUserPortAgain = await _cacheManagement.SetGetServerApplicationsAsync(apps, _cancellationToken);
        Assert.Equal(apps.First().InternalPort, byUserPortAgain!.First().InternalPort);
    }

    //[Theory]
    //[InlineData(1234)]
    //public async Task SetGetTcpListenerAsync_WhenEveryThinIsOk_ShouldStoreAndRetrieve(int port)
    //{
    //    var ipAddress = System.Net.IPAddress.Loopback;
    //    var tcpListener = new TcpListener(ipAddress, port); 
    //    var storedTcpListener = await _cacheManagement.SetGetTcpListenerAsync(port, tcpListener, _cancellationToken);
    //    Assert.NotNull(storedTcpListener);

    //    var storedTcpListenerAgain = await _cacheManagement.SetGetTcpListenerAsync(port, cancellationToken: _cancellationToken);
    //    Assert.NotNull(storedTcpListenerAgain);
    //    Assert.Equal(tcpListener.LocalEndpoint, storedTcpListenerAgain!.LocalEndpoint);
    //}


    //[Theory]
    //[InlineData(1234, "test")]
    //public async Task SetGetUsePortModelAsync_WhenEveryThinIsOk_ShouldStoreAndRetrieve(int port, string connectionId)
    //{
    //    var tcpClient = new TcpClient();
    //    var userPort = new UsePortCacheDto(tcpClient, Task.CompletedTask);

    //    var storedUsePort = await _cacheManagement.SetGetUsePortModelAsync(port, connectionId, userPort, _cancellationToken);
    //    Assert.NotNull(storedUsePort);

    //    var storedUsePortAgain = await _cacheManagement.SetGetUsePortModelAsync(port, connectionId, cancellationToken: _cancellationToken);
    //    Assert.NotNull(storedUsePortAgain); 
    //    Assert.Equal(userPort.TcpClient, storedUsePortAgain!.TcpClient);
    //}




    //[Theory]
    //[InlineData("testUser", 1234, "testId")]
    //public async Task SetSharePortModelAsync_WhenEveryThinIsOk_ShouldStoreAndRetrieve(string useportUsername, int useportPort, string connectionId)
    //{
    //    var tcpClient = new TcpClient();
    //    var userPort = new SharePortCacheDto(tcpClient, Task.CompletedTask);

    //    var storedUsePort = await _cacheManagement.SetSharePortModelAsync(useportUsername, useportPort, connectionId, userPort, _cancellationToken);
    //    Assert.NotNull(storedUsePort);

    //    var storedUsePortAgain = await _cacheManagement.SetSharePortModelAsync(useportUsername, useportPort, connectionId, cancellationToken: _cancellationToken);
    //    Assert.NotNull(storedUsePortAgain);
    //    Assert.Equal(userPort.TcpClient, storedUsePortAgain!.TcpClient);
    //}



      
      
}
