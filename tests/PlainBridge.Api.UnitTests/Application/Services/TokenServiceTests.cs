
using System.Composition;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Moq;
using PlainBridge.Api.Application.Services.Token;
using StackExchange.Redis;

namespace PlainBridge.Api.UnitTests.Application.Services;

[Collection("TestRun")]
public class TokenServiceTests : IClassFixture<TestRunFixture>
{
    private readonly TestRunFixture _fixture;
    private readonly TokenService _tokenService;
    private readonly Mock<IConnectionMultiplexer> _mockIConnectionMultiplexer;
    private readonly Mock<IDatabase> _mockDb;

    public TokenServiceTests(TestRunFixture fixture)
    {
        _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
        _mockIConnectionMultiplexer = new Mock<IConnectionMultiplexer>();
        _mockDb = new Mock<IDatabase>();
        _mockIConnectionMultiplexer.Setup(x => x.GetDatabase(It.IsAny<int>(), null)).Returns(_mockDb.Object);
        _tokenService = new TokenService(_mockIConnectionMultiplexer.Object);
    }

    [Fact]
    public void ParseToken_ShouldReturnJwtSecurityToken()
    {
        // Arrange
        var handler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes("576537ae-b3da-4393-9233-68190b1646e3");
        var tokenDescriptor = new Microsoft.IdentityModel.Tokens.SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim("user_id", "1") }),
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

    [Fact]
    public async Task GenerateToken_ShouldReturnTokenString()
    {
        // Act
        var token = await _tokenService.GenerateToken("sub1", "John", "Doe");

        // Assert
        Assert.False(string.IsNullOrWhiteSpace(token));
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);
        Assert.Contains(jwt.Claims, c => c.Value == "John Doe");
        Assert.Contains(jwt.Claims, c => c.Value == "John");
        Assert.Contains(jwt.Claims, c => c.Value == "Doe"); 
        Assert.Equal("sub1", jwt.Claims.FirstOrDefault(c => c.Type == "user_id")?.Value);
    }

    [Fact]
    public async Task SetTokenPSubAsync_ShouldCallStringSetAsync()
    {
        _mockDb.Setup(x => x.StringSetAsync("tokenpsub:tokenp", "sub", It.IsAny<TimeSpan?>(), It.IsAny<bool>(), It.IsAny<When>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(true);
         

        await _tokenService.SetTokenPSubAsync("tokenp", "sub");

        _mockDb.Verify(x => x.StringSetAsync("tokenpsub:tokenp", "sub", It.IsAny<TimeSpan?>(), It.IsAny<bool>(), It.IsAny<When>(), It.IsAny<CommandFlags>()), Times.Once);
    }

    [Fact]
    public async Task GetTokenPSubAsync_ShouldCallStringGetAsync()
    {
        _mockDb.Setup(x => x.StringGetAsync("tokenpsub:tokenp", It.IsAny<CommandFlags>()))
            .ReturnsAsync("sub");

        var result = await _tokenService.GetTokenPSubAsync("tokenp");

        Assert.Equal("sub", result);
        _mockDb.Verify(x => x.StringGetAsync("tokenpsub:tokenp", It.IsAny<CommandFlags>()), Times.Once);
    }

    [Fact]
    public async Task SetSubTokenAsync_ShouldCallStringSetAsync()
    {
        _mockDb.Setup(x => x.StringSetAsync("subtoken:sub", "token", It.IsAny<TimeSpan?>(), It.IsAny<bool>(), It.IsAny<When>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(true);

        await _tokenService.SetSubTokenAsync("sub", "token");

        _mockDb.Verify(x => x.StringSetAsync("subtoken:sub", "token", It.IsAny<TimeSpan?>(), It.IsAny<bool>(), It.IsAny<When>(), It.IsAny<CommandFlags>()), Times.Once);
    }

    [Fact]
    public async Task GetSunTokenAsync_ShouldCallStringGetAsync()
    {
        _mockDb.Setup(x => x.StringGetAsync("subtoken:sub", It.IsAny<CommandFlags>()))
            .ReturnsAsync("token");

        var result = await _tokenService.GetSunTokenAsync("sub");

        Assert.Equal("token", result);
        _mockDb.Verify(x => x.StringGetAsync("subtoken:sub", It.IsAny<CommandFlags>()), Times.Once);
    }

    [Fact]
    public async Task SetSubTokenPAsync_ShouldCallStringSetAsync()
    {
        _mockDb.Setup(x => x.StringSetAsync("subtokenp:sub", "tokenp", It.IsAny<TimeSpan?>(), It.IsAny<bool>(), It.IsAny<When>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(true);

        await _tokenService.SetSubTokenPAsync("sub", "tokenp");

        _mockDb.Verify(x => x.StringSetAsync("subtokenp:sub", "tokenp", It.IsAny<TimeSpan?>(), It.IsAny<bool>(), It.IsAny<When>(), It.IsAny<CommandFlags>()), Times.Once);
    }

    [Fact]
    public async Task GetSubTokenPAsync_ShouldCallStringGetAsync()
    {
        _mockDb.Setup(x => x.StringGetAsync("subtokenp:sub", It.IsAny<CommandFlags>()))
            .ReturnsAsync("tokenp");

        var result = await _tokenService.GetSubTokenPAsync("sub");

        Assert.Equal("tokenp", result);
        _mockDb.Verify(x => x.StringGetAsync("subtokenp:sub", It.IsAny<CommandFlags>()), Times.Once);
    }

    [Fact]
    public async Task SetTokenPTokenAsync_ShouldCallStringSetAsync()
    {
        _mockDb.Setup(x => x.StringSetAsync("tokenptoken:tokenp", "token", It.IsAny<TimeSpan?>(), It.IsAny<bool>(), It.IsAny<When>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(true);

        await _tokenService.SetTokenPTokenAsync("tokenp", "token");

        _mockDb.Verify(x => x.StringSetAsync("tokenptoken:tokenp", "token", It.IsAny<TimeSpan?>(), It.IsAny<bool>(), It.IsAny<When>(), It.IsAny<CommandFlags>()), Times.Once);
    }

    [Fact]
    public async Task GetTokenPTokenAsync_ShouldCallStringGetAsync()
    {
        _mockDb.Setup(x => x.StringGetAsync("tokenptoken:tokenp", It.IsAny<CommandFlags>()))
            .ReturnsAsync("token");

        var result = await _tokenService.GetTokenPTokenAsync("tokenp");

        Assert.Equal("token", result);
        _mockDb.Verify(x => x.StringGetAsync("tokenptoken:tokenp", It.IsAny<CommandFlags>()), Times.Once);
    }

    [Fact]
    public async Task SetSubIdTokenAsync_ShouldCallStringSetAsync()
    {
        _mockDb.Setup(x => x.StringSetAsync("subidtoken:sub", "idToken", It.IsAny<TimeSpan?>(), It.IsAny<bool>(), It.IsAny<When>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(true);

        await _tokenService.SetSubIdTokenAsync("sub", "idToken");

        _mockDb.Verify(x => x.StringSetAsync("subidtoken:sub", "idToken", It.IsAny<TimeSpan?>(), It.IsAny<bool>(), It.IsAny<When>(), It.IsAny<CommandFlags>()), Times.Once);
    }

    [Fact]
    public async Task GetSubIdTokenAsync_ShouldCallStringGetAsync()
    {
        _mockDb.Setup(x => x.StringGetAsync("subidtoken:sub", It.IsAny<CommandFlags>()))
            .ReturnsAsync("idToken");

        var result = await _tokenService.GetSubIdTokenAsync("sub");

        Assert.Equal("idToken", result);
        _mockDb.Verify(x => x.StringGetAsync("subidtoken:sub", It.IsAny<CommandFlags>()), Times.Once);
    }

    [Fact]
    public async Task SetTokenPRefreshTokenAsync_ShouldCallStringSetAsync()
    {
        _mockDb.Setup(x => x.StringSetAsync("tokenprefreshtoken:tokenp", "refreshToken", It.IsAny<TimeSpan?>(), It.IsAny<bool>(), It.IsAny<When>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(true);

        await _tokenService.SetTokenPRefreshTokenAsync("tokenp", "refreshToken");

        _mockDb.Verify(x => x.StringSetAsync("tokenprefreshtoken:tokenp", "refreshToken", It.IsAny<TimeSpan?>(), It.IsAny<bool>(), It.IsAny<When>(), It.IsAny<CommandFlags>()), Times.Once);
    }

    [Fact]
    public async Task GetTokenPRefreshTokenAsync_ShouldCallStringGetAsync()
    {
        _mockDb.Setup(x => x.StringGetAsync("tokenprefreshtoken:tokenp", It.IsAny<CommandFlags>()))
            .ReturnsAsync("refreshToken");

        var result = await _tokenService.GetTokenPRefreshTokenAsync("tokenp");

        Assert.Equal("refreshToken", result);
        _mockDb.Verify(x => x.StringGetAsync("tokenprefreshtoken:tokenp", It.IsAny<CommandFlags>()), Times.Once);
    }
}
