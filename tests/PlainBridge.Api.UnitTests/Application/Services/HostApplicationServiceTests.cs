
using Humanizer;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Moq;

using PlainBridge.Api.Application.DTOs;
using PlainBridge.Api.Application.Exceptions;
using PlainBridge.Api.Application.Handler.Bus;
using PlainBridge.Api.Application.Services.HostApplication;

namespace PlainBridge.Api.UnitTests.Application.Services;

public class HostApplicationServiceTests : IClassFixture<TestRunFixture>
{
    private readonly TestRunFixture _fixture;
    private readonly IHostApplicationService hostApplicationService;
    private readonly Mock<ILogger<HostApplicationService>> mockLoggerProjectService;
    private readonly Mock<IBusHandler> _mockBusHandler;

    public HostApplicationServiceTests(TestRunFixture fixture)
    {
        _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
        mockLoggerProjectService = new Mock<ILogger<HostApplicationService>>();
        _mockBusHandler = new Mock<IBusHandler>();
        hostApplicationService = new HostApplicationService(mockLoggerProjectService.Object, _fixture.MemoryMainDbContext, _mockBusHandler.Object);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnData()
    {
        var result = await hostApplicationService.GetAllAsync(CancellationToken.None);
        Assert.NotNull(result);
        Assert.True(result.Count > 0);
    }

    #region GetByIdAsync 
    [Theory]
    [InlineData(1)]
    public async Task GetByIdAsync_WhenIdExists_ShouldReturnData(long id)
    {
        var result = await hostApplicationService.GetByIdAsync(id, CancellationToken.None);
        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
    }

    [Theory]
    [InlineData(999)]
    public async Task GetByIdAsync_WhenIdDoesntExist_ShouldReturnNull(long id)
    {
        await Assert.ThrowsAsync<NotFoundException>(async () => await hostApplicationService.GetByIdAsync(id, CancellationToken.None));
    }
    #endregion

    #region CreateAsync

    [Theory]
    [MemberData(nameof(HostApplicationServiceData.SetDataFor_CreateAsync_WhenEveryThingIsOk_ShouldBeSucceeded), MemberType = typeof(HostApplicationServiceData))]
    public async Task CreateAsync_WhenEveryThingIsOk_ShouldBeSucceeded(HostApplicationDto dto)
    {
        var guid = await hostApplicationService.CreateAsync(dto, CancellationToken.None);
        Assert.NotEqual(Guid.Empty, guid);

        var created = await _fixture.MemoryMainDbContext.HostApplications.FirstOrDefaultAsync(x => x.AppId == guid);
        Assert.NotNull(created);
        Assert.Equal("NewApp", created.Name);
    }
    
    [Theory]
    [MemberData(nameof(HostApplicationServiceData.SetDataFor_CreateAsync_WhenDomainIsExisted_ShouldThrowException), MemberType = typeof(HostApplicationServiceData))]
    public async Task CreateAsync_WhenDomainIsExisted_ShouldThrowException(HostApplicationDto dto)
    {
        await Assert.ThrowsAsync<DuplicatedException>(() => hostApplicationService.CreateAsync(dto, CancellationToken.None));
    }
    #endregion

    #region DeleteAsync

    [Theory]
    [InlineData(3)]
    public async Task DeleteAsync_WhenEveryThingIsOk_ShouldBeSucceeded(int id)
    {

        await hostApplicationService.DeleteAsync(id, CancellationToken.None);
        var deleted = await _fixture.MemoryMainDbContext.HostApplications.AsNoTracking().FirstOrDefaultAsync(x=>x.Id == id);
        Assert.Null(deleted);
    }

    [Theory]
    [InlineData(9)]
    public async Task DeleteAsync_WhenIdDoesntExist_ShouldThrowException(int id)
    {
        await Assert.ThrowsAsync<NotFoundException>(() => hostApplicationService.DeleteAsync(9999, CancellationToken.None));
    }
    #endregion

    #region UpdateAsync

    [Theory]
    [MemberData(nameof(HostApplicationServiceData.SetDataFor_UpdateAsync_WhenEveryThingIsOk_ShouldBeSucceeded), MemberType = typeof(HostApplicationServiceData))]
    public async Task UpdateAsync_WhenEveryThingIsOk_ShouldBeSucceeded(HostApplicationDto dto)
    {
        await hostApplicationService.UpdateAsync(dto, CancellationToken.None);

        var updated = await _fixture.MemoryMainDbContext.HostApplications.AsNoTracking().FirstOrDefaultAsync(x=>x.Id == dto.Id);

        Assert.NotNull(updated);
        Assert.Equal(dto.Name, updated.Name);
        Assert.Equal(dto.Domain, updated.Domain);
        Assert.Equal(dto.InternalUrl, updated.InternalUrl);
    } 
    [Theory]
    [MemberData(nameof(HostApplicationServiceData.SetDataFor_UpdateAsync_WhenDomainIsExisted_ShouldThrowException), MemberType = typeof(HostApplicationServiceData))]
    public async Task UpdateAsync_WhenDomainIsExisted_ShouldThrowException(HostApplicationDto dto)
    {
        await Assert.ThrowsAsync<ApplicationException>(() => hostApplicationService.UpdateAsync(dto, CancellationToken.None));
    } 

    [Theory]
    [MemberData(nameof(HostApplicationServiceData.SetDataFor_UpdateAsync_WhenIdDoesntExist_ShouldThrowApplicationException), MemberType = typeof(HostApplicationServiceData))]
    public async Task UpdateAsync_WhenIdDoesntExist_ShouldThrowApplicationException(HostApplicationDto dto)
    {
        await Assert.ThrowsAsync<NotFoundException>(() => hostApplicationService.UpdateAsync(dto, CancellationToken.None));
    }
    #endregion


}
