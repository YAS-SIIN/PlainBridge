

using PlainBridge.Api.Application.DTOs;
using PlainBridge.SharedApplication.DTOs;

namespace PlainBridge.Api.UnitTests.Application.Services;

public class ServerApplicationServiceData
{
    public static IEnumerable<object[]> SetDataFor_CreateAsync_WhenEveryThingIsOk_ShouldBeSucceeded()
    {
        yield return new object[] { new ServerApplicationDto
        {
            Name = "NewApp", 
            InternalPort = 4000
        }};
    }  
    
    public static IEnumerable<object[]> SetDataFor_CreateAsync_WhenInternalPortIsNotValid_ShouldThrowException()
    {
        yield return new object[] { new ServerApplicationDto
        {
            Name = "NewApp",
            InternalPort = 99999
        }};
    }  
    
    public static IEnumerable<object[]> SetDataFor_UpdateAsync_WhenEveryThingIsOk_ShouldBeSucceeded()
    {
        yield return new object[] { new ServerApplicationDto
        {
            Id = 2,
            InternalPort = 4000
        }};
    }  
     
    public static IEnumerable<object[]> SetDataFor_UpdateAsync_WhenIdDoesntExist_ShouldThrowException()
    {
        yield return new object[] { new ServerApplicationDto
        {
            Id = 0,
            InternalPort = 4000
        }};
    }  
     
    public static IEnumerable<object[]> SetDataFor_UpdateAsync_WhenIdDoesntExist_ShouldThrowApplicationException()
    {
        yield return new object[] { new ServerApplicationDto
        {
            Id = 999,
            InternalPort = 4000
        }};
    }  



}
