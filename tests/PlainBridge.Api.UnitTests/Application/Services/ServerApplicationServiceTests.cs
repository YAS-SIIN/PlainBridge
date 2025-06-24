

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq; 
using PlainBridge.Api.Application.Services.ServerApplication;
using PlainBridge.Api.Domain.Entities;
using PlainBridge.Api.Infrastructure.Messaging;
using PlainBridge.SharedApplication.DTOs;
using PlainBridge.SharedApplication.Exceptions;

namespace PlainBridge.Api.UnitTests.Application.Services;

[Collection("TestRun")]
public class ServerApplicationServiceTests : IClassFixture<TestRunFixture>
{
    private readonly TestRunFixture _fixture;
    private readonly IServerApplicationService _serverApplicationService;
    private readonly Mock<ILogger<ServerApplicationService>> _mockLoggerServerApplicationService;
    private readonly Mock<IEventBus> _mockEventBus;

    public ServerApplicationServiceTests(TestRunFixture fixture)
    {
        _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
        _mockLoggerServerApplicationService = new Mock<ILogger<ServerApplicationService>>();
        _mockEventBus = new Mock<IEventBus>();
        _serverApplicationService = new ServerApplicationService(_mockLoggerServerApplicationService.Object, _fixture.MemoryMainDbContext, _mockEventBus.Object);
    }

    [Theory]
    [InlineData(1)]
    public async Task GetAllAsync_WhenEveryThingIsOk_ShouldReturnData(long userId)
    {
        var result = await _serverApplicationService.GetAllAsync(userId, CancellationToken.None);
        Assert.NotNull(result);
        Assert.True(result.Count > 0);
    }

    #region GetAsync 
    [Theory]
    [InlineData(1, 1)]
    public async Task GetAsync_WhenEveryThingIsOk_ShouldReturnData(long id, long userId)
    {
        var result = await _serverApplicationService.GetAsync(id, userId, CancellationToken.None);
        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
    }

    [Theory]
    [InlineData(999, 1)]
    public async Task GetAsync_WhenIdDoesntExist__ShouldThrowException(long id, long userId)
    {
        await Assert.ThrowsAsync<NotFoundException>(async () => await _serverApplicationService.GetAsync(id, userId, CancellationToken.None));
    }
    #endregion

    #region CreateAsync

    [Theory]
    [MemberData(nameof(ServerApplicationServiceData.SetDataFor_CreateAsync_WhenEveryThingIsOk_ShouldBeSucceeded), MemberType = typeof(ServerApplicationServiceData))]
    public async Task CreateAsync_WhenEveryThingIsOk_ShouldBeSucceeded(ServerApplicationDto dto)
    {
        if (dto.ServerApplicationType == SharedApplication.Enums.ServerApplicationTypeEnum.UsePort)
        {
            var serverApplication = await _fixture.MemoryMainDbContext.ServerApplications.FirstOrDefaultAsync();
            dto.ServerApplicationAppId = serverApplication.AppId;
        }

        var guid = await _serverApplicationService.CreateAsync(dto, CancellationToken.None);
        Assert.NotEqual(Guid.Empty, guid);

        var created = await _fixture.MemoryMainDbContext.ServerApplications.FirstOrDefaultAsync(x => x.AppId == guid);
        Assert.NotNull(created);
        Assert.Equal(dto.Name, created.Name);
        Assert.Equal(dto.InternalPort, created.InternalPort);
    }

     
    [Theory]
    [MemberData(nameof(ServerApplicationServiceData.SetDataFor_CreateAsync_WhenInternalPortIsNotValid_ShouldThrowException), MemberType = typeof(ServerApplicationServiceData))]
    public async Task CreateAsync_WhenInternalPortIsNotValid_ShouldThrowException(ServerApplicationDto dto)
    {
        var res = await Assert.ThrowsAsync<ApplicationException>(() => _serverApplicationService.CreateAsync(dto, CancellationToken.None));
        Assert.NotNull(res);
        Assert.Equal("Port range is not valid", res.Message);
    }
    
    [Theory]
    [MemberData(nameof(ServerApplicationServiceData.SetDataFor_CreateAsync_WhenServerApplicationViewIdIsEmpty_ShouldThrowException), MemberType = typeof(ServerApplicationServiceData))]
    public async Task CreateAsync_WhenServerApplicationViewIdIsEmpty_ShouldThrowException(ServerApplicationDto dto)
    {
        var res = await Assert.ThrowsAsync<ArgumentNullException>(() => _serverApplicationService.CreateAsync(dto, CancellationToken.None));
        Assert.NotNull(res);
        Assert.Equal(nameof(ServerApplicationDto.ServerApplicationAppId), res.ParamName);
    }


    #endregion

    #region DeleteAsync

    [Theory]
    [InlineData(3)]
    public async Task DeleteAsync_WhenEveryThingIsOk_ShouldBeSucceeded(int id)
    {
        await _serverApplicationService.DeleteAsync(id, CancellationToken.None);
        var deleted = await _fixture.MemoryMainDbContext.ServerApplications.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        Assert.Null(deleted);
    }

    [Theory]
    [InlineData(9999)]
    public async Task DeleteAsync_WhenIdDoesntExist_ShouldThrowException(int id)
    {
        var res = await Assert.ThrowsAsync<NotFoundException>(() => _serverApplicationService.DeleteAsync(id, CancellationToken.None));
        Assert.NotNull(res);
    }
    #endregion

    #region UpdateAsync

    [Theory]
    [MemberData(nameof(ServerApplicationServiceData.SetDataFor_UpdateAsync_WhenEveryThingIsOk_ShouldBeSucceeded), MemberType = typeof(ServerApplicationServiceData))]
    public async Task UpdateAsync_WhenEveryThingIsOk_ShouldBeSucceeded(ServerApplicationDto dto)
    {
        await _serverApplicationService.UpdateAsync(dto, CancellationToken.None);

        var updated = await _fixture.MemoryMainDbContext.ServerApplications.AsNoTracking().FirstOrDefaultAsync(x => x.Id == dto.Id);

        Assert.NotNull(updated);
        Assert.Equal(dto.Name, updated.Name); 
        Assert.Equal(dto.InternalPort, updated.InternalPort);
    }

    [Theory]
    [MemberData(nameof(ServerApplicationServiceData.SetDataFor_UpdateAsync_WhenIdDoesntExist_ShouldThrowException), MemberType = typeof(ServerApplicationServiceData))]
    public async Task UpdateAsync_WhenIdDoesntExist_ShouldThrowException(ServerApplicationDto dto)
    {
       var res = await Assert.ThrowsAsync<NotFoundException>(() => _serverApplicationService.UpdateAsync(dto, CancellationToken.None));
        Assert.NotNull(res);
    }

    [Theory]
    [MemberData(nameof(ServerApplicationServiceData.SetDataFor_UpdateAsync_WhenIdDoesntExist_ShouldThrowApplicationException), MemberType = typeof(ServerApplicationServiceData))]
    public async Task UpdateAsync_WhenIdDoesntExist_ShouldThrowApplicationException(ServerApplicationDto dto)
    {
        var res = await Assert.ThrowsAsync<NotFoundException>(() => _serverApplicationService.UpdateAsync(dto, CancellationToken.None));
        Assert.NotNull(res);
    }

    #endregion

}
