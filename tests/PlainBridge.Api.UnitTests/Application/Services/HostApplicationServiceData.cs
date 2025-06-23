

using PlainBridge.SharedApplication.DTOs;

namespace PlainBridge.Api.UnitTests.Application.Services;

public class HostApplicationServiceData
{
    public static IEnumerable<object[]> SetDataFor_CreateAsync_WhenEveryThingIsOk_ShouldBeSucceeded()
    {
        yield return new object[] { new HostApplicationDto
        {
            Name = "NewApp",
            UserId = 1,
            Domain = "newdomain",
            InternalUrl = "http://localhost:4000"
        }};
    }  

    public static IEnumerable<object[]> SetDataFor_CreateAsync_WhenDomainIsExisted_ShouldThrowException()
    {
        yield return new object[] { new HostApplicationDto
        {
            Name = "NewApp",
            UserId = 1,
            Domain = "TestDomain1",
            InternalUrl = "http://localhost:4000"
        }};
    }  
    
    public static IEnumerable<object[]> SetDataFor_UpdateAsync_WhenEveryThingIsOk_ShouldBeSucceeded()
    {
        yield return new object[] { new HostApplicationDto
        {
            Id = 2,
            Name = "NewApp2",
            Domain = "newdomain2",
            InternalUrl = "http://localhost:4000"
        }};
    }  
     
    public static IEnumerable<object[]> SetDataFor_UpdateAsync_WhenDomainIsExisted_ShouldThrowException()
    {
        yield return new object[] { new HostApplicationDto
        {
            Id = 2,
            Name = "NewApp",
            Domain = "TestDomain1",
            InternalUrl = "http://localhost:4000"
        }};
    }  
     
    public static IEnumerable<object[]> SetDataFor_UpdateAsync_WhenIdDoesntExist_ShouldThrowApplicationException()
    {
        yield return new object[] { new HostApplicationDto
        {
            Id = 999,
            Name = "NewApp",
            Domain = "newdomain",
            InternalUrl = "http://localhost:4000"
        }};
    }  



}
