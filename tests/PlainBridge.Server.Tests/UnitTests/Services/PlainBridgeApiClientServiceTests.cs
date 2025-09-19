
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using PlainBridge.Server.Application.DTOs;
using PlainBridge.Server.Application.Management.Cache;
using PlainBridge.Server.Application.Services.HostApplication;
using PlainBridge.Server.Application.Services.Identity;
using PlainBridge.Server.Application.Services.PlainBridgeApiClient;
using PlainBridge.SharedApplication.DTOs;
using PlainBridge.SharedApplication.Enums;
using System.Text.Json;

namespace PlainBridge.Server.Tests.UnitTests.Services;

[Collection("ServerUnitTestRun")]
public class PlainBridgeApiClientServiceTests
{
    private readonly IPlainBridgeApiClientService _plainBridgeApiClientService;

    private readonly Mock<ILogger<PlainBridgeApiClientService>> _mockLogger = new();
    private readonly Mock<IHttpClientFactory> _mockIHttpClientFactory = new();
    private readonly Mock<IIdentityService> _mockIdentityService = new();
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    public PlainBridgeApiClientServiceTests()
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

        _plainBridgeApiClientService = new PlainBridgeApiClientService(_mockLogger.Object,
            appSettings,
            _mockIHttpClientFactory.Object,
            _mockIdentityService.Object);
    }
    #region GetHostApplicationsAsync

    [Fact]
    public async Task GetHostApplicationsAsync_WhenTokenIsNull_ShouldThrowApplicationException()
    {
        // Arrange
        _mockIdentityService.Setup(m => m.GetTokenAsync(_cancellationToken))
            .ReturnsAsync((IdentityModel.Client.TokenResponse?)null);
        // Act & Assert
        await Assert.ThrowsAsync<ApplicationException>(() => _plainBridgeApiClientService.GetHostApplicationsAsync(_cancellationToken));
    }

    [Fact]
    public async Task GetHostApplicationsAsync_WhenEverythingIsOk_ShouldReturnHostApplications()
    {
        // Arrange
        var expectedHostApplications = ResultDto<List<HostApplicationDto>>.ReturnData(new List<HostApplicationDto>
        {
            new HostApplicationDto
            {
                Id = 1,
                Domain = "example.com",
                State = RowStateEnum.Active
            }
        }, ResultCodeEnum.Success, "");
        var mockTokenResponse = new IdentityModel.Client.TokenResponse();
        _mockIdentityService.Setup(m => m.GetTokenAsync(_cancellationToken))
            .ReturnsAsync(mockTokenResponse);
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();

        mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(expectedHostApplications, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }))
            });

        var httpClient = new HttpClient(mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("http://api.local")
        };

        _mockIHttpClientFactory.Setup(m => m.CreateClient(It.IsAny<string>())).Returns(httpClient);
        // Act
        var result = await _plainBridgeApiClientService.GetHostApplicationsAsync(_cancellationToken);
        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(expectedHostApplications.Data![0].Id, result[0].Id);
        Assert.Equal(expectedHostApplications.Data![0].Domain, result[0].Domain);
        Assert.Equal(expectedHostApplications.Data![0].State, result[0].State);
    }

    #endregion


    #region GetServerApplicationsAsync

    [Fact]
    public async Task GetServerApplicationsAsync_WhenTokenIsNull_ShouldThrowApplicationException()
    {
        // Arrange
        _mockIdentityService.Setup(m => m.GetTokenAsync(_cancellationToken))
            .ReturnsAsync((IdentityModel.Client.TokenResponse?)null);
        // Act & Assert
        await Assert.ThrowsAsync<ApplicationException>(() => _plainBridgeApiClientService.GetServerApplicationsAsync(_cancellationToken));
    }

    [Fact]
    public async Task GetServerApplicationsAsync_WhenEverythingIsOk_ShouldReturnServerApplications()
    {
        // Arrange
        var expectedServerApplications = ResultDto<List<ServerApplicationDto>>.ReturnData(new List<ServerApplicationDto>
        {
            new ServerApplicationDto
            {
                Id = 1,
                Name = "TestApp",
                State = RowStateEnum.Active
            }
        }, ResultCodeEnum.Success, "");
        var mockTokenResponse = new IdentityModel.Client.TokenResponse();
        _mockIdentityService.Setup(m => m.GetTokenAsync(_cancellationToken))
            .ReturnsAsync(mockTokenResponse);
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(expectedServerApplications, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }))
            });
        var httpClient = new HttpClient(mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("http://api.local")
        };
        _mockIHttpClientFactory.Setup(m => m.CreateClient(It.IsAny<string>())).Returns(httpClient);
        // Act
        var result = await _plainBridgeApiClientService.GetServerApplicationsAsync(_cancellationToken);
        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(expectedServerApplications.Data![0].Id, result[0].Id);
        Assert.Equal(expectedServerApplications.Data![0].Name, result[0].Name);
        Assert.Equal(expectedServerApplications.Data![0].State, result[0].State);
    }

    #endregion

}
