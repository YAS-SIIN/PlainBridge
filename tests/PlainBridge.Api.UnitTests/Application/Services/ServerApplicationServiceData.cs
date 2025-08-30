

using PlainBridge.Api.Application.DTOs;
using PlainBridge.SharedApplication.DTOs;

namespace PlainBridge.Api.UnitTests.Application.Services;

public class ServerApplicationServiceData
{
    public static IEnumerable<object[]> SetDataFor_CreateAsync_WhenEveryThingIsOk_ShouldBeSucceeded()
    {
        yield return new object[] { new ServerApplicationDto
        {
            Name = "NewApp1", 
            InternalPort = 4001,
            UserId = 1,
            ServerApplicationType = SharedApplication.Enums.ServerApplicationTypeEnum.SharePort,
            ServerApplicationAppId = Guid.NewGuid().ToString()
        }};
        yield return new object[] { new ServerApplicationDto
        {
            Name = "NewApp2",
            InternalPort = 4002,
            UserId = 1,
            ServerApplicationType = SharedApplication.Enums.ServerApplicationTypeEnum.UsePort,
            ServerApplicationAppId = Guid.NewGuid().ToString()
        }};
    }  
    
    public static IEnumerable<object[]> SetDataFor_CreateAsync_WhenInternalPortIsNotValid_ShouldThrowException()
    {
        yield return new object[] { new ServerApplicationDto
        {
            Name = "NewApp",
            InternalPort = 99999,
            UserId = 1,
            ServerApplicationType = SharedApplication.Enums.ServerApplicationTypeEnum.SharePort,
            ServerApplicationAppId = Guid.NewGuid().ToString()
        }};
    }  
    
    public static IEnumerable<object[]> SetDataFor_CreateAsync_WhenServerApplicationViewIdIsEmpty_ShouldThrowException()
    {
        yield return new object[] { new ServerApplicationDto
        {
            Name = "NewApp",
            InternalPort = 4000,
            UserId = 1,
            ServerApplicationType = SharedApplication.Enums.ServerApplicationTypeEnum.UsePort,
        }};
    }  
    
    public static IEnumerable<object[]> SetDataFor_UpdateAsync_WhenEveryThingIsOk_ShouldBeSucceeded()
    {
        yield return new object[] { new ServerApplicationDto
        {
            Id = 2,
            Name = "NewApp",
            InternalPort = 4000,
            UserId = 1,
            ServerApplicationType = SharedApplication.Enums.ServerApplicationTypeEnum.SharePort,
            ServerApplicationAppId = Guid.NewGuid().ToString()
        }};
    }  
     
    public static IEnumerable<object[]> SetDataFor_UpdateAsync_WhenIdDoesntExist_ShouldThrowException()
    {
        yield return new object[] { new ServerApplicationDto
        {
            Id = 0,
            Name = "NewApp",
            InternalPort = 4000
        }};
    }  
     
    public static IEnumerable<object[]> SetDataFor_UpdateAsync_WhenIdDoesntExist_ShouldThrowApplicationException()
    {
        yield return new object[] { new ServerApplicationDto
        {
            Id = 999,
            Name = "NewApp",
            InternalPort = 4000
        }};
    }  
     


}
