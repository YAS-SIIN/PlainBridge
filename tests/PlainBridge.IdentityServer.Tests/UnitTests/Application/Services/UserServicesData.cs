
using PlainBridge.IdentityServer.EndPoint.DTOs;

namespace PlainBridge.IdentityServer.Tests.UnitTests.Application.Services;

public static class UserServicesData
{

    #region CreateAsync

    public static IEnumerable<object[]> SetDataFor_CreateAsync_WhenEveryThingIsOk_ShouldBeSucceeded()
    {
        yield return new object[] { new UserRequestDto
        {
            UserName = "newTestUser",
            Email = "newtestuser@plainbridge.com",
            Name = "newTest",
            Family = "newUser",
            Password = "TestPassword123",
            RePassword = "TestPassword123",
            PhoneNumber = "09121112222"
        }};
    }

    public static IEnumerable<object[]> SetDataFor_CreateAsync_WhenCreatingOfUserManagerFails_ShouldThrowApplicationException()
    {
        yield return new object[] { new UserRequestDto
        {
            UserName = "newTestUser",
            Email = "newtestuser@plainbridge.com",
            Name = "newTest",
            Family = "newUser",
            Password = "TestPassword123",
            RePassword = "TestPassword123",
            PhoneNumber = "09121112222"
        }};
    }

    public static IEnumerable<object[]> SetDataFor_CreateAsync_WhenAddingClaimsFails_ShouldThrowApplicationException()
    {
        yield return new object[] { new UserRequestDto
        {
            UserName = "newTestUser",
            Email = "newtestuser@plainbridge.com",
            Name = "newTest",
            Family = "newUser",
            Password = "TestPassword123",
            RePassword = "TestPassword123",
            PhoneNumber = "09121112222"
        }};
    }

    #endregion

    #region UpdateAsync

    public static IEnumerable<object[]> SetDataFor_UpdateAsync_WhenEveryThingIsOk_ShouldBeSucceeded()
    {
        yield return new object[] { new UserRequestDto
        {
            UserId = "1",
            UserName = "newTestUser",
            Email = "newtestuser@plainbridge.com",
            Name = "newTest",
            Family = "newUser",
            Password = "TestPassword123",
            RePassword = "TestPassword123",
            PhoneNumber = "09121112222"
        }};
    }


    public static IEnumerable<object[]> SetDataFor_UpdateAsync_WhenUserIdDoesntExist_ShouldThrowNotFoundException()
    {
        yield return new object[] { new UserRequestDto
        {
            UserId = "0",
            UserName = "newTestUser",
            Email = "newtestuser@plainbridge.com",
            Name = "newTest",
            Family = "newUser",
            Password = "TestPassword123",
            RePassword = "TestPassword123",
            PhoneNumber = "09121112222"
        }};
    }

    #endregion

    #region ChangePasswordAsync

    public static IEnumerable<object[]> SetDataFor_ChangePasswordAsync_WhenEveryThingIsOk_ShouldBeSucceeded()
    {
        yield return new object[] { new ChangeUserPasswordRequestDto
        {
            UserId = "1", 
            CurrentPassword = "TestPassword123",
            NewPassword = "TestPassword123", 
            RePassword = "TestPassword123", 
        }};
    }


    public static IEnumerable<object[]> SetDataFor_ChangePasswordAsync_WhenUserIdDoesntExist_ShouldThrowNotFoundException()
    {
        yield return new object[] { new ChangeUserPasswordRequestDto
        {
            UserId = "0",
            CurrentPassword = "TestPassword123",
            NewPassword = "TestPassword123",
            RePassword = "TestPassword123",
        }};
    }

    public static IEnumerable<object[]> SetDataFor_ChangePasswordAsync_WhenChangePasswordFails_ShouldThrowApplicationException()
    {
        yield return new object[] { new ChangeUserPasswordRequestDto
        {
            UserId = "1",
            CurrentPassword = "TestPassword123",
            NewPassword = "TestPassword123",
            RePassword = "TestPassword123",
        }};
    }


    #endregion
}
