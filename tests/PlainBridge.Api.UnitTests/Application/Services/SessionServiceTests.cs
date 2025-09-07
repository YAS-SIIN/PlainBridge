

using System.Security.Claims;
using IdentityModel.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using PlainBridge.Api.Application.Services.Session;
using PlainBridge.Api.Application.Services.Token;
using PlainBridge.Api.Application.Services.User;
using PlainBridge.Api.Infrastructure.DTOs;
using PlainBridge.SharedApplication.DTOs;

namespace PlainBridge.Api.UnitTests.Application.Services;

[Collection("ApiUnitTestRun")]
public class SessionServiceTests : IClassFixture<TestRunFixture>
{
    private readonly TestRunFixture _fixture;
    private readonly ISessionService _sessionService;
    private readonly Mock<ILogger<SessionService>> _mockLoggerSessionService;
    private readonly Mock<IHttpContextAccessor> _mockIHttpContextAccessor;
    private readonly Mock<IUserService> _mockIUserService;
    private readonly Mock<ITokenService> _mockITokenService;
    private readonly Mock<IHttpClientFactory> _mockIHttpClientFactory;
    private readonly Mock<IOptions<ApplicationSettings>> _mockApplicationSetting;

    public SessionServiceTests(TestRunFixture fixture)
    {
        _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
        _mockLoggerSessionService = new Mock<ILogger<SessionService>>();
        _mockIHttpContextAccessor = new Mock<IHttpContextAccessor>();
        _mockIUserService = new Mock<IUserService>();
        _mockITokenService = new Mock<ITokenService>();
        _mockIHttpClientFactory = new Mock<IHttpClientFactory>();
        _mockApplicationSetting = new Mock<IOptions<ApplicationSettings>>();
        _sessionService = new SessionService(
            _mockLoggerSessionService.Object,
            _mockIHttpContextAccessor.Object,
            _mockIUserService.Object,
            _mockITokenService.Object,
            _mockIHttpClientFactory.Object,
            _mockApplicationSetting.Object
        );
    }

    [Fact]
    public async Task GetCurrentUserAsync_WithValidSubClaim_ReturnsUserDto()
    {
        // Arrange
        var userId = "123";
        var claims = new List<Claim> { new Claim("sub", userId) };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);
        var context = new DefaultHttpContext { User = principal };
        _mockIHttpContextAccessor.Setup(x => x.HttpContext).Returns(context);

        var expectedUser = new UserDto { ExternalId = userId, Username = "TestUser" };
        _mockIUserService.Setup(x => x.GetUserByExternalIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedUser);

        // Act
        var result = await _sessionService.GetCurrentUserAsync(CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userId, result.ExternalId);
        Assert.Equal("TestUser", result.Username);
    }

    [Fact]
    public async Task GetCurrentUserAsync_WithoutSubClaim_ShouldReturnsNull()
    {
        // Arrange
        var claims = new List<Claim>();
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);
        var context = new DefaultHttpContext { User = principal };
        _mockIHttpContextAccessor.Setup(x => x.HttpContext).Returns(context);

        // Act
        var result = await _sessionService.GetCurrentUserAsync(CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetCurrentUserProfileAsync_WithValidSubClaim_ShouldReturnData()
    {
        // Arrange  
        var userId = "123";
        var claims = new List<Claim> { new Claim("sub", userId) };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);
        var context = new DefaultHttpContext { User = principal };
        _mockIHttpContextAccessor.Setup(x => x.HttpContext).Returns(context);

        var token = "token123";
        _mockITokenService.Setup(x => x.SetGetSubTokenAsync(userId, It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(token);

        // Replace this line:
        // var appSetting = new ApplicationSettings { PlainBridgeIdsUrl = "https://ids.example.com" };

        // With the following, setting all required properties:
        var appSetting = new ApplicationSettings
        {
            PlainBridgeIdsClientId = "test-client-id",
            PlainBridgeIdsClientSecret = "test-client-secret",
            PlainBridgeIdsScope = "test-scope",
            PlainBridgeUseHttp = false,
            PlainBridgeIdsUrl = "https://ids.example.com",
            PlainBridgeWebUrl = "https://web.example.com",
            PlainBridgeWebRedirectPage = "/redirect",
            HybridDistributedCacheExpirationTime = "00:10:00",
            HybridMemoryCacheExpirationTime = "00:05:00",
            HybridCacheMaximumPayloadBytes = 1024,
            HybridCacheMaximumKeyLength = 128
        };
        _mockApplicationSetting.Setup(x => x.Value).Returns(appSetting);

        var userInfo = new UserProfileViewDto
        {
            Username = "TestUser",
            Email = "test@example.com",
            Name = "Test",
            Family = "User"
        };
        var userInfoJson = System.Text.Json.JsonSerializer.Serialize(userInfo);

        var httpMessageHandler = new MockHttpMessageHandler(userInfoJson);
        var mockHttpClient = new HttpClient(httpMessageHandler);

        _mockIHttpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>()))
            .Returns(mockHttpClient);

        // Act  
        var result = await _sessionService.GetCurrentUserProfileAsync(CancellationToken.None);

        // Assert  
        Assert.NotNull(result);
        Assert.Equal(userInfo.Username, result.Username);
        Assert.Equal(userInfo.Email, result.Email);
        Assert.Equal(userInfo.Name, result.Name);
        Assert.Equal(userInfo.Family, result.Family);
    }

    [Fact]
    public async Task GetCurrentUserProfileAsync_WithoutSubClaim_ShouldReturnsNull()
    {
        // Arrange
        var claims = new List<Claim>();
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);
        var context = new DefaultHttpContext { User = principal };
        _mockIHttpContextAccessor.Setup(x => x.HttpContext).Returns(context);

        // Act
        var result = await _sessionService.GetCurrentUserProfileAsync(CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    // Helper for mocking HttpClient if needed
    private class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly string _responseContent;
        public MockHttpMessageHandler(string responseContent)
        {
            _responseContent = responseContent;
        }
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent(_responseContent)
            };
            return Task.FromResult(response);
        }
    }
}
