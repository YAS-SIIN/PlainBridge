

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
 
using PlainBridge.Api.Application.Handler.Bus;
using PlainBridge.Api.Application.Services.ServerApplication;
using PlainBridge.Api.Domain.Entities;
using PlainBridge.SharedApplication.DTOs;
using PlainBridge.SharedApplication.Exceptions;

namespace PlainBridge.Api.UnitTests.Application.Services;

public class ServerApplicationServiceTests : IClassFixture<TestRunFixture>
{
    private readonly TestRunFixture _fixture;
    private readonly ServerApplicationService serverApplicationService;
    private readonly Mock<ILogger<ServerApplicationService>> mockLoggerProjectService;
    private readonly Mock<IBusHandler> _mockBusHandler;

    public ServerApplicationServiceTests(TestRunFixture fixture)
    {
        _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
        mockLoggerProjectService = new Mock<ILogger<ServerApplicationService>>();
        _mockBusHandler = new Mock<IBusHandler>();
        serverApplicationService = new ServerApplicationService(mockLoggerProjectService.Object, _fixture.MemoryMainDbContext, _mockBusHandler.Object);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnData()
    {
        var result = await serverApplicationService.GetAllAsync(CancellationToken.None);
        Assert.NotNull(result);
        Assert.True(result.Count > 0);
    }

    #region GetByIdAsync 
    [Theory]
    [InlineData(1)]
    public async Task GetByIdAsync_WhenIdExists_ShouldReturnData(long id)
    {
        var result = await serverApplicationService.GetByIdAsync(id, CancellationToken.None);
        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
    }

    [Theory]
    [InlineData(999)]
    public async Task GetByIdAsync_WhenIdDoesntExist_ShouldReturnNull(long id)
    {
        await Assert.ThrowsAsync<NotFoundException>(async () => await serverApplicationService.GetByIdAsync(id, CancellationToken.None));
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

        var guid = await serverApplicationService.CreateAsync(dto, CancellationToken.None);
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
        var res = await Assert.ThrowsAsync<ApplicationException>(() => serverApplicationService.CreateAsync(dto, CancellationToken.None));
        Assert.NotNull(res);
        Assert.Equal("Port range is not valid", res.Message);
    }
    
    [Theory]
    [MemberData(nameof(ServerApplicationServiceData.SetDataFor_CreateAsync_WhenServerApplicationViewIdIsEmpty_ShouldThrowException), MemberType = typeof(ServerApplicationServiceData))]
    public async Task CreateAsync_WhenServerApplicationViewIdIsEmpty_ShouldThrowException(ServerApplicationDto dto)
    {
        var res = await Assert.ThrowsAsync<ArgumentNullException>(() => serverApplicationService.CreateAsync(dto, CancellationToken.None));
        Assert.NotNull(res);
        Assert.Equal(nameof(ServerApplicationDto.ServerApplicationAppId), res.ParamName);
    }


    #endregion

    #region DeleteAsync

    [Theory]
    [InlineData(3)]
    public async Task DeleteAsync_WhenEveryThingIsOk_ShouldBeSucceeded(int id)
    {
        await serverApplicationService.DeleteAsync(id, CancellationToken.None);
        var deleted = await _fixture.MemoryMainDbContext.ServerApplications.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        Assert.Null(deleted);
    }

    [Theory]
    [InlineData(9999)]
    public async Task DeleteAsync_WhenIdDoesntExist_ShouldThrowException(int id)
    {
        var res = await Assert.ThrowsAsync<NotFoundException>(() => serverApplicationService.DeleteAsync(id, CancellationToken.None));
        Assert.NotNull(res);
    }
    #endregion

    #region UpdateAsync

    [Theory]
    [MemberData(nameof(ServerApplicationServiceData.SetDataFor_UpdateAsync_WhenEveryThingIsOk_ShouldBeSucceeded), MemberType = typeof(ServerApplicationServiceData))]
    public async Task UpdateAsync_WhenEveryThingIsOk_ShouldBeSucceeded(ServerApplicationDto dto)
    {
        await serverApplicationService.UpdateAsync(dto, CancellationToken.None);

        var updated = await _fixture.MemoryMainDbContext.ServerApplications.AsNoTracking().FirstOrDefaultAsync(x => x.Id == dto.Id);

        Assert.NotNull(updated);
        Assert.Equal(dto.Name, updated.Name); 
        Assert.Equal(dto.InternalPort, updated.InternalPort);
    }

    [Theory]
    [MemberData(nameof(ServerApplicationServiceData.SetDataFor_UpdateAsync_WhenIdDoesntExist_ShouldThrowException), MemberType = typeof(ServerApplicationServiceData))]
    public async Task UpdateAsync_WhenIdDoesntExist_ShouldThrowException(ServerApplicationDto dto)
    {
       var res = await Assert.ThrowsAsync<NotFoundException>(() => serverApplicationService.UpdateAsync(dto, CancellationToken.None));
        Assert.NotNull(res);
    }

    [Theory]
    [MemberData(nameof(ServerApplicationServiceData.SetDataFor_UpdateAsync_WhenIdDoesntExist_ShouldThrowApplicationException), MemberType = typeof(ServerApplicationServiceData))]
    public async Task UpdateAsync_WhenIdDoesntExist_ShouldThrowApplicationException(ServerApplicationDto dto)
    {
        var res = await Assert.ThrowsAsync<NotFoundException>(() => serverApplicationService.UpdateAsync(dto, CancellationToken.None));
        Assert.NotNull(res);
    }

    [Theory]
    [MemberData(nameof(ServerApplicationServiceData.SetDataFor_UpdateAsync_WhenServerApplicationViewIdIsEmpty_ShouldThrowException), MemberType = typeof(ServerApplicationServiceData))]
    public async Task UpdateAsync_WhenServerApplicationViewIdIsEmpty_ShouldThrowException(ServerApplicationDto dto)
    {
        var res = await Assert.ThrowsAsync<ArgumentNullException>(() => serverApplicationService.UpdateAsync(dto, CancellationToken.None));
        Assert.NotNull(res);
        Assert.Equal(nameof(ServerApplicationDto.ServerApplicationAppId), res.ParamName);
    }

    #endregion

}
