

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using PlainBridge.Api.Application.UseCases.ServerApplication.Commands;
using PlainBridge.Api.Infrastructure.ExternalServices.Messaging;
using PlainBridge.Api.Tests.UnitTests.Application.Utils;
using PlainBridge.SharedApplication.Exceptions;
using PlainBridge.SharedDomain.Base.Enums;

namespace PlainBridge.Api.Tests.UnitTests.Application.UseCases.ServerApplication;

[Collection("ApiUnitTestRun")]
public class ServerApplicationCommandsTests : IClassFixture<ApiApplcationUnitTestRunFixture>
{
    private readonly ApiApplcationUnitTestRunFixture _fixture;
    private readonly CreateServerApplicationCommandHandler _createHandler;
    private readonly UpdateServerApplicationCommandHandler _updateHandler;
    private readonly DeleteServerApplicationCommandHandler _deleteHandler;
    private readonly UpdateStateServerApplicationCommandHandler _updateStateHandler;

    private readonly Mock<IEventBus> _mockEventBus;


    public ServerApplicationCommandsTests(ApiApplcationUnitTestRunFixture fixture)
    {
        _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
        _mockEventBus = new Mock<IEventBus>();

        _createHandler = new CreateServerApplicationCommandHandler(new Mock<ILogger<CreateServerApplicationCommandHandler>>().Object, _fixture.MemoryMainDbContext, _mockEventBus.Object);

        _updateHandler = new UpdateServerApplicationCommandHandler(new Mock<ILogger<UpdateServerApplicationCommandHandler>>().Object, _fixture.MemoryMainDbContext, _mockEventBus.Object);

        _deleteHandler = new DeleteServerApplicationCommandHandler(new Mock<ILogger<DeleteServerApplicationCommandHandler>>().Object, _fixture.MemoryMainDbContext, _mockEventBus.Object);

        _updateStateHandler = new UpdateStateServerApplicationCommandHandler(new Mock<ILogger<UpdateStateServerApplicationCommandHandler>>().Object, _fixture.MemoryMainDbContext, _mockEventBus.Object);

    }


    #region CreateServerApplicationCommandHandler

    [Theory]
    [MemberData(nameof(ServerApplicationCommandsData.SetDataFor_CreateServerApplicationCommandHandler_WhenEveryThingIsOk_ShouldBeSucceeded), MemberType = typeof(ServerApplicationCommandsData))]
    public async Task CreateServerApplicationCommandHandler_WhenEveryThingIsOk_ShouldBeSucceeded(CreateServerApplicationCommand dto)
    {
        if (dto.ServerApplicationType == SharedApplication.Enums.ServerApplicationTypeEnum.UsePort)
        {
            var serverApplication = await _fixture.MemoryMainDbContext.ServerApplications.FirstOrDefaultAsync();
            dto.ServerApplicationAppId = serverApplication!.AppId.ViewId.ToString();
        }

        var guid = await _createHandler.Handle(dto, CancellationToken.None);
        Assert.NotEqual(Guid.Empty, guid);

        var created = await _fixture.MemoryMainDbContext.ServerApplications.FirstOrDefaultAsync(x => x.AppId.ViewId == guid);
        Assert.NotNull(created);
        Assert.Equal(dto.Name, created.Name);
        Assert.Equal(dto.InternalPort, created.InternalPort.Port);
    }

    [Theory]
    [MemberData(nameof(ServerApplicationCommandsData.SetDataFor_CreateServerApplicationCommandHandler_WhenInternalPortIsNotValid_ShouldThrowApplicationException), MemberType = typeof(ServerApplicationCommandsData))]
    public async Task CreateServerApplicationCommandHandler_WhenInternalPortIsNotValid_ShouldThrowApplicationException(CreateServerApplicationCommand dto)
    {
        var res = await Assert.ThrowsAsync<ApplicationException>(() => _createHandler.Handle(dto, CancellationToken.None));
        Assert.NotNull(res);
        Assert.Equal("Port range is not valid (1-65535).", res.Message);
    }


    [Theory]
    [MemberData(nameof(ServerApplicationCommandsData.SetDataFor_CreateServerApplicationCommandHandler_WhenServerApplicationViewIdIsEmpty_ShouldThrowNotFoundException), MemberType = typeof(ServerApplicationCommandsData))]
    public async Task CreateServerApplicationCommandHandler_WhenServerApplicationViewIdIsEmpty_ShouldThrowNotFoundException(CreateServerApplicationCommand dto)
    {
        var res = await Assert.ThrowsAsync<NotFoundException>(() => _createHandler.Handle(dto, CancellationToken.None));
        Assert.NotNull(res);
        //Assert.Equal(nameof(NotFoundException.ServerApplicationAppId), res.ParamName);
    }

    #endregion

    #region DeleteServerApplicationCommandHandler

    [Theory]
    [InlineData(3)]
    public async Task DeleteServerApplicationCommandHandler_WhenEveryThingIsOk_ShouldBeSucceeded(int id)
    {

        await _deleteHandler.Handle(new DeleteServerApplicationCommand { Id = id }, CancellationToken.None);
        var deleted = await _fixture.MemoryMainDbContext.ServerApplications.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        Assert.Null(deleted);
    }

    [Theory]
    [InlineData(9999)]
    public async Task DeleteServerApplicationCommandHandler_WhenIdDoesntExist_ShouldThrowNotFoundException(int id)
    {
        var res = await Assert.ThrowsAsync<NotFoundException>(() => _deleteHandler.Handle(new DeleteServerApplicationCommand { Id = id }, CancellationToken.None));
        Assert.NotNull(res);

    }
    #endregion

    #region UpdateServerApplicationCommandHandler

    [Theory]
    [MemberData(nameof(ServerApplicationCommandsData.SetDataFor_UpdateServerApplicationCommandHandler_WhenEveryThingIsOk_ShouldBeSucceeded), MemberType = typeof(ServerApplicationCommandsData))]
    public async Task UpdateServerApplicationCommandHandler_WhenEveryThingIsOk_ShouldBeSucceeded(UpdateServerApplicationCommand dto)
    {
        await _updateHandler.Handle(dto, CancellationToken.None);

        var updated = await _fixture.MemoryMainDbContext.ServerApplications.AsNoTracking().FirstOrDefaultAsync(x => x.Id == dto.Id);

        Assert.NotNull(updated);
        Assert.Equal(dto.Name, updated.Name);
        Assert.Equal(dto.InternalPort, updated.InternalPort.Port);
    }
    [Theory]
    [MemberData(nameof(ServerApplicationCommandsData.SetDataFor_UpdateServerApplicationCommandHandler_WhenIdDoesntExist_ShouldThrowNotFoundException), MemberType = typeof(ServerApplicationCommandsData))]
    public async Task UpdateServerApplicationCommandHandler_WhenIdDoesntExist_ShouldThrowNotFoundException(UpdateServerApplicationCommand dto)
    {
        await Assert.ThrowsAsync<NotFoundException>(() => _updateHandler.Handle(dto, CancellationToken.None));
    }
    #endregion
     
    #region UpdateStateServerApplicationCommandHandler
     
    [Theory]
    [InlineData(2, true)]
    [InlineData(2, false)]
    public async Task UpdateStateServerApplicationCommandHandler_WhenEveryThingIsOk_ShouldBeSucceeded(int id, bool IsActive)
    {
        await _updateStateHandler.Handle(new UpdateStateServerApplicationCommand { Id = id, IsActive = IsActive }, CancellationToken.None);

        var updated = await _fixture.MemoryMainDbContext.ServerApplications.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

        Assert.NotNull(updated);
        Assert.Equal(id, updated.Id);
        Assert.Equal(IsActive ? RowStateEnum.Active : RowStateEnum.DeActive, updated.State);
    }


    [Theory]
    [InlineData(999, true)]
    [InlineData(999, false)]
    public async Task UpdateStateServerApplicationCommandHandler_WhenIdDoesntExist_ShouldThrowNotFoundException(int id, bool IsActive)
    {
        await Assert.ThrowsAsync<NotFoundException>(() => _updateStateHandler.Handle(new UpdateStateServerApplicationCommand { Id = id, IsActive = IsActive }, CancellationToken.None));
    }
    #endregion


}
