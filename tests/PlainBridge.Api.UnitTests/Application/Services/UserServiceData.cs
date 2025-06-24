
using PlainBridge.Api.Application.DTOs;

namespace PlainBridge.Api.UnitTests.Application.Services;

public class UserServiceData
{
    #region CreateAsync
    public static IEnumerable<object[]> SetDataFor_WhenEveryThingIsOk_ShouldBeSucceeded()
    {
        yield return new object[] { new UserDto
        {
            Username = "NewUser",
            Email = "newuser@PlainBridge.com",
            PhoneNumber = "09120000000",
            Name = "New",
            Family = "User",
            Password = "Password123!",
            RePassword = "Password123!"
        }};
    }

    public static IEnumerable<object[]> SetDataFor_CreateAsync_WhenUserExists_ShouldThrowException()
    {
        yield return new object[] { new UserDto
        {
            Username = "TestUser1",
            Email = "TestUser1@PlainBridge.com",
            PhoneNumber = "09120000000",
            Name = "New",
            Family = "User",
            Password = "Password123!",
            RePassword = "Password123!"
        }};
    }

    public static IEnumerable<object[]> SetDataFor_CreateAsync_WhenIdentityFails_ShouldThrowException()
    {
        yield return new object[] { new UserDto
        {
            Username = "NewUser",
            Email = "newuser@PlainBridge.com",
            PhoneNumber = "09120000000",
            Name = "New",
            Family = "User",
            Password = "Password123!",
            RePassword = "Password123!"
        }};
    }
    #endregion
    
    
    #region ChangePasswordAsync

    public static IEnumerable<object[]> SetDataFor_ChangePasswordAsync_WhenEveryThingIsOk_ShouldBeSucceeded()
    {
        yield return new object[] { new ChangeUserPasswordDto
        {
            Id = 1,
            CurrentPassword = "oldpass",
            NewPassword = "newpass"
        }};
    }
    
    public static IEnumerable<object[]> SetDataFor_ChangePasswordAsync_WhenUserDoesNotExist_ShouldThrowException()
    {
        yield return new object[] { new ChangeUserPasswordDto
        {
            Id = 9999,
            CurrentPassword = "oldpass",
            NewPassword = "newpass"
        }};
    }
    
    public static IEnumerable<object[]> SetDataFor_ChangePasswordAsync_WhenIdentityFails_ShouldThrowException()
    {
        yield return new object[] { new ChangeUserPasswordDto
        {
            Id = 1,
            CurrentPassword = "oldpass",
            NewPassword = "newpass"
        }};
    }

    #endregion

    #region UpdateProfileAsync

    public static IEnumerable<object[]> SetDataFor_UpdateProfileAsync_WhenEveryThingIsOk_ShouldBeSucceeded()
    {
        yield return new object[] { new UserDto
        {
            Id = 1,
            Username = "TestUser1",
            Email = "TestUser1@PlainBridge.com",
            PhoneNumber = "09120000000",
            Name = "New",
            Family = "User",
            Password = "Password123!",
            RePassword = "Password123!"
        }};
    }

    public static IEnumerable<object[]> SetDataFor_UpdateProfileAsync_WhenUserDoesNotExist_ShouldThrowException()
    {
        yield return new object[] { new UserDto
        {
            Id = 9999,
            Username = "TestUser1",
            Email = "TestUser1@PlainBridge.com",
            PhoneNumber = "09120000000",
            Name = "New",
            Family = "User",
            Password = "Password123!",
            RePassword = "Password123!"
        }};
    }

    public static IEnumerable<object[]> SetDataFor_UpdateProfileAsync_WhenIdentityFails_ShouldThrowException()
    {
        yield return new object[] { new UserDto
        {
            Id = 1,
            Username = "TestUser1",
            Email = "TestUser1@PlainBridge.com",
            PhoneNumber = "09120000000",
            Name = "New",
            Family = "User",
            Password = "Password123!",
            RePassword = "Password123!"
        }};
    }

    #endregion
}
