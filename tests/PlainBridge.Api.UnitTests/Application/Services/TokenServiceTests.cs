using System.Composition;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Caching.Hybrid;
using Moq;
using PlainBridge.Api.Application.Services.Token;
using StackExchange.Redis;

namespace PlainBridge.Api.UnitTests.Application.Services;

[Collection("ApiUnitTestRun")]
public class TokenServiceTests : IClassFixture<TestRunFixture>
{
    private readonly TestRunFixture _fixture;
    private readonly TokenService _tokenService;
    private readonly Mock<HybridCache> _mockDb;

    public TokenServiceTests(TestRunFixture fixture)
    {
        _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
        _mockDb = new Mock<HybridCache>();
        _tokenService = new TokenService(_mockDb.Object);

        // _mockDb.Setup(x => x.GetOrCreateAsync<string, bool>(
        //    It.IsAny<string>(),
        //    It.IsAny<string>(),
        //    It.IsAny<Func<string, CancellationToken, ValueTask<bool>>>(),
        //    It.IsAny<HybridCacheEntryOptions?>(),
        //    It.IsAny<IEnumerable<string>?>(),
        //    It.IsAny<CancellationToken>()
        //)).ReturnsAsync(true);
    }

    [Theory]
    [InlineData("user_id", "1")]
    public void ParseToken_ShouldReturnJwtSecurityToken(string type, string value)
    {
        // Arrange
        var handler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes("576537ae-b3da-4393-9233-68190b1646e3");
        var tokenDescriptor = new Microsoft.IdentityModel.Tokens.SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim(type, value) }),
            Expires = DateTime.UtcNow.AddMinutes(5),
            SigningCredentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(
                new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(key),
                Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256Signature)
        };
        var token = handler.CreateToken(tokenDescriptor);
        var tokenString = handler.WriteToken(token);

        // Act
        var result = _tokenService.ParseToken(tokenString);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<JwtSecurityToken>(result);
        Assert.Contains(result.Claims, c => c.Type == "user_id" && c.Value == "1");
    }

    [Theory]
    [InlineData("sub1", "John", "Doe")]
    public async Task GenerateToken_WhenEverythingIsOk_ShouldReturnTokenString(string sub, string name, string family)
    {
        // Act
        var token = await _tokenService.GenerateToken(sub, name, family);

        // Assert
        Assert.False(string.IsNullOrWhiteSpace(token));
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);
        Assert.Contains(jwt.Claims, c => c.Value == $"{name} {family}");
        Assert.Contains(jwt.Claims, c => c.Value == name);
        Assert.Contains(jwt.Claims, c => c.Value == family);
        Assert.Equal(sub, jwt.Claims.FirstOrDefault(c => c.Type == "user_id")?.Value);
    }

    // Fix for CS0411: Specify type arguments explicitly for GetOrCreateAsync
    [Theory]
    [InlineData("tokenp", "sub")]
    public async Task SetGetTokenPSubAsync_WhenEverythingIsOk_ShouldCallSuccessfully(string tokenp, string value)
    {
        _mockDb.Setup(x => x.GetOrCreateAsync<string, string>(
            $"tokenpsub:{tokenp}",
            value,
            It.IsAny<Func<string, CancellationToken, ValueTask<string>>>(),
            It.IsAny<HybridCacheEntryOptions?>(),
            It.IsAny<IEnumerable<string>?>(),
            CancellationToken.None
        )).ReturnsAsync("true");

        await _tokenService.SetTokenPSubAsync(tokenp, value, CancellationToken.None);

        _mockDb.Verify(x => x.GetOrCreateAsync<string, string>(
            $"tokenpsub:{tokenp}",
            value,
            It.IsAny<Func<string, CancellationToken, ValueTask<string>>>(),
            It.IsAny<HybridCacheEntryOptions?>(),
            It.IsAny<IEnumerable<string>?>(),
            CancellationToken.None
        ), Times.Once);
    }


    [Fact]
    public async Task SetGetSubTokenAsync_WhenEverythingIsOk_ShouldCallSuccessfully()
    {
        //_mockDb.Setup(x => x.GetOrCreateAsync<string, bool>("tokenpsub:tokenp", "sub", It.IsAny<Func<string, CancellationToken, ValueTask<bool>>>(), It.IsAny<HybridCacheEntryOptions?>(), It.IsAny<IEnumerable<string>?>(), It.IsAny<CancellationToken>()))
        //    .ReturnsAsync(true);

        await _tokenService.SetSubTokenAsync("sub", "token");


        _mockDb.Verify(x => x.GetOrCreateAsync<string, bool>(
            "subtoken:sub",
            "token",
            It.IsAny<Func<string, CancellationToken, ValueTask<bool>>>(),
            It.IsAny<HybridCacheEntryOptions?>(),
            It.IsAny<IEnumerable<string>?>(),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Fact]
    public async Task SetGetSubTokenPAsync_WhenEverythingIsOk_ShouldCallSuccessfully()
    {
        //_mockDb.Setup(x => x.GetOrCreateAsync<string, bool>("tokenpsub:tokenp", "sub", It.IsAny<Func<string, CancellationToken, ValueTask<bool>>>(), It.IsAny<HybridCacheEntryOptions?>(), It.IsAny<IEnumerable<string>?>(), It.IsAny<CancellationToken>()))
        //    .ReturnsAsync(true);

        await _tokenService.SetSubTokenPAsync("sub", "tokenp");

        _mockDb.Verify(x => x.GetOrCreateAsync<string, bool>("subtokenp:sub", "sub", It.IsAny<Func<string, CancellationToken, ValueTask<bool>>>(), It.IsAny<HybridCacheEntryOptions?>(), It.IsAny<IEnumerable<string>?>(), It.IsAny<CancellationToken>()), Times.Once);
    }


    [Fact]
    public async Task SetGetTokenPTokenAsync_WhenEverythingIsOk_ShouldCallSuccessfully()
    {
        //_mockDb.Setup(x => x.GetOrCreateAsync<string, bool>("tokenptoken:tokenp", "sub", It.IsAny<Func<string, CancellationToken, ValueTask<bool>>>(), It.IsAny<HybridCacheEntryOptions?>(), It.IsAny<IEnumerable<string>?>(), It.IsAny<CancellationToken>()))
        //    .ReturnsAsync(true);

        await _tokenService.GetTokenPTokenAsync("tokenp");

        _mockDb.Verify(x => x.GetOrCreateAsync<string, bool>("tokenptoken:tokenp", "sub", It.IsAny<Func<string, CancellationToken, ValueTask<bool>>>(), It.IsAny<HybridCacheEntryOptions?>(), It.IsAny<IEnumerable<string>?>(), It.IsAny<CancellationToken>()), Times.Once);
    }


    [Fact]
    public async Task SetGetSubIdTokenAsync_WhenEverythingIsOk_ShouldCallSuccessfully()
    {
        //_mockDb.Setup(x => x.GetOrCreateAsync<string, bool>("subidtoken:sub", "idToken", It.IsAny<Func<string, CancellationToken, ValueTask<bool>>>(), It.IsAny<HybridCacheEntryOptions?>(), It.IsAny<IEnumerable<string>?>(), It.IsAny<CancellationToken>()))
        //    .ReturnsAsync(true);

        await _tokenService.SetSubIdTokenAsync("sub", "idToken");

        _mockDb.Verify(x => x.GetOrCreateAsync<string, bool>("subidtoken:sub", "sub", It.IsAny<Func<string, CancellationToken, ValueTask<bool>>>(), It.IsAny<HybridCacheEntryOptions?>(), It.IsAny<IEnumerable<string>?>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SetGetTokenPRefreshTokenAsync_WhenEverythingIsOk_ShouldCallSuccessfully()
    {
        //_mockDb.Setup(x => x.GetOrCreateAsync<string, bool>("tokenpsub:tokenp", "sub", It.IsAny<Func<string, CancellationToken, ValueTask<bool>>>(), It.IsAny<HybridCacheEntryOptions?>(), It.IsAny<IEnumerable<string>?>(), It.IsAny<CancellationToken>()))
        //    .ReturnsAsync(true);

        await _tokenService.SetTokenPRefreshTokenAsync("tokenp", "refreshToken");

        _mockDb.Verify(x => x.GetOrCreateAsync<string, bool>("tokenprefreshtoken:tokenp", "sub", It.IsAny<Func<string, CancellationToken, ValueTask<bool>>>(), It.IsAny<HybridCacheEntryOptions?>(), It.IsAny<IEnumerable<string>?>(), It.IsAny<CancellationToken>()), Times.Once);
    }

}
