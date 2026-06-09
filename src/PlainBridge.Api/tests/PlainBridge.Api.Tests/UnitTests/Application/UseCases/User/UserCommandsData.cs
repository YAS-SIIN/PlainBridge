using PlainBridge.Api.Application.Features.User.Commands;
using PlainBridge.Api.Infrastructure.DTOs;

namespace PlainBridge.Api.Tests.UnitTests.Application.UseCases.User;

 
public static class UserCommandsData
{
    #region CreateUserCommandHandler
		
    public static IEnumerable<object[]> SetDataFor_CreateUserCommandHandler_WhenEveryThingIsOk_ShouldBeSucceeded()
    {
        yield return new object[]
        {
            new CreateUserCommand
            {
                UserName = "new.user1",
                Password = "Pass@12345",
                Email = "new.user1@test.local",
                Name = "New",
                Family = "UserOne",
                PhoneNumber = "+10000000001"
            }
        };
    }

    public static IEnumerable<object[]> SetDataFor_CreateUserCommandHandler_WhenUserExists_ShouldThrowDuplicatedException()
    {
        yield return new object[]
        {
            new CreateUserCommand
            {
                // Assume this username already exists in seeded data
                UserName = "TestUser1",
                Password = "Pass@12345",
                Email = "admin.dup@test.local",
                Name = "Admin",
                Family = "Dup"
            }
        };
    }

    public static IEnumerable<object[]> SetDataFor_CreateUserCommandHandler_WhenIdentityFails_ShouldThrowApplicationException()
    {
        yield return new object[]
        {
            new CreateUserCommand
            {
                UserName = "identity.fail",
                Password = "Pass@12345",
                Email = "identity.fail@test.local",
                Name = "Ident",
                Family = "Fail"
            }
        };
    }
    #endregion

    #region CreateLocallyAsync
    public static IEnumerable<object[]> SetDataFor_CreateUserLocallyCommandHandler_WhenEveryThingIsOk_ShouldBeSucceeded()
    {
        yield return new object[] { new CreateUserLocallyCommand
        {
            UserName = "NewUserLocally",
            Email = "newuserLocally@PlainBridge.com",
            PhoneNumber = "+989120000000",
            Name = "New",
            Family = "User",
            Password = "Password123!",
            RePassword = "Password123!",
            ExternalId = "1"
        }};
    }

    public static IEnumerable<object[]> SetDataFor_CreateUserLocallyCommandHandler_WhenUserExists_ShouldThrowDuplicatedException()
    {
        yield return new object[] { new CreateUserLocallyCommand
        {
            UserName = "TestUser1",
            Email = "TestUser1@PlainBridge.com",
            PhoneNumber = "+989120000000",
            Name = "New",
            Family = "User",
            Password = "Password123!",
            RePassword = "Password123!",
            ExternalId = "1"
        }};
    }

    #endregion
    public static IEnumerable<object[]> SetDataFor_UpdateUserCommandHandler_WhenEveryThingIsOk_ShouldBeSucceeded()
    {
        yield return new object[]
        {
            new UpdateUserCommand
            {
                Id = 1, // existing seeded user id
                Name = "UpdatedName",
                Family = "UpdatedFamily"
            }
        };
    }

    public static IEnumerable<object[]> SetDataFor_UpdateUserCommandHandler_WhenUserDoesNotExist_ShouldThrowNotFoundException()
    {
        yield return new object[]
        {
            new UpdateUserCommand
            {
                Id = 999999,
                Name = "Ghost",
                Family = "User"
            }
        };
    }

    public static IEnumerable<object[]> SetDataFor_UpdateUserCommandHandler_WhenIdentityFails_ShouldThrow()
    {
        yield return new object[]
        {
            new UpdateUserCommand
            {
                Id = 1,
                Name = "NameFail",
                Family = "FamilyFail"
            }
        };
    }

    public static IEnumerable<object[]> SetDataFor_ChangeUserPasswordCommandHandler_WhenEveryThingIsOk_ShouldBeSucceeded()
    {
        yield return new object[]
        {
            new ChangeUserPasswordCommand
            {
                Id = 1,
                CurrentPassword = "OldPass@123",
                NewPassword = "NewPass@123"
            }
        };
    }

    public static IEnumerable<object[]> SetDataFor_ChangeUserPasswordCommandHandler_WhenUserDoesNotExist_ShouldThrowNotFoundException()
    {
        yield return new object[]
        {
            new ChangeUserPasswordCommand
            {
                Id = 999999,
                CurrentPassword = "Whatever1!",
                NewPassword = "Irrelevant2!"
            }
        };
    }

    public static IEnumerable<object[]> SetDataFor_ChangeUserPasswordCommandHandler_WhenIdentityFails_ShouldThrowApplicationException()
    {
        yield return new object[]
        {
            new ChangeUserPasswordCommand
            {
                Id = 1,
                CurrentPassword = "OldPass@123",
                NewPassword = "NewPass@123"
            }
        };
    }

  
}