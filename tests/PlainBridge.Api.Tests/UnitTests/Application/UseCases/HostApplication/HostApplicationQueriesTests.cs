

using Microsoft.Extensions.Logging;
using Moq;
using PlainBridge.Api.Application.UseCases.HostApplication.Queries;
using PlainBridge.SharedApplication.Exceptions;

namespace PlainBridge.Api.Tests.UnitTests.Application.UseCases.HostApplication;

[Collection("ApiUnitTestRun")]
public class HostApplicationQueriesTests : IClassFixture<TestRunFixture>
{
    private readonly TestRunFixture _fixture;
    private readonly GetAllHostApplicationsQueryHandler _getAllHostApplicationsQueryHandler;
    private readonly GetHostApplicationQueryHandler _getHostApplicationQueryHandler; 


    public HostApplicationQueriesTests(TestRunFixture fixture)
    {
        _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture)); 

        _getAllHostApplicationsQueryHandler = new GetAllHostApplicationsQueryHandler(new Mock<ILogger<GetAllHostApplicationsQueryHandler>>().Object, _fixture.MemoryMainDbContext);
        _getHostApplicationQueryHandler = new GetHostApplicationQueryHandler(new Mock<ILogger<GetHostApplicationQueryHandler>>().Object, _fixture.MemoryMainDbContext);
    }

    #region GetAllHostApplicationsQueryHandler

    [Fact]
    public async Task GetAllHostApplicationsQueryHandler_WhenEveryThingIsOk_ShouldReturnData()
    {
        var result = await _getAllHostApplicationsQueryHandler.Handle(new GetAllHostApplicationsQuery(), CancellationToken.None);
        Assert.NotNull(result);
        Assert.True(result.Count > 0);
    }

    #endregion

    #region GetHostApplicationQueryHandler 
    [Theory]
    [InlineData(1, 1)]
    public async Task GetHostApplicationQueryHandler_WhenEveryThingIsOk_ShouldReturnData(long id, long userId)
    {
        GetHostApplicationQuery getHostApplicationQuery = new GetHostApplicationQuery
        {
            Id = id,
            UserId = userId
        };
        var result = await _getHostApplicationQueryHandler.Handle(getHostApplicationQuery, CancellationToken.None);
        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
    }

    [Theory]
    [InlineData(999, 1)]
    public async Task GetHostApplicationQueryHandler_WhenIdDoesntExist_ShouldThrowException(long id, long userId)
    {
        GetHostApplicationQuery getHostApplicationQuery = new GetHostApplicationQuery
        {
            Id = id,
            UserId = userId
        };
        await Assert.ThrowsAsync<NotFoundException>(async () => await _getHostApplicationQueryHandler.Handle(getHostApplicationQuery, CancellationToken.None));
    }
    #endregion

}
