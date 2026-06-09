using PlainBridge.IdentityServer.EndPoint.DTOs;

namespace PlainBridge.IdentityServer.Tests.IntegrationTests.Endpoints;

public class UserEndpointData
{
    #region CreateUser
    public static IEnumerable<object[]> SetDataFor_CreateUser_WhenEveryThingIsOk_ShouldBeSucceeded()
    {
        yield return new object[] { new UserRequestDto
        {
            UserName = "newTestUser",
            Email = "newtestuser@plainbridge.com",
            Name = "newTest",
            Family = "newUser",
            Password = "TestPassword123",
            PhoneNumber = "09121112222"
        }};
    }

    public static IEnumerable<object[]> SetDataFor_CreateUser_WhenPasswordIsNotValid_ShouldThrowException()
    {
        yield return new object[] { new UserRequestDto
        {
            UserName = "newTestUser",
            Email = "newtestuser@plainbridge.com",
            Name = "newTest",
            Family = "newUser",
            Password = "0",
            PhoneNumber = "09121112222"
        }};
    }

    public static IEnumerable<object[]> SetDataFor_CreateUser_WhenUsernameIsEmpty_ShouldThrowException()
    {
        yield return new object[] { new UserRequestDto
        {
            UserName = "",
            Email = "newtestuser@plainbridge.com",
            Name = "newTest",
            Family = "newUser",
            Password = "TestPassword123",
            PhoneNumber = "09121112222"
        }};
    }

    public static IEnumerable<object[]> SetDataFor_CreateUser_WhenUsernameIsDuplicated_ShouldThrowException()
    {
        yield return new object[] { new UserRequestDto
        {
                 UserName = "TestUser",
                Email = "testuser@plainbridge.com",
                Name = "Test",
                Family = "User",
                Password = "TestPassword123",
                PhoneNumber = "09121112222"
        }};
    }

    #endregion

    #region UpdateUser

    public static IEnumerable<object[]> SetDataFor_UpdateUser_WhenEveryThingIsOk_ShouldBeSucceeded()
    {
        yield return new object[] { new UserRequestDto
        {
            UserId = Guid.Empty.ToString(),
            UserName = "updatedTestUser",
            Email = "updatedtestuser@plainbridge.com",
            Name = "updatedTest",
            Family = "updatedUser",
            Password = "TestPassword123",
            PhoneNumber = "09121112222"
        }};
    }

    public static IEnumerable<object[]> SetDataFor_UpdateUser_WhenUserIdNotExist_ShouldThrowException()
    {
        yield return new object[] { new UserRequestDto
        {
            UserId = Guid.Empty.ToString(), 
            UserName = "updatedTestUser",
            Email = "updatedtestuser@plainbridge.com",
            Name = "updatedTest",
            Family = "updatedUser",
            Password = "TestPassword123",
            PhoneNumber = "09121112222"
        }};
    }

    #endregion


    #region ChangePassword

    public static IEnumerable<object[]> SetDataFor_ChangePassword_WhenEveryThingIsOk_ShouldBeSucceeded()
    {
        yield return new object[] { new ChangeUserPasswordRequestDto
        {
            UserId = Guid.Empty.ToString(), 
            CurrentPassword = "TestPassword123",
            NewPassword = "TestPassword321"
        }};
    }

    public static IEnumerable<object[]> SetDataFor_ChangePassword_WhenUserIdNotExist_ShouldThrowException()
    {
        yield return new object[] { new ChangeUserPasswordRequestDto
        {
            UserId = Guid.Empty.ToString(),
            CurrentPassword = "TestPassword123",
            NewPassword = "TestPassword321"
        }};
    }

    public static IEnumerable<object[]> SetDataFor_ChangePassword_WhenPasswordIsNotValid_ShouldThrowException()
    {
        yield return new object[] { new ChangeUserPasswordRequestDto
        {
            UserId = Guid.Empty.ToString(),
            CurrentPassword = "TestPassword123",
            NewPassword = "0"
        }};
    }

    #endregion



}
