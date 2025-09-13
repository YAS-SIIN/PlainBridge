

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using PlainBridge.Api.Application.UseCases.HostApplication.Commands;
using PlainBridge.Api.Domain.HostAggregate.ValueObjects;
using PlainBridge.Api.Infrastructure.ExternalServices.Messaging;
using PlainBridge.SharedApplication.DTOs;
using PlainBridge.SharedApplication.Exceptions;
using PlainBridge.SharedDomain.Base.Enums;

namespace PlainBridge.Api.Tests.UnitTests.Application.UseCases.HostApplication;

[Collection("ApiUnitTestRun")]
public class HostApplicationCommandsTests : IClassFixture<TestRunFixture>
{
    private readonly TestRunFixture _fixture;
    private readonly CreateHostApplicationCommandHandler _createHandler;
    private readonly UpdateHostApplicationCommandHandler _updateHandler;
    private readonly DeleteHostApplicationCommandHandler _deleteHandler;
    private readonly UpdateStateHostApplicationCommandHandler _updateStateHandler;

    private readonly Mock<IEventBus> _mockEventBus;


    public HostApplicationCommandsTests(TestRunFixture fixture)
    {
        _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
        _mockEventBus = new Mock<IEventBus>();

        _createHandler = new CreateHostApplicationCommandHandler(new Mock<ILogger<CreateHostApplicationCommandHandler>>().Object, _fixture.MemoryMainDbContext, _mockEventBus.Object);

        _updateHandler = new UpdateHostApplicationCommandHandler(new Mock<ILogger<UpdateHostApplicationCommandHandler>>().Object, _fixture.MemoryMainDbContext, _mockEventBus.Object);

        _deleteHandler = new DeleteHostApplicationCommandHandler(new Mock<ILogger<DeleteHostApplicationCommandHandler>>().Object, _fixture.MemoryMainDbContext, _mockEventBus.Object);

        _updateStateHandler = new UpdateStateHostApplicationCommandHandler(new Mock<ILogger<UpdateStateHostApplicationCommandHandler>>().Object, _fixture.MemoryMainDbContext, _mockEventBus.Object);

    }

     
    #region CreateHostApplicationCommandHandler

    [Theory]
    [MemberData(nameof(HostApplicationCommandsData.SetDataFor_CreateHostApplicationCommandHandler_WhenEveryThingIsOk_ShouldBeSucceeded), MemberType = typeof(HostApplicationCommandsData))]
    public async Task CreateHostApplicationCommandHandler_WhenEveryThingIsOk_ShouldBeSucceeded(CreateHostApplicationCommand dto)
    {
        var guid = await _createHandler.Handle(dto, CancellationToken.None);
        Assert.NotEqual(Guid.Empty, guid);

        var created = await _fixture.MemoryMainDbContext.HostApplications.FirstOrDefaultAsync(x => x.AppId.ViewId == guid);
        Assert.NotNull(created);
        Assert.Equal("NewApp", created.Name);
    }

    [Theory]
    [MemberData(nameof(HostApplicationCommandsData.CreateHostApplicationCommandHandler_WhenDomainIsExisted_ShouldThrowDuplicatedException), MemberType = typeof(HostApplicationCommandsData))]
    public async Task CreateHostApplicationCommandHandler_WhenDomainIsExisted_ShouldThrowDuplicatedException(CreateHostApplicationCommand dto)
    {
        await Assert.ThrowsAsync<DuplicatedException>(() => _createHandler.Handle(dto, CancellationToken.None));
    }
    #endregion

    #region DeleteHostApplicationCommandHandler

    [Theory]
    [InlineData(3)]
    public async Task DeleteHostApplicationCommandHandler_WhenEveryThingIsOk_ShouldBeSucceeded(int id)
    {

        await _deleteHandler.Handle(new DeleteHostApplicationCommand { Id = id }, CancellationToken.None);
        var deleted = await _fixture.MemoryMainDbContext.HostApplications.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        Assert.Null(deleted);
    }

    [Theory]
    [InlineData(9999)]
    public async Task DeleteHostApplicationCommandHandler_WhenIdDoesntExist_ShouldThrowNotFoundException(int id)
    { 
        var res = await Assert.ThrowsAsync<NotFoundException>(() => _deleteHandler.Handle(new DeleteHostApplicationCommand { Id = id }, CancellationToken.None));
        Assert.NotNull(res);
    }
    #endregion

    #region UpdateHostApplicationCommandHandler

    [Theory]
    [MemberData(nameof(HostApplicationCommandsData.SetDataFor_UpdateHostApplicationCommandHandler_WhenEveryThingIsOk_ShouldBeSucceeded), MemberType = typeof(HostApplicationCommandsData))]
    public async Task UpdateHostApplicationCommandHandler_WhenEveryThingIsOk_ShouldBeSucceeded(UpdateHostApplicationCommand dto)
    {
        await _updateHandler.Handle(dto, CancellationToken.None);

        var updated = await _fixture.MemoryMainDbContext.HostApplications.AsNoTracking().FirstOrDefaultAsync(x => x.Id == dto.Id);

        Assert.NotNull(updated);
        Assert.Equal(dto.Name, updated.Name);
        Assert.Equal(dto.Domain, updated.Domain.HostDomainName);
        Assert.Equal(dto.InternalUrl, updated.InternalUrl.InternalUrlValue);
    }
    [Theory]
    [MemberData(nameof(HostApplicationCommandsData.SetDataFor_UpdateHostApplicationCommandHandler_WhenDomainIsExisted_ShouldThrowApplicationException), MemberType = typeof(HostApplicationCommandsData))]
    public async Task UpdateHostApplicationCommandHandler_WhenDomainIsExisted_ShouldThrowApplicationException(UpdateHostApplicationCommand dto)
    {
        await Assert.ThrowsAsync<ApplicationException>(() => _updateHandler.Handle(dto, CancellationToken.None));
    }

    [Theory]
    [MemberData(nameof(HostApplicationCommandsData.SetDataFor_UpdateHostApplicationCommandHandler_WhenIdDoesntExist_ShouldThrowApplicationException), MemberType = typeof(HostApplicationCommandsData))]
    public async Task UpdateHostApplicationCommandHandler_WhenIdDoesntExist_ShouldThrowApplicationException(UpdateHostApplicationCommand dto)
    {
        await Assert.ThrowsAsync<NotFoundException>(() => _updateHandler.Handle(dto, CancellationToken.None));
    }
    #endregion



    #region UpdateStateHostApplicationCommandHandler

    [Theory]
    [InlineData(2, true)]
    [InlineData(2, false)]
    public async Task UpdateStateHostApplicationCommandHandler_WhenEveryThingIsOk_ShouldBeSucceeded(int id, bool IsActive)
    {
        await _updateStateHandler.Handle(new UpdateStateHostApplicationCommand { Id = id, IsActive = IsActive }, CancellationToken.None);

        var updated = await _fixture.MemoryMainDbContext.HostApplications.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

        Assert.NotNull(updated);
        Assert.Equal(id, updated.Id);


        Assert.Equal(IsActive ? RowStateEnum.Active : RowStateEnum.DeActive, updated.State);
    }


    [Theory]
    [InlineData(999, true)]
    [InlineData(999, false)]
    public async Task UpdateStateHostApplicationCommandHandler_WhenDomainIsExisted_ShouldThrowNotFoundException(int id, bool IsActive)
    {
        await Assert.ThrowsAsync<NotFoundException>(() => _updateStateHandler.Handle(new UpdateStateHostApplicationCommand { Id = id, IsActive = IsActive }, CancellationToken.None));
    }
    #endregion

}
