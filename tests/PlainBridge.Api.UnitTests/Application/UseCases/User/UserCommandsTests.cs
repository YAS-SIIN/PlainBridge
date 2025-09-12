using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using PlainBridge.SharedApplication.DTOs;
using PlainBridge.SharedApplication.Enums;
using PlainBridge.SharedApplication.Exceptions;
using PlainBridge.Api.UnitTests.Application.Services;
using Xunit;
using PlainBridge.Api.Application.Features.User.Commands;
using PlainBridge.Api.Infrastructure.ExternalServices.Identity;
using PlainBridge.Api.Infrastructure.DTOs;

namespace PlainBridge.Api.UnitTests.Application.UseCases.User;

[Collection("ApiUnitTestRun")]
public class UserCommandsTests : IClassFixture<TestRunFixture>
{
    private readonly TestRunFixture _fixture;
    private readonly Mock<IIdentityService> _mockIdentityService;
    private readonly CreateUserCommandHandler _createHandler;
    private readonly CreateUserLocallyCommandHandler _createLocallyHandler;
    private readonly UpdateUserCommandHandler _updateHandler;
    private readonly ChangeUserPasswordCommandHandler _changePasswordHandler;

    public UserCommandsTests(TestRunFixture fixture)
    {
        _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
        _mockIdentityService = new Mock<IIdentityService>();

        _createHandler = new CreateUserCommandHandler(
            new Mock<ILogger<CreateUserCommandHandler>>().Object,
            _fixture.MemoryMainDbContext,
            _mockIdentityService.Object);

        _createLocallyHandler = new CreateUserLocallyCommandHandler(
            new Mock<ILogger<CreateUserLocallyCommandHandler>>().Object,
            _fixture.MemoryMainDbContext);

        _updateHandler = new UpdateUserCommandHandler(
            new Mock<ILogger<UpdateUserCommandHandler>>().Object,
            _fixture.MemoryMainDbContext,
            _mockIdentityService.Object);

        _changePasswordHandler = new ChangeUserPasswordCommandHandler(
            new Mock<ILogger<ChangeUserPasswordCommandHandler>>().Object,
            _fixture.MemoryMainDbContext,
            _mockIdentityService.Object);
    }

    #region CreateUserCommandHandler
    [Theory]
    [MemberData(nameof(UserCommandsData.SetDataFor_CreateUserCommandHandler_WhenEveryThingIsOk_ShouldBeSucceeded), MemberType = typeof(UserCommandsData))]
    public async Task CreateUserCommandHandler_WhenEveryThingIsOk_ShouldBeSucceeded(CreateUserCommand cmd)
    {
        string externalId = Guid.NewGuid().ToString();
        _mockIdentityService.Setup(x => x.CreateUserAsync(It.IsAny<UserRequest>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync(new ResultDto<string> { Data = externalId, ResultCode = ResultCodeEnum.Success });

        var appId = await _createHandler.Handle(cmd, CancellationToken.None);
        Assert.NotEqual(Guid.Empty, appId);

        var created = await _fixture.MemoryMainDbContext.Users.FirstOrDefaultAsync(x => x.Username == cmd.Username);

        _mockIdentityService.Verify(x => x.CreateUserAsync(It.IsAny<UserRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        Assert.NotNull(created);
        Assert.Equal(externalId, created.ExternalId);
        Assert.Equal(cmd.Username, created.Username);
        Assert.Equal(cmd.Email, created.Email);
        Assert.Equal(cmd.Name, created.Name);
        Assert.Equal(cmd.Family, created.Family);
    }

    [Theory]
    [MemberData(nameof(UserCommandsData.SetDataFor_CreateUserCommandHandler_WhenUserExists_ShouldThrowDuplicatedException), MemberType = typeof(UserCommandsData))]
    public async Task CreateUserCommandHandler_WhenUserExists_ShouldThrowDuplicatedException(CreateUserCommand cmd)
    {
        await Assert.ThrowsAsync<DuplicatedException>(() => _createHandler.Handle(cmd, CancellationToken.None));
    }

    [Theory]
    [MemberData(nameof(UserCommandsData.SetDataFor_CreateUserCommandHandler_WhenIdentityFails_ShouldThrowApplicationException), MemberType = typeof(UserCommandsData))]
    public async Task CreateUserCommandHandler_WhenIdentityFails_ShouldThrow(CreateUserCommand cmd)
    {
        _mockIdentityService
            .Setup(x => x.CreateUserAsync(It.IsAny<UserRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ResultDto<string> { Data = null, ResultCode = ResultCodeEnum.Error });

        await Assert.ThrowsAsync<ApplicationException>(() => _createHandler.Handle(cmd, CancellationToken.None));
        _mockIdentityService.Verify(x => x.CreateUserAsync(It.IsAny<UserRequest>(), It.IsAny<CancellationToken>()), Times.Once);
    }
    #endregion

    #region CreateUserLocallyCommandHandler

    [Theory]
    [MemberData(nameof(UserCommandsData.SetDataFor_CreateUserLocallyCommandHandler_WhenEveryThingIsOk_ShouldBeSucceeded), MemberType = typeof(UserCommandsData))]
    public async Task CreateUserLocallyCommandHandler_WhenEveryThingIsOk_ShouldBeSucceeded(CreateUserLocallyCommand cmd)
    {
        var appId = await _createLocallyHandler.Handle(cmd, CancellationToken.None);

        Assert.NotEqual(Guid.Empty, appId);

        var created = await _fixture.MemoryMainDbContext.Users.FirstOrDefaultAsync(x => x.Username == cmd.Username);
        Assert.NotNull(created);
        Assert.Equal(cmd.Username, created.Username);
        Assert.Equal(cmd.Email, created.Email);
        Assert.Equal(cmd.Name, created.Name);
        Assert.Equal(cmd.Family, created.Family);
    }

    [Theory]
    [MemberData(nameof(UserCommandsData.SetDataFor_CreateUserLocallyCommandHandler_WhenUserExists_ShouldThrowDuplicatedException), MemberType = typeof(UserCommandsData))]
    public async Task CreateUserLocallyCommandHandler_WhenUserExists_ShouldThrowDuplicatedException(CreateUserLocallyCommand cmd)
    {
        await Assert.ThrowsAsync<DuplicatedException>(() => _createLocallyHandler.Handle(cmd, CancellationToken.None));
    }
    #endregion


    #region UpdateUserCommandHandler
    [Theory]
    [MemberData(nameof(UserCommandsData.SetDataFor_UpdateUserCommandHandler_WhenEveryThingIsOk_ShouldBeSucceeded), MemberType = typeof(UserCommandsData))]
    public async Task UpdateUserCommandHandler_WhenEveryThingIsOk_ShouldBeSucceeded(UpdateUserCommand cmd)
    {
        string externalId = Guid.NewGuid().ToString();
        _mockIdentityService.Setup(x => x.UpdateUserAsync(It.IsAny<UserRequest>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync(new ResultDto<string> { Data = externalId, ResultCode = ResultCodeEnum.Success });

        await _updateHandler.Handle(cmd, CancellationToken.None);

        var updated = await _fixture.MemoryMainDbContext.Users.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == cmd.Id);

        Assert.NotNull(updated);
        Assert.Equal(externalId, updated.ExternalId);
        Assert.Equal(cmd.Name, updated.Name);
        Assert.Equal(cmd.Family, updated.Family);
    }

    [Theory]
    [MemberData(nameof(UserCommandsData.SetDataFor_UpdateUserCommandHandler_WhenUserDoesNotExist_ShouldThrowNotFoundException), MemberType = typeof(UserCommandsData))]
    public async Task UpdateUserCommandHandler_WhenUserDoesNotExist_ShouldThrowNotFoundException(UpdateUserCommand cmd)
    {
        await Assert.ThrowsAsync<NotFoundException>(() => _updateHandler.Handle(cmd, CancellationToken.None));
    }

    [Theory]
    [MemberData(nameof(UserCommandsData.SetDataFor_UpdateUserCommandHandler_WhenIdentityFails_ShouldThrow), MemberType = typeof(UserCommandsData))]
    public async Task UpdateUserCommandHandler_WhenIdentityFails_ShouldThrow(UpdateUserCommand cmd)
    {
        _mockIdentityService
            .Setup(x => x.UpdateUserAsync(It.IsAny<UserRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ResultDto<string> { ResultCode = ResultCodeEnum.Error });

        await Assert.ThrowsAsync<ApplicationException>(() => _updateHandler.Handle(cmd, CancellationToken.None));
    }
    #endregion


    #region ChangeUserPasswordCommandHandler
    [Theory]
    [MemberData(nameof(UserCommandsData.SetDataFor_ChangeUserPasswordCommandHandler_WhenEveryThingIsOk_ShouldBeSucceeded), MemberType = typeof(UserCommandsData))]
    public async Task ChangeUserPasswordCommandHandler_WhenEveryThingIsOk_ShouldBeSucceeded(ChangeUserPasswordCommand cmd)
    {
        _mockIdentityService
            .Setup(x => x.ChangePasswordAsync(It.IsAny<ChangeUserPasswordRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ResultDto<string> { ResultCode = ResultCodeEnum.Success });

        await _changePasswordHandler.Handle(cmd, CancellationToken.None);

        _mockIdentityService.Verify(x => x.ChangePasswordAsync(It.IsAny<ChangeUserPasswordRequest>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [MemberData(nameof(UserCommandsData.SetDataFor_ChangeUserPasswordCommandHandler_WhenUserDoesNotExist_ShouldThrowNotFoundException), MemberType = typeof(UserCommandsData))]
    public async Task ChangeUserPasswordCommandHandler_WhenUserDoesNotExist_ShouldThrowNotFoundException(ChangeUserPasswordCommand cmd)
    {
        await Assert.ThrowsAsync<NotFoundException>(() => _changePasswordHandler.Handle(cmd, CancellationToken.None));
    }

    [Theory]
    [MemberData(nameof(UserCommandsData.SetDataFor_ChangeUserPasswordCommandHandler_WhenIdentityFails_ShouldThrowApplicationException), MemberType = typeof(UserCommandsData))]
    public async Task ChangeUserPasswordCommandHandler_WhenIdentityFails_ShouldThrowApplicationException(ChangeUserPasswordCommand cmd)
    {
        _mockIdentityService
            .Setup(x => x.ChangePasswordAsync(It.IsAny<ChangeUserPasswordRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ResultDto<string> { ResultCode = ResultCodeEnum.Error });

        await Assert.ThrowsAsync<ApplicationException>(() => _changePasswordHandler.Handle(cmd, CancellationToken.None));
    }
    #endregion
}