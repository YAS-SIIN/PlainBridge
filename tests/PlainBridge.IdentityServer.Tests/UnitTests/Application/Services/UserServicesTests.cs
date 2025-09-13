using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using PlainBridge.Api.Application.DTOs;
using PlainBridge.IdentityServer.EndPoint.Application.Services.User;
using PlainBridge.IdentityServer.EndPoint.Domain.Entities;
using PlainBridge.IdentityServer.EndPoint.DTOs;
using PlainBridge.SharedApplication.Exceptions;

namespace PlainBridge.IdentityServer.Tests.UnitTests.Application.Services;

public class UserServicesTests
{
    public readonly IUserServices _userServices;
    public readonly Mock<UserManager<ApplicationUser>> _userManager;
    public UserServicesTests()
    {
        var store = new Mock<IUserStore<ApplicationUser>>();
        _userManager = new Mock<UserManager<ApplicationUser>>(
            store.Object,
            null, null, null, null, null, null, null, null
        );

        _userServices = new UserServices(new Mock<ILogger<UserServices>>().Object, _userManager.Object);
    }

    #region CreateAsync

    [Theory]
    [MemberData(nameof(UserServicesData.SetDataFor_CreateAsync_WhenEveryThingIsOk_ShouldBeSucceeded), MemberType = typeof(UserServicesData))]
    public async Task CreateAsync_WhenEveryThingIsOk_ShouldBeSucceeded(UserRequestDto userDto)
    {
        _userManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
        _userManager.Setup(x => x.AddClaimsAsync(It.IsAny<ApplicationUser>(), It.IsAny<IEnumerable<System.Security.Claims.Claim>>()))
            .ReturnsAsync(IdentityResult.Success);


        var res = await _userServices.CreateAsync(userDto);

        Assert.NotNull(res);
        Assert.Equal(userDto.Email, res.Email);
        Assert.Equal(userDto.UserName, res.UserName);


        _userManager.Verify(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Once);
        _userManager.Verify(x => x.AddClaimsAsync(It.IsAny<ApplicationUser>(), It.IsAny<IEnumerable<System.Security.Claims.Claim>>()), Times.Once);
    }


    [Theory]
    [MemberData(nameof(UserServicesData.SetDataFor_CreateAsync_WhenCreatingOfUserManagerFails_ShouldThrowApplicationException), MemberType = typeof(UserServicesData))]
    public async Task CreateAsync_WhenCreatingOfUserManagerFails_ShouldThrowApplicationException(UserRequestDto userDto)
    {
        _userManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed());

        var res = await Assert.ThrowsAsync<ApplicationException>(async () => await _userServices.CreateAsync(userDto));

        _userManager.Verify(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Once);
    }


    [Theory]
    [MemberData(nameof(UserServicesData.SetDataFor_CreateAsync_WhenAddingClaimsFails_ShouldThrowApplicationException), MemberType = typeof(UserServicesData))]
    public async Task CreateAsync_WhenAddingClaimsFails_ShouldThrowApplicationException(UserRequestDto userDto)
    {
        _userManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
        _userManager.Setup(x => x.AddClaimsAsync(It.IsAny<ApplicationUser>(), It.IsAny<IEnumerable<System.Security.Claims.Claim>>()))
            .ReturnsAsync(IdentityResult.Failed());

        var res = await Assert.ThrowsAsync<ApplicationException>(async () => await _userServices.CreateAsync(userDto));

        _userManager.Verify(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Once);
        _userManager.Verify(x => x.AddClaimsAsync(It.IsAny<ApplicationUser>(), It.IsAny<IEnumerable<System.Security.Claims.Claim>>()), Times.Once);
    }

    #endregion

    #region UpdateAsync
    [Theory]
    [MemberData(nameof(UserServicesData.SetDataFor_UpdateAsync_WhenEveryThingIsOk_ShouldBeSucceeded), MemberType = typeof(UserServicesData))]
    public async Task UpdateAsync_WhenEveryThingIsOk_ShouldBeSucceeded(UserRequestDto userDto)
    {
        var user = new ApplicationUser
        {
            Id = userDto.UserId,
            UserName = userDto.UserName,
            Email = userDto.Email,
            PhoneNumber = userDto.PhoneNumber
        };
        var claims = new List<System.Security.Claims.Claim>
        {
            new System.Security.Claims.Claim(JwtClaimTypes.GivenName, userDto.Name),
            new System.Security.Claims.Claim(JwtClaimTypes.FamilyName, userDto.Family),
            new System.Security.Claims.Claim(JwtClaimTypes.PhoneNumber, userDto.PhoneNumber ?? string.Empty),
            new System.Security.Claims.Claim(JwtClaimTypes.Name, $"{userDto.Name} {userDto.Family}"),
            new System.Security.Claims.Claim(JwtClaimTypes.Email, $"{userDto.Email}"),
            new System.Security.Claims.Claim(JwtClaimTypes.PreferredUserName, $"{userDto.UserName}"),
        };
        _userManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(user);
        _userManager.Setup(x => x.GetClaimsAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(claims);
        _userManager.Setup(x => x.ReplaceClaimAsync(It.IsAny<ApplicationUser>(), It.IsAny<System.Security.Claims.Claim>(), It.IsAny<System.Security.Claims.Claim>()))
            .ReturnsAsync(IdentityResult.Success);

        var res = await _userServices.UpdateAsync(userDto);

        Assert.NotNull(res);
        Assert.Equal(userDto.Email, res.Email);
        Assert.Equal(userDto.UserName, res.UserName);

        _userManager.Verify(x => x.FindByIdAsync(It.IsAny<string>()), Times.Once);
        _userManager.Verify(x => x.GetClaimsAsync(It.IsAny<ApplicationUser>()), Times.Once);
        _userManager.Verify(x => x.ReplaceClaimAsync(It.IsAny<ApplicationUser>(), It.IsAny<System.Security.Claims.Claim>(), It.IsAny<System.Security.Claims.Claim>()), Times.AtLeast(1));
    }

    [Theory]
    [MemberData(nameof(UserServicesData.SetDataFor_UpdateAsync_WhenUserIdDoesntExist_ShouldThrowNotFoundException), MemberType = typeof(UserServicesData))]
    public async Task UpdateAsync_WhenUserIdDoesntExist_ShouldThrowNotFoundException(UserRequestDto userDto)
    {

        _userManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync((ApplicationUser?)null);

        var res = await Assert.ThrowsAsync<NotFoundException>(async () => await _userServices.UpdateAsync(userDto));

        _userManager.Verify(x => x.FindByIdAsync(It.IsAny<string>()), Times.Once);
    }

    #endregion
     
    #region ChangePasswordAsync

    [Theory]
    [MemberData(nameof(UserServicesData.SetDataFor_ChangePasswordAsync_WhenEveryThingIsOk_ShouldBeSucceeded), MemberType = typeof(UserServicesData))]
    public async Task ChangePasswordAsync_WhenEveryThingIsOk_ShouldBeSucceeded(ChangeUserPasswordRequestDto changeUserPasswordRequest)
    {
        var user = new ApplicationUser
        {
            Id = changeUserPasswordRequest.UserId,
            UserName = "testuser",
            Email = "testuser@test.com"
        };
        _userManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);

        _userManager.Setup(x => x.ChangePasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

        var res = await _userServices.ChangePasswordAsync(changeUserPasswordRequest);

        Assert.NotNull(res);
        _userManager.Verify(x => x.FindByIdAsync(It.IsAny<string>()), Times.Once);
        _userManager.Verify(x => x.ChangePasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);

    }

    [Theory]
    [MemberData(nameof(UserServicesData.SetDataFor_ChangePasswordAsync_WhenUserIdDoesntExist_ShouldThrowNotFoundException), MemberType = typeof(UserServicesData))]
    public async Task ChangePasswordAsync_WhenUserIdDoesntExist_ShouldThrowNotFoundException(ChangeUserPasswordRequestDto changeUserPasswordRequest)
    {
        _userManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
             .ReturnsAsync((ApplicationUser?)null);

        var res = await Assert.ThrowsAsync<NotFoundException>(async () => await _userServices.ChangePasswordAsync(changeUserPasswordRequest));

        _userManager.Verify(x => x.FindByIdAsync(It.IsAny<string>()), Times.Once);

    }

    [Theory]
    [MemberData(nameof(UserServicesData.SetDataFor_ChangePasswordAsync_WhenChangePasswordFails_ShouldThrowApplicationException), MemberType = typeof(UserServicesData))]
    public async Task ChangePasswordAsync_WhenChangePasswordFails_ShouldThrowApplicationException(ChangeUserPasswordRequestDto changeUserPasswordRequest)
    {
        var user = new ApplicationUser
        {
            Id = changeUserPasswordRequest.UserId,
            UserName = "testuser",
            Email = "testuser@test.com"
        };
        _userManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);

        _userManager.Setup(x => x.ChangePasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Failed());

        var res = await Assert.ThrowsAsync<ApplicationException>(async () => await _userServices.ChangePasswordAsync(changeUserPasswordRequest));

        _userManager.Verify(x => x.FindByIdAsync(It.IsAny<string>()), Times.Once);
        _userManager.Verify(x => x.ChangePasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);

    }

    #endregion


}
