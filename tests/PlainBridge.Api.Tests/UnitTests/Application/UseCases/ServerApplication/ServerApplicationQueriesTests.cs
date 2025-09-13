

using Microsoft.Extensions.Logging;
using Moq;
using PlainBridge.Api.Application.UseCases.HostApplication.Queries;
using PlainBridge.Api.Application.UseCases.ServerApplication.Queries;
using PlainBridge.SharedApplication.Exceptions;

namespace PlainBridge.Api.Tests.UnitTests.Application.UseCases.ServerApplication;

[Collection("ApiUnitTestRun")]
public class ServerApplicationQueriesTests : IClassFixture<TestRunFixture>
{
    private readonly TestRunFixture _fixture;
    private readonly GetAllServerApplicationsQueryHandler _getAllServerApplicationsQueryHandler;
    private readonly GetServerApplicationQueryHandler _getServerApplicationQueryHandler; 


    public ServerApplicationQueriesTests(TestRunFixture fixture)
    {
        _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));

        _getAllServerApplicationsQueryHandler = new GetAllServerApplicationsQueryHandler(new Mock<ILogger<GetAllServerApplicationsQueryHandler>>().Object, _fixture.MemoryMainDbContext);
        _getServerApplicationQueryHandler = new GetServerApplicationQueryHandler(new Mock<ILogger<GetServerApplicationQueryHandler>>().Object, _fixture.MemoryMainDbContext);
    }

    #region GetAllServerApplicationsQueryHandler

    [Fact]
    public async Task GetAllServerApplicationsQueryHandler_WhenEveryThingIsOk_ShouldReturnData()
    {
        var result = await _getAllServerApplicationsQueryHandler.Handle(new GetAllServerApplicationsQuery(), CancellationToken.None);
        Assert.NotNull(result);
        Assert.True(result.Count > 0);
    }

    #endregion

    #region GetServerApplicationQueryHandler
    [Theory]
    [InlineData(1, 1)]
    public async Task GetServerApplicationQueryHandler_WhenEveryThingIsOk_ShouldReturnData(long id, long userId)
    {
        GetServerApplicationQuery getServerApplicationQuery = new GetServerApplicationQuery
        {
            Id = id,
            UserId = userId
        };
        var result = await _getServerApplicationQueryHandler.Handle(getServerApplicationQuery, CancellationToken.None);
        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
    }

    [Theory]
    [InlineData(999, 1)]
    public async Task GetServerApplicationQueryHandler_WhenIdDoesntExist_ShouldThrowException(long id, long userId)
    {
        GetServerApplicationQuery getServerApplicationQuery = new GetServerApplicationQuery
        {
            Id = id,
            UserId = userId
        };
        await Assert.ThrowsAsync<NotFoundException>(async () => await _getServerApplicationQueryHandler.Handle(getServerApplicationQuery, CancellationToken.None));
    }
    #endregion

}
