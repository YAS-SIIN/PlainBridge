

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using PlainBridge.Api.Application.UseCases.HostApplication.Commands;
using PlainBridge.Api.Application.UseCases.ServerApplication.Commands;
using PlainBridge.Api.Infrastructure.ExternalServices.Messaging;
using PlainBridge.Api.UnitTests.Application.Services;
using PlainBridge.SharedApplication.DTOs;
using PlainBridge.SharedApplication.Exceptions;
using PlainBridge.SharedDomain.Base.Enums;

namespace PlainBridge.Api.UnitTests.Application.UseCases.HostApplication;

[Collection("ApiUnitTestRun")]
public class HostApplicationCommandsTests : IClassFixture<TestRunFixture>
{
    private readonly TestRunFixture _fixture;
    private readonly CreateHostApplicationCommandHandler _createHostApplicationCommandHandler;
    private readonly UpdateHostApplicationCommandHandler _updateHostApplicationCommandHandler;
    private readonly DeleteHostApplicationCommandHandler _deleteHostApplicationCommandHandler;
    private readonly UpdateStateHostApplicationCommandHandler _updateStateHostApplicationCommandHandler;

    private readonly Mock<IEventBus> _mockEventBus;


    public HostApplicationCommandsTests(TestRunFixture fixture)
    {
        _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
        _mockEventBus = new Mock<IEventBus>();

        _createHostApplicationCommandHandler = new CreateHostApplicationCommandHandler(new Mock<ILogger<CreateHostApplicationCommandHandler>>().Object, _fixture.MemoryMainDbContext, _mockEventBus.Object);

        _updateHostApplicationCommandHandler = new UpdateHostApplicationCommandHandler(new Mock<ILogger<UpdateHostApplicationCommandHandler>>().Object, _fixture.MemoryMainDbContext, _mockEventBus.Object);

        _deleteHostApplicationCommandHandler = new DeleteHostApplicationCommandHandler(new Mock<ILogger<DeleteHostApplicationCommandHandler>>().Object, _fixture.MemoryMainDbContext, _mockEventBus.Object);

        _updateStateHostApplicationCommandHandler = new UpdateStateHostApplicationCommandHandler(new Mock<ILogger<UpdateStateHostApplicationCommandHandler>>().Object, _fixture.MemoryMainDbContext, _mockEventBus.Object);

    }

     
    #region CreateHostApplicationCommandHandler

    [Theory]
    [MemberData(nameof(HostApplicationCommandsData.SetDataFor_CreateHostApplicationCommandHandler_WhenEveryThingIsOk_ShouldBeSucceeded), MemberType = typeof(HostApplicationCommandsData))]
    public async Task CreateHostApplicationCommandHandler_WhenEveryThingIsOk_ShouldBeSucceeded(CreateHostApplicationCommand dto)
    {
        var guid = await _createHostApplicationCommandHandler.Handle(dto, CancellationToken.None);
        Assert.NotEqual(Guid.Empty, guid);

        var created = await _fixture.MemoryMainDbContext.HostApplications.FirstOrDefaultAsync(x => x.AppId.ViewId == guid);
        Assert.NotNull(created);
        Assert.Equal("NewApp", created.Name);
    }

    [Theory]
    [MemberData(nameof(HostApplicationCommandsData.SetDataFor_CreateHostApplicationCommandHandler_WhenDomainIsExisted_ShouldThrowException), MemberType = typeof(HostApplicationCommandsData))]
    public async Task CreateHostApplicationCommandHandler__WhenDomainIsExisted_ShouldThrowException(CreateHostApplicationCommand dto)
    {
        await Assert.ThrowsAsync<DuplicatedException>(() => _createHostApplicationCommandHandler.Handle(dto, CancellationToken.None));
    }
    #endregion

    #region DeleteHostApplicationCommandHandler

    [Theory]
    [InlineData(3)]
    public async Task DeleteHostApplicationCommandHandler_WhenEveryThingIsOk_ShouldBeSucceeded(int id)
    {

        await _deleteHostApplicationCommandHandler.Handle(new DeleteHostApplicationCommand { Id = id }, CancellationToken.None);
        var deleted = await _fixture.MemoryMainDbContext.HostApplications.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        Assert.Null(deleted);
    }

    [Theory]
    [InlineData(9999)]
    public async Task DeleteHostApplicationCommandHandler_WhenIdDoesntExist_ShouldThrowException(int id)
    { 
        var res = await Assert.ThrowsAsync<NotFoundException>(() => _deleteHostApplicationCommandHandler.Handle(new DeleteHostApplicationCommand { Id = id }, CancellationToken.None));
        Assert.NotNull(res);
    }
    #endregion

    #region UpdateHostApplicationCommandHandler

    [Theory]
    [MemberData(nameof(HostApplicationCommandsData.SetDataFor_UpdateHostApplicationCommandHandler_WhenEveryThingIsOk_ShouldBeSucceeded), MemberType = typeof(HostApplicationCommandsData))]
    public async Task UpdateHostApplicationCommandHandler_WhenEveryThingIsOk_ShouldBeSucceeded(UpdateHostApplicationCommand dto)
    {
        await _updateHostApplicationCommandHandler.Handle(dto, CancellationToken.None);

        var updated = await _fixture.MemoryMainDbContext.HostApplications.AsNoTracking().FirstOrDefaultAsync(x => x.Id == dto.Id);

        Assert.NotNull(updated);
        Assert.Equal(dto.Name, updated.Name);
        Assert.Equal(dto.Domain, updated.Domain.HostDomainName);
        Assert.Equal(dto.InternalUrl, updated.InternalUrl.InternalUrlValue);
    }
    [Theory]
    [MemberData(nameof(HostApplicationCommandsData.SetDataFor_UpdateHostApplicationCommandHandler_WhenDomainIsExisted_ShouldThrowException), MemberType = typeof(HostApplicationCommandsData))]
    public async Task UpdateHostApplicationCommandHandler_WhenDomainIsExisted_ShouldThrowException(UpdateHostApplicationCommand dto)
    {
        await Assert.ThrowsAsync<ApplicationException>(() => _updateHostApplicationCommandHandler.Handle(dto, CancellationToken.None));
    }

    [Theory]
    [MemberData(nameof(HostApplicationCommandsData.SetDataFor_UpdateHostApplicationCommandHandler_WhenIdDoesntExist_ShouldThrowApplicationException), MemberType = typeof(HostApplicationCommandsData))]
    public async Task UpdateHostApplicationCommandHandler_WhenIdDoesntExist_ShouldThrowApplicationException(UpdateHostApplicationCommand dto)
    {
        await Assert.ThrowsAsync<NotFoundException>(() => _updateHostApplicationCommandHandler.Handle(dto, CancellationToken.None));
    }
    #endregion



    #region UpdateStateHostApplicationCommandHandler

    [Theory]
    [InlineData(2, true)]
    [InlineData(2, false)]
    public async Task UpdateStateHostApplicationCommandHandler_WhenEveryThingIsOk_ShouldBeSucceeded(int id, bool IsActive)
    {
        await _updateStateHostApplicationCommandHandler.Handle(new UpdateStateHostApplicationCommand { Id = id, IsActive = IsActive }, CancellationToken.None);

        var updated = await _fixture.MemoryMainDbContext.HostApplications.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

        Assert.NotNull(updated);
        Assert.Equal(id, updated.Id);
        Assert.Equal(RowStateEnum.Active, updated.State);
    }


    [Theory]
    [InlineData(999, true)]
    [InlineData(999, false)]
    public async Task UpdateStateHostApplicationCommandHandler_WhenDomainIsExisted_ShouldThrowException(int id, bool IsActive)
    {
        await Assert.ThrowsAsync<NotFoundException>(() => _updateStateHostApplicationCommandHandler.Handle(new UpdateStateHostApplicationCommand { Id = id, IsActive = IsActive }, CancellationToken.None));
    }
    #endregion

}
