
using PlainBridge.Api.Application.UseCases.ServerApplication.Commands;

namespace PlainBridge.Api.UnitTests.Application.UseCases.ServerApplication;

public class ServerApplicationCommandsData
{
    public static IEnumerable<object[]> SetDataFor_CreateServerApplicationCommandHandler_WhenEveryThingIsOk_ShouldBeSucceeded()
    {
        yield return new object[] { new CreateServerApplicationCommand
        {
            Name = "NewApp",
            UserId = 1,
            Domain = "newdomain",
            InternalUrl = "http://localhost:4000"
        }};
    }

    public static IEnumerable<object[]> SetDataFor_CreateServerApplicationCommandHandler_WhenDomainIsExisted_ShouldThrowException()
    {
        yield return new object[] { new CreateServerApplicationCommand
        {
            Name = "NewApp",
            UserId = 1,
            Domain = "TestDomain1",
            InternalUrl = "http://localhost:4000"
        }};
    }

    public static IEnumerable<object[]> SetDataFor_UpdateServerApplicationCommandHandler_WhenEveryThingIsOk_ShouldBeSucceeded()
    {
        yield return new object[] { new UpdateServerApplicationCommand
        {
            Id = 2,
            Name = "NewApp2",
            Domain = "newdomain2",
            InternalUrl = "http://localhost:4000"
        }};
    }

    public static IEnumerable<object[]> SetDataFor_UpdateServerApplicationCommandHandler_WhenDomainIsExisted_ShouldThrowException()
    {
        yield return new object[] { new UpdateServerApplicationCommand
        {
            Id = 2,
            Name = "NewApp",
            Domain = "TestDomain1",
            InternalUrl = "http://localhost:4000"
        }};
    }

    public static IEnumerable<object[]> SetDataFor_UpdateServerApplicationCommandHandler_WhenIdDoesntExist_ShouldThrowApplicationException()
    {
        yield return new object[] { new UpdateServerApplicationCommand
        {
            Id = 999,
            Name = "NewApp",
            Domain = "newdomain",
            InternalUrl = "http://localhost:4000"
        }};
    }


}
