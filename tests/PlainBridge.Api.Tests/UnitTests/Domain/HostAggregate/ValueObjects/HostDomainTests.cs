using PlainBridge.Api.Domain.HostAggregate.ValueObjects;
using Xunit;

namespace PlainBridge.Api.Tests.UnitTests.Domain.HostAggregate.ValueObjects;

[Collection("ApiUnitTestRun")]
public class HostDomainTests
{
    [Fact]
    public void Create_WhenValueIsValid_ShouldSucceed()
    {
        var host = HostDomain.Create("example-host");
        Assert.Equal("example-host", host.HostDomainName);
    }

    [Theory]
    [InlineData(" ")]
    [InlineData("")]
    public void Create_WhenValueIsNullOrWhitespace_ShouldThrowArgumentException(string hostDomain)
    {
        Assert.Throws<ArgumentException>(() => HostDomain.Create(hostDomain));
        Assert.Throws<ArgumentException>(() => HostDomain.Create(hostDomain));
    }

    [Fact]
    public void Create_WhenValueIsTooLong_ShouldThrowApplicationException()
    {
        var tooLong = new string('a', 201);
        var ex = Assert.Throws<ApplicationException>(() => HostDomain.Create(tooLong));
        Assert.Equal("InternalUrl must be 200 characters or fewer.", ex.Message);
    }

    [Fact]
    public void Equality_WhenValuesMatch_ShouldBeEqual()
    {
        var a = HostDomain.Create("example");
        var b = HostDomain.Create("example");

        Assert.True(a.Equals(b));
        Assert.True(a == b);
        Assert.False(a != b);
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void Equality_WhenValuesDiffer_ShouldNotBeEqual()
    {
        var a = HostDomain.Create("example");
        var b = HostDomain.Create("another");

        Assert.False(a.Equals(b));
        Assert.False(a == b);
        Assert.True(a != b);
    }
}

