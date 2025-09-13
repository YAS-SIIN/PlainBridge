using PlainBridge.Api.Application.UseCases.HostApplication.Commands;

namespace PlainBridge.Api.Tests.UnitTests.Application.UseCases.HostApplication;

public static class HostApplicationCommandsData
{
    public static IEnumerable<object[]> SetDataFor_CreateHostApplicationCommandHandler_WhenEveryThingIsOk_ShouldBeSucceeded()
    {
        yield return new object[] { new CreateHostApplicationCommand
        {
            Name = "NewApp",
            UserId = 1,
            Domain = "newdomain",
            InternalUrl = "http://localhost:4000"
        }};
    }

    public static IEnumerable<object[]> SetDataFor_CreateHostApplicationCommandHandler__WhenDomainIsExisted_ShouldThrowDuplicatedException()
    {
        yield return new object[] { new CreateHostApplicationCommand
        {
            Name = "NewApp",
            UserId = 1,
            Domain = "TestDomain1",
            InternalUrl = "http://localhost:4000"
        }};
    }

    public static IEnumerable<object[]> SetDataFor_UpdateHostApplicationCommandHandler_WhenEveryThingIsOk_ShouldBeSucceeded()
    {
        yield return new object[] { new UpdateHostApplicationCommand
        {
            Id = 2,
            Name = "NewApp2",
            Domain = "newdomain2",
            InternalUrl = "http://localhost:4000"
        }};
    }

    public static IEnumerable<object[]> SetDataFor_UpdateHostApplicationCommandHandler_WhenDomainIsExisted_ShouldThrowApplicationException()
    {
        yield return new object[] { new UpdateHostApplicationCommand
        {
            Id = 2,
            Name = "NewApp",
            Domain = "TestDomain1",
            InternalUrl = "http://localhost:4000"
        }};
    }

    public static IEnumerable<object[]> SetDataFor_UpdateHostApplicationCommandHandler_WhenIdDoesntExist_ShouldThrowApplicationException()
    {
        yield return new object[] { new UpdateHostApplicationCommand
        {
            Id = 999,
            Name = "NewApp",
            Domain = "newdomain",
            InternalUrl = "http://localhost:4000"
        }};
    }


}
