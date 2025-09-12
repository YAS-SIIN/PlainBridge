using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using PlainBridge.Api.Application.Features.User.Queries;
using PlainBridge.SharedApplication.Exceptions; 

namespace PlainBridge.Api.UnitTests.Application.UseCases.User;

[Collection("ApiUnitTestRun")]
public class UserQueriesTests : IClassFixture<TestRunFixture>
{
    private readonly TestRunFixture _fixture;
    private readonly GetAllUsersQueryHandler _getAllHandler;
    private readonly GetUserByExternalIdQueryHandler _getByExternalIdHandler; 

    public UserQueriesTests(TestRunFixture fixture)
    {
        _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));

        _getAllHandler = new GetAllUsersQueryHandler(
            new Mock<ILogger<GetAllUsersQueryHandler>>().Object,
            _fixture.MemoryMainDbContext);

        _getByExternalIdHandler = new GetUserByExternalIdQueryHandler(
            new Mock<ILogger<GetUserByExternalIdQueryHandler>>().Object,
            _fixture.MemoryMainDbContext);
         
    }


    #region GetAllUsersQueryHandler

    [Fact]
    public async Task GetAllUsersQueryHandler_WhenEveryThingIsOk_ShouldReturnData()
    {
        var result = await _getAllHandler.Handle(new GetAllUsersQuery(), CancellationToken.None);
        Assert.NotNull(result);
        Assert.True(result.Count > 0);
    }

    #endregion

    #region GetUserByExternalIdQueryHandler_WhenEveryThingIsOk_ShouldReturnData

    [Theory]
    [InlineData("1")]
    [InlineData("2")]
    public async Task GetUserByExternalIdQueryHandler_WhenEveryThingIsOk_ShouldReturnData(string externalId)
    {
        var user = await _getByExternalIdHandler.Handle(new GetUserByExternalIdQuery { ExternalId = externalId }, CancellationToken.None);
        Assert.NotNull(user);
        Assert.Equal(externalId, user.ExternalId);
    }

    [Theory]
    [InlineData("does-not-exist")]
    public async Task GetUserByExternalIdAsync_WhenIdDoesntExist_ShouldThrowNotFoundException(string externalId)
    {
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _getByExternalIdHandler.Handle(new GetUserByExternalIdQuery { ExternalId = externalId }, CancellationToken.None));
    }

    #endregion
}