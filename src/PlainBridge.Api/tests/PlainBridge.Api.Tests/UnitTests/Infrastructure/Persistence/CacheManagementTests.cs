using Microsoft.Extensions.Caching.Hybrid;
using Moq;
using PlainBridge.Api.Infrastructure.Persistence.Cache;
using PlainBridge.Api.Tests.UnitTests.Infrastructure.Utils;
using PlainBridge.SharedApplication.DTOs;

namespace PlainBridge.Api.Tests.UnitTests.Infrastructure.Persistence;

[Collection("ApiUnitTestRun")]
public class CacheManagementTests : IClassFixture<ApiInfrastructureUnitTestRunFixture>
{
    private readonly ApiInfrastructureUnitTestRunFixture _fixture;
    private readonly HybridCache _hybrid;
    private readonly ICacheManagement _cacheManagement;

    public CacheManagementTests(ApiInfrastructureUnitTestRunFixture fixture)
    {
       
        _fixture = fixture;
        _hybrid = _fixture.Services.BuildServiceProvider().GetRequiredService<HybridCache>();
        _cacheManagement = new CacheManagement(new Mock<Microsoft.Extensions.Logging.ILogger<CacheManagement>>().Object, _hybrid);
    }
     
    [Theory]
    [InlineData("tokenp", "token")]
    public async Task SetGetTokenPSubAsync_WhenEveryThinIsOk_ShouldStoreAndRetrieve(string tokenp, string token)
    {
        var firstToken = await _cacheManagement.SetGetTokenPSubAsync(tokenp, token, CancellationToken.None);
        Assert.NotNull(firstToken);

        var secondToken = await _cacheManagement.SetGetTokenPSubAsync(tokenp, cancellationToken: CancellationToken.None);
        Assert.NotNull(secondToken);
        Assert.Equal(token, secondToken!);
    }
      
    [Theory]
    [InlineData("sub", "token")]
    public async Task SetGetSubTokenAsync_WhenEveryThinIsOk_ShouldStoreAndRetrieve(string sub, string token)
    {
        var firstToken = await _cacheManagement.SetGetSubTokenAsync(sub, token, CancellationToken.None);
        Assert.NotNull(firstToken);

        var secondToken = await _cacheManagement.SetGetSubTokenAsync(sub, cancellationToken: CancellationToken.None);
        Assert.NotNull(secondToken);
        Assert.Equal(token, secondToken!);
    }
     
    [Theory]
    [InlineData("sub", "token")]
    public async Task SetGetSubTokenPAsync_WhenEveryThinIsOk_ShouldStoreAndRetrieve(string sub, string token)
    {
        var firstToken = await _cacheManagement.SetGetSubTokenPAsync(sub, token, CancellationToken.None);
        Assert.NotNull(firstToken);

        var secondToken = await _cacheManagement.SetGetSubTokenPAsync(sub, cancellationToken: CancellationToken.None);
        Assert.NotNull(secondToken);
        Assert.Equal(token, secondToken!);
    }
     
    [Theory]
    [InlineData("tokenp", "token")]
    public async Task SetGetTokenPTokenAsync_WhenEveryThinIsOk_ShouldStoreAndRetrieve(string tokenp, string token)
    {
        var firstToken = await _cacheManagement.SetGetTokenPTokenAsync(tokenp, token, CancellationToken.None);
        Assert.NotNull(firstToken);

        var secondToken = await _cacheManagement.SetGetTokenPTokenAsync(tokenp, cancellationToken: CancellationToken.None);
        Assert.NotNull(secondToken);
        Assert.Equal(token, secondToken!);
    }

    [Theory]
    [InlineData("sub", "token")]
    public async Task SetGetSubIdTokenAsync_WhenEveryThinIsOk_ShouldStoreAndRetrieve(string sub, string token)
    {
        var firstToken = await _cacheManagement.SetGetSubIdTokenAsync(sub, token, CancellationToken.None);
        Assert.NotNull(firstToken);

        var secondToken = await _cacheManagement.SetGetSubIdTokenAsync(sub, cancellationToken: CancellationToken.None);
        Assert.NotNull(secondToken);
        Assert.Equal(token, secondToken!);
    }

    [Theory]
    [InlineData("tokenp", "token")]
    public async Task SetGetTokenPRefreshTokenAsync_WhenEveryThinIsOk_ShouldStoreAndRetrieve(string tokenp, string token)
    {
        var firstToken = await _cacheManagement.SetGetTokenPRefreshTokenAsync(tokenp, token, CancellationToken.None);
        Assert.NotNull(firstToken);

        var secondToken = await _cacheManagement.SetGetTokenPRefreshTokenAsync(tokenp, cancellationToken: CancellationToken.None);
        Assert.NotNull(secondToken);
        Assert.Equal(token, secondToken!);
    }


     
     
     

}
