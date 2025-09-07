
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using PlainBridge.Api.Application.Services.User;
using PlainBridge.Api.Infrastructure.DTOs;
using PlainBridge.Api.Infrastructure.Identity;
using PlainBridge.SharedApplication.DTOs;
using PlainBridge.SharedApplication.Enums;
using PlainBridge.SharedApplication.Exceptions;

namespace PlainBridge.Api.UnitTests.Application.Services;

[Collection("ApiUnitTestRun")]
public class UserServiceTests : IClassFixture<TestRunFixture>
{
    private readonly TestRunFixture _fixture;
    private readonly IUserService _userService;
    private readonly Mock<ILogger<UserService>> _mockLoggerUserService;
    private readonly Mock<IIdentityService> _mockIdentityService;

    public UserServiceTests(TestRunFixture fixture)
    {
        _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
        _mockLoggerUserService = new Mock<ILogger<UserService>>();
        _mockIdentityService = new Mock<IIdentityService>();
        _userService = new UserService(_mockLoggerUserService.Object, _fixture.MemoryMainDbContext, _mockIdentityService.Object);
    }
    
    #region GetUserByExternalIdAsync

    [Theory]
    [InlineData("1")]
    [InlineData("2")]
    public async Task GetUserByExternalIdAsync_WhenEveryThingIsOk_ShouldReturnData(string externalId)
    {
        var user = await _userService.GetUserByExternalIdAsync(externalId, CancellationToken.None);
        Assert.NotNull(user);
        Assert.Equal(externalId, user.ExternalId);
    }

    [Theory]
    [InlineData("999")]
    public async Task GetUserByExternalIdAsync_WhenIdDoesntExist_ShouldThrowException(string externalId)
    {
        await Assert.ThrowsAsync<NotFoundException>(() => _userService.GetUserByExternalIdAsync(externalId, CancellationToken.None));
    }

    #endregion

    #region GetAllAsync
    [Fact]
    public async Task GetAllAsync_WhenEveryThingIsOk_ShouldReturnData()
    {
        var users = await _userService.GetAllAsync(CancellationToken.None);
        Assert.NotNull(users);
        Assert.True(users.Count >= 3);
    }

    #endregion

    #region CreateAsync

    [Theory]
    [MemberData(nameof(UserServiceData.SetDataFor_CreateAsync_WhenEveryThingIsOk_ShouldBeSucceeded), MemberType = typeof(UserServiceData))]
    public async Task CreateAsync_WhenEveryThingIsOk_ShouldBeSucceeded(UserDto userDto)
    { 
        _mockIdentityService.Setup(x => x.CreateUserAsync(userDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ResultDto<string> { Data = "external-123", ResultCode = ResultCodeEnum.Success });

        var appId = await _userService.CreateAsync(userDto, CancellationToken.None);
        Assert.NotEqual(Guid.Empty, appId);

        var created = await _fixture.MemoryMainDbContext.Users.FirstOrDefaultAsync(x => x.Username == userDto.Username);

        _mockIdentityService.Verify(x => x.CreateUserAsync(userDto, It.IsAny<CancellationToken>()), Times.Once);
        Assert.NotNull(created);
        Assert.Equal(userDto.Username, created.Username); 
        Assert.Equal(userDto.Email, created.Email); 
        Assert.Equal(userDto.Name, created.Name); 
        Assert.Equal(userDto.Family, created.Family); 
    }

    [Theory]
    [MemberData(nameof(UserServiceData.SetDataFor_CreateAsync_WhenUserExists_ShouldThrowException), MemberType = typeof(UserServiceData))]
    public async Task CreateAsync_WhenUserExists_ShouldThrowException(UserDto userDto)
    { 
        await Assert.ThrowsAsync<DuplicatedException>(() => _userService.CreateAsync(userDto, CancellationToken.None));
    }

    [Theory]
    [MemberData(nameof(UserServiceData.SetDataFor_CreateAsync_WhenIdentityFails_ShouldThrowException), MemberType = typeof(UserServiceData))]
    public async Task CreateAsync_WhenIdentityFails_ShouldThrowException(UserDto userDto)
    { 
        _mockIdentityService.Setup(x => x.CreateUserAsync(userDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ResultDto<string> { Data = null, ResultCode = ResultCodeEnum.Error });

        await Assert.ThrowsAsync<ApplicationException>(() => _userService.CreateAsync(userDto, CancellationToken.None));
        _mockIdentityService.Verify(x => x.CreateUserAsync(userDto, It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region CreateLocallyAsync
     
    [Theory]
    [MemberData(nameof(UserServiceData.SetDataFor_CreateLocallyAsync_WhenEveryThingIsOk_ShouldBeSucceeded), MemberType = typeof(UserServiceData))]
    public async Task CreateLocallyAsync_WhenEveryThingIsOk_ShouldBeSucceeded(UserDto userDto)
    { 
        var appId = await _userService.CreateLocallyAsync(userDto, CancellationToken.None);
        Assert.NotEqual(Guid.Empty, appId);

        var created = await _fixture.MemoryMainDbContext.Users.FirstOrDefaultAsync(x => x.Username == userDto.Username);

        Assert.NotNull(created);
        Assert.Equal(userDto.Username, created.Username); 
        Assert.Equal(userDto.Email, created.Email); 
        Assert.Equal(userDto.Name, created.Name); 
        Assert.Equal(userDto.Family, created.Family); 
    }

    [Theory]
    [MemberData(nameof(UserServiceData.SetDataFor_CreateLocallyAsync_WhenUserExists_ShouldThrowException), MemberType = typeof(UserServiceData))]
    public async Task CreateLocallyAsync_WhenUserExists_ShouldThrowException(UserDto userDto)
    { 
        await Assert.ThrowsAsync<DuplicatedException>(() => _userService.CreateLocallyAsync(userDto, CancellationToken.None));
    }

    #endregion

    #region ChangePasswordAsync
     
    [Theory]
    [MemberData(nameof(UserServiceData.SetDataFor_ChangePasswordAsync_WhenEveryThingIsOk_ShouldBeSucceeded), MemberType = typeof(UserServiceData))]
    public async Task ChangePasswordAsync_WhenEveryThingIsOk_ShouldBeSucceeded(ChangeUserPasswordDto changeUserPassword)
    {

        _mockIdentityService.Setup(x => x.ChangePasswordAsync(changeUserPassword, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ResultDto<string> { ResultCode = ResultCodeEnum.Success });

        await _userService.ChangePasswordAsync(changeUserPassword, CancellationToken.None);

        _mockIdentityService.Verify(x => x.ChangePasswordAsync(changeUserPassword, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [MemberData(nameof(UserServiceData.SetDataFor_ChangePasswordAsync_WhenUserDoesNotExist_ShouldThrowException), MemberType = typeof(UserServiceData))]
    public async Task ChangePasswordAsync_WhenUserDoesNotExist_ShouldThrowException(ChangeUserPasswordDto changeUserPassword)
    { 
        await Assert.ThrowsAsync<NotFoundException>(() => _userService.ChangePasswordAsync(changeUserPassword, CancellationToken.None));
    }

    [Theory]
    [MemberData(nameof(UserServiceData.SetDataFor_ChangePasswordAsync_WhenEveryThingIsOk_ShouldBeSucceeded), MemberType = typeof(UserServiceData))]
    public async Task ChangePasswordAsync_WhenIdentityFails_ShouldThrowException(ChangeUserPasswordDto changeUserPassword)
    { 

        _mockIdentityService.Setup(x => x.ChangePasswordAsync(changeUserPassword, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ResultDto<string> { ResultCode = ResultCodeEnum.Error });

        await Assert.ThrowsAsync<ApplicationException>(() => _userService.ChangePasswordAsync(changeUserPassword, CancellationToken.None));
        _mockIdentityService.Verify(x => x.ChangePasswordAsync(changeUserPassword, It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region UpdateProfileAsync
     
    [Theory]
    [MemberData(nameof(UserServiceData.SetDataFor_UpdateProfileAsync_WhenEveryThingIsOk_ShouldBeSucceeded), MemberType = typeof(UserServiceData))]
    public async Task UpdateProfileAsync_WhenEveryThingIsOk_ShouldBeSucceeded(UserDto user)
    {
         

        _mockIdentityService.Setup(x => x.UpdateUserAsync(user, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ResultDto<string> { ResultCode = ResultCodeEnum.Success });

        await _userService.UpdateAsync(user, CancellationToken.None);

        var updated = await _fixture.MemoryMainDbContext.Users.FindAsync(user.Id);
        _mockIdentityService.Verify(x => x.UpdateUserAsync(user, It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal(user.Name, updated.Name);
        Assert.Equal(user.Family, updated.Family);
    }

    [Theory]
    [MemberData(nameof(UserServiceData.SetDataFor_UpdateProfileAsync_WhenUserDoesNotExist_ShouldThrowException), MemberType = typeof(UserServiceData))]
    public async Task UpdateProfileAsync_WhenUserDoesNotExist_ShouldThrowException(UserDto user)
    { 
        await Assert.ThrowsAsync<NotFoundException>(() => _userService.UpdateAsync(user, CancellationToken.None));
    }

    [Theory]
    [MemberData(nameof(UserServiceData.SetDataFor_UpdateProfileAsync_WhenIdentityFails_ShouldThrowException), MemberType = typeof(UserServiceData))]
    public async Task UpdateProfileAsync_WhenIdentityFails_ShouldThrowException(UserDto user)
    { 
        _mockIdentityService.Setup(x => x.UpdateUserAsync(user, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ResultDto<string> { ResultCode = ResultCodeEnum.Error });

        await Assert.ThrowsAsync<ApplicationException>(() => _userService.UpdateAsync(user, CancellationToken.None));
        _mockIdentityService.Verify(x => x.UpdateUserAsync(user, It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion
}
