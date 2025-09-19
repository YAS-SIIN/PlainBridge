

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using PlainBridge.Server.Application.DTOs;
using PlainBridge.Server.Application.Management.Cache; 
using PlainBridge.Server.Application.Services.HostApplication;
using PlainBridge.Server.Application.Services.PlainBridgeApiClient;
using PlainBridge.SharedApplication.DTOs;

namespace PlainBridge.Server.Tests.UnitTests.Services;

[Collection("ServerUnitTestRun")]
public class HostApplicationServiceTests
{
    private readonly IHostApplicationService _hostApplicationService;

    private readonly Mock<ILogger<HostApplicationService>> _mockLogger = new();
    private readonly Mock<IPlainBridgeApiClientService> _mockPlainBridgeApiClientService = new();
    private readonly Mock<ICacheManagement> _mockCacheManagement = new();
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public HostApplicationServiceTests()
    {
        var appSettings = Options.Create(new ApplicationSettings
        {
            DefaultDomain = ".example.com",
            PlainBridgeApiAddress = "http://api.local",
            PlainBridgeIdsUrl = "http://ids.local",
            PlainBridgeIdsClientId = "client",
            PlainBridgeIdsClientSecret = "secret",
            PlainBridgeIdsScope = "scope",
            PlainBridgeUseHttp = true,
            HybridDistributedCacheExpirationTime = "24:00:00",
            HybridMemoryCacheExpirationTime = "00:30:00",
            HybridCacheMaximumPayloadBytes = 10485760,
            HybridCacheMaximumKeyLength = 512
        });

        _hostApplicationService = new HostApplicationService(_mockLogger.Object, 
            _mockPlainBridgeApiClientService.Object, 
            _mockCacheManagement.Object, appSettings);
    }

    #region GetByHostAsync

    [Theory]
    [InlineData("test.example.com")]
    public async Task GetByHostAsync_WhenEveryThingIsOk_ShouldReturnHostApplication(string host)
    {
        // Arrange 
        var expectedHostApplication = new HostApplicationDto
        {
            Id = 0,
            Domain = "example.com",
            State = SharedApplication.Enums.RowStateEnum.Active
        };
        _mockCacheManagement.Setup(m => m.SetGetHostApplicationAsync(host, It.IsAny<HostApplicationDto>(), _cancellationToken))
            .ReturnsAsync(expectedHostApplication);
        // Act
        var result = await _hostApplicationService.GetByHostAsync(host, _cancellationToken);
        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedHostApplication.Id, result.Id);
        Assert.Equal(expectedHostApplication.Domain, result.Domain);
        Assert.Equal(expectedHostApplication.State, result.State);
    }

    [Theory]
    [InlineData("")]
    public async Task GetByHostAsync_WhenHostIsEmpty_ShouldThrowArgumentNullException(string host)
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _hostApplicationService.GetByHostAsync(host, _cancellationToken));
    }

    [Theory]
    [InlineData("unknown.example.com")]
    public async Task GetByHostAsync_WhenHostApplicationNotFound_ShouldThrowNotFoundException(string host)
    {
        // Arrange  
        _mockCacheManagement.Setup(m => m.SetGetHostApplicationAsync(host, It.IsAny<HostApplicationDto>(), _cancellationToken))
            .ReturnsAsync((HostApplicationDto?)null);
        // Act & Assert
        await Assert.ThrowsAsync<PlainBridge.SharedApplication.Exceptions.NotFoundException>(() => _hostApplicationService.GetByHostAsync(host, _cancellationToken));
    }

    #endregion

    #region UpdateHostApplicationAsync

    [Fact]
    public async Task UpdateHostApplicationAsync_WhenEveryThingIsOk_ShouldUpdateCache()
    {
        // Arrange 
        var hostApplications = new List<HostApplicationDto>
        {
            new HostApplicationDto
            {
                Id = 0,
                Domain = "example.com",
                State = SharedApplication.Enums.RowStateEnum.Active
            },
            new HostApplicationDto
            {
                Id = 1,
                Domain = "inactive.com",
                State = SharedApplication.Enums.RowStateEnum.DeActive
            }
        };
        _mockPlainBridgeApiClientService.Setup(m => m.GetHostApplicationsAsync(_cancellationToken))
            .ReturnsAsync(hostApplications);
        // Act
        await _hostApplicationService.UpdateHostApplicationAsync(_cancellationToken);
        // Assert
        _mockCacheManagement.Verify(m => m.SetGetHostApplicationAsync(It.IsAny<string>(), It.IsAny<HostApplicationDto>(),
            _cancellationToken),
            Times.Exactly(hostApplications.Where(x => x.State == SharedApplication.Enums.RowStateEnum.Active).Count()));
    }

    [Fact]
    public async Task UpdateHostApplicationAsync_WhenNoActiveHostApplications_ShouldNotUpdateCache()
    {
        // Arrange 
        var hostApplications = new List<HostApplicationDto>
        {
            new HostApplicationDto
            {
                Id = 0,
                Domain = "example.com",
                State = SharedApplication.Enums.RowStateEnum.DeActive
            },
            new HostApplicationDto
            {
                Id = 1,
                Domain = "inactive.com",
                State = SharedApplication.Enums.RowStateEnum.DeActive
            }
        };
        _mockPlainBridgeApiClientService.Setup(m => m.GetHostApplicationsAsync(_cancellationToken))
            .ReturnsAsync(hostApplications);
        // Act
        await _hostApplicationService.UpdateHostApplicationAsync(_cancellationToken);
        // Assert
        _mockCacheManagement.Verify(m => m.SetGetHostApplicationAsync(It.IsAny<string>(), It.IsAny<HostApplicationDto>(),
            _cancellationToken),
            Times.Never);
    }

    #endregion

}
