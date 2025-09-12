
using PlainBridge.Api.Application.UseCases.ServerApplication.Commands;
using PlainBridge.SharedApplication.DTOs;

namespace PlainBridge.Api.UnitTests.Application.UseCases.ServerApplication;

public static class ServerApplicationCommandsData
{
    public static IEnumerable<object[]> SetDataFor_CreateServerApplicationCommandHandler_WhenEveryThingIsOk_ShouldBeSucceeded()
    {
        yield return new object[] { new CreateServerApplicationCommand
        {
            Name = "NewApp1",
            InternalPort = 4001,
            UserId = 1,
            ServerApplicationType = SharedApplication.Enums.ServerApplicationTypeEnum.SharePort
        }};
        yield return new object[] { new CreateServerApplicationCommand
        {
            Name = "NewApp2",
            InternalPort = 4002,
            UserId = 1,
            ServerApplicationType = SharedApplication.Enums.ServerApplicationTypeEnum.UsePort,
            ServerApplicationAppId = Guid.NewGuid().ToString()
        }};
    }

    public static IEnumerable<object[]> SetDataFor_CreateServerApplicationCommandHandler_WhenInternalPortIsNotValid_ShouldThrowApplicationException()
    {
        yield return new object[] { new CreateServerApplicationCommand
        {
            Name = "NewApp",
            InternalPort = 99999,
            UserId = 1,
            ServerApplicationType = SharedApplication.Enums.ServerApplicationTypeEnum.SharePort
        }};
    }

    public static IEnumerable<object[]> SetDataFor_CreateServerApplicationCommandHandler_WhenServerApplicationViewIdIsEmpty_ShouldThrowNotFoundException()
    {
        yield return new object[] { new CreateServerApplicationCommand
        {
            Name = "NewApp",
            InternalPort = 4000,
            UserId = 1,
            ServerApplicationType = SharedApplication.Enums.ServerApplicationTypeEnum.UsePort,
        }};
    }

    public static IEnumerable<object[]> SetDataFor_UpdateServerApplicationCommandHandler_WhenEveryThingIsOk_ShouldBeSucceeded()
    {
        yield return new object[] { new UpdateServerApplicationCommand
        {
            Id = 2,
            Name = "NewApp",
            InternalPort = 4000,
            UserId = 2,
            ServerApplicationType = SharedApplication.Enums.ServerApplicationTypeEnum.SharePort,
            ServerApplicationAppId = Guid.NewGuid().ToString()
        }};
    }

    public static IEnumerable<object[]> SetDataFor_UpdateServerApplicationCommandHandler_WhenIdDoesntExist_ShouldThrowNotFoundException()
    {
        yield return new object[] { new UpdateServerApplicationCommand
        {
            Id = 0,
            Name = "NewApp",
            InternalPort = 4000
        }};
    }
     

}
