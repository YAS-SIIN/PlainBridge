using PlainBridge.Api.Domain.HostAggregate.ValueObjects;
using Xunit;

namespace PlainBridge.Api.Tests.UnitTests.Domain.HostAggregate.ValueObjects;

[Collection("ApiUnitTestRun")]
public class InternalUrlTests
{
    [Fact]
    public void Create_WhenValueIsValid_ShouldSucceed()
    {
        var url = InternalUrl.Create("http://internal.local");
        Assert.Equal("http://internal.local", url.InternalUrlValue);
    }

    [Theory]
    [InlineData(" ")]
    [InlineData("")]
    public void Create_WhenValueIsNullOrWhitespace_ShouldThrowArgumentException(string internalUrl)
    {
        Assert.Throws<ArgumentException>(() => InternalUrl.Create(internalUrl)); 
    }

    [Fact]
    public void Create_WhenValueIsTooLong_ShouldThrowApplicationException()
    {
        var tooLong = new string('a', 201);
        var ex = Assert.Throws<ApplicationException>(() => InternalUrl.Create(tooLong));
        Assert.Equal("Domain must be 200 characters or fewer.", ex.Message);
    }

    [Fact]
    public void Equality_WhenValuesMatch_ShouldBeEqual()
    {
        var a = InternalUrl.Create("http://intranet");
        var b = InternalUrl.Create("http://intranet");

        Assert.True(a.Equals(b));
        Assert.True(a == b);
        Assert.False(a != b);
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void Equality_WhenValuesDiffer_ShouldNotBeEqual()
    {
        var a = InternalUrl.Create("http://a");
        var b = InternalUrl.Create("http://b");

        Assert.False(a.Equals(b));
        Assert.False(a == b);
        Assert.True(a != b);
    }
}

