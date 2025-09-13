using PlainBridge.Api.Domain.ServerAggregate.ValueObjects;
using Xunit;

namespace PlainBridge.Api.Tests.UnitTests.Domain.ServerAggregate.ValueObjects;

[Collection("ApiUnitTestRun")]
public class InternalPortTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(8080)]
    [InlineData(65535)]
    public void Create_WhenPortInRange_ShouldSucceed(int port)
    {
        var p = InternalPort.Create(port);
        Assert.Equal(port, p.Port);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(65536)]
    public void Create_WhenPortOutOfRange_ShouldThrowApplicationException(int port)
    {
        var ex = Assert.Throws<ApplicationException>(() => InternalPort.Create(port));
        Assert.Equal("Port range is not valid (1-65535).", ex.Message);
    }

    [Fact]
    public void Equality_WhenPortsEqual_ShouldBeEqual()
    {
        var a = InternalPort.Create(1234);
        var b = InternalPort.Create(1234);
        Assert.True(a.Equals(b));
        Assert.True(a == b);
        Assert.False(a != b);
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void Equality_WhenPortsDifferent_ShouldNotBeEqual()
    {
        var a = InternalPort.Create(1234);
        var b = InternalPort.Create(4321);
        Assert.False(a.Equals(b));
        Assert.False(a == b);
        Assert.True(a != b);
    }
}

