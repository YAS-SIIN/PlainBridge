

using Microsoft.Extensions.Logging;
using Moq;
using PlainBridge.Server.Application.DTOs;
using PlainBridge.Server.Application.Management.Cache;
using PlainBridge.Server.Application.Services.HostApplication;
using PlainBridge.Server.Application.Services.PlainBridgeApiClient;
using PlainBridge.Server.Application.Services.ServerApplication;
using PlainBridge.SharedApplication.DTOs; 

namespace PlainBridge.Server.Tests.UnitTests.Services;

[Collection("ServerUnitTestRun")]
public class ServerApplicationServiceTests
{
    private readonly IServerApplicationService _serverApplicationService;

    private readonly Mock<ILogger<ServerApplicationService>> _mockLogger = new();
    private readonly Mock<IPlainBridgeApiClientService> _mockPlainBridgeApiClientService = new();
    private readonly Mock<ICacheManagement> _mockCacheManagement = new();
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public ServerApplicationServiceTests()
    { 
        _serverApplicationService = new ServerApplicationService(_mockLogger.Object,
            _mockPlainBridgeApiClientService.Object,
            _mockCacheManagement.Object);
    }

    #region UpdateServerApplicationAsync
    [Fact]
    public async Task UpdateServerApplicationAsync_WhenEverythingIsOk_ShouldUpdateCache()
    {
        // Arrange 
        var serverApplications = new List<ServerApplicationDto>
        {
            new ServerApplicationDto
            {
                Id = 1,
                Name = "App1",
                State = SharedApplication.Enums.RowStateEnum.Active
            },
            new ServerApplicationDto
            {
                Id = 2,
                Name = "App2",
                State = SharedApplication.Enums.RowStateEnum.DeActive
            }
        };
        _mockPlainBridgeApiClientService.Setup(m => m.GetServerApplicationsAsync(_cancellationToken))
            .ReturnsAsync(serverApplications);

        // Act
        await _serverApplicationService.UpdateServerApplicationAsync(_cancellationToken);

        // Assert
        _mockCacheManagement.Verify(m => m.SetGetServerApplicationAsync(It.IsAny<string>(), 
            It.IsAny<int>(), It.IsAny<ServerApplicationDto>(), _cancellationToken),
            Times.Exactly(serverApplications.Where(x => x.State == SharedApplication.Enums.RowStateEnum.Active).Count()));
        _mockCacheManagement.Verify(m => m.SetGetServerApplicationAsync(It.IsAny<string>(), 
            It.IsAny<ServerApplicationDto>(), _cancellationToken), 
            Times.Exactly(serverApplications.Where(x => x.State == SharedApplication.Enums.RowStateEnum.Active).Count()));

    }

    [Fact]
    public async Task UpdateServerApplicationAsync_WhenNoActiveServerApplications_ShouldNotUpdateCache()
    {
        // Arrange 
        var serverApplications = new List<ServerApplicationDto>
        {
            new ServerApplicationDto
            {
                Id = 1,
                Name = "App1",
                State = SharedApplication.Enums.RowStateEnum.DeActive
            },
            new ServerApplicationDto
            {
                Id = 2,
                Name = "App2",
                State = SharedApplication.Enums.RowStateEnum.DeActive
            }
        };

        _mockPlainBridgeApiClientService.Setup(m => m.GetServerApplicationsAsync(_cancellationToken))
            .ReturnsAsync(serverApplications);

        // Act
        await _serverApplicationService.UpdateServerApplicationAsync(_cancellationToken);

        // Assert
        _mockCacheManagement.Verify(m => m.SetGetServerApplicationAsync(It.IsAny<string>(), 
            It.IsAny<int>(), It.IsAny<ServerApplicationDto>(), _cancellationToken),
            Times.Never);
        _mockCacheManagement.Verify(m => m.SetGetServerApplicationAsync(It.IsAny<string>(), 
            It.IsAny<ServerApplicationDto>(), _cancellationToken),
            Times.Never);

    }


    #endregion
}
