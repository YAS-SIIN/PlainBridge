using PlainBridge.SharedDomain.Base.ValueObjects;
using Xunit;

namespace PlainBridge.Api.Tests.UnitTests.Domain.Shared.ValueObjects;

[Collection("ApiUnitTestRun")]
public class AppIdTests
{
    [Fact]
    public void CreateUniqueId_ShouldGenerateNonEmptyGuid()
    {
        var a = AppId.CreateUniqueId();
        Assert.NotEqual(Guid.Empty, a.ViewId);
    }
    
    [Fact]
    public void Create_ShouldGenerateNonEmptyGuid()
    {
        var a = AppId.Create(Guid.NewGuid());
        Assert.NotEqual(Guid.Empty, a.ViewId);
    }

    [Fact]
    public void Equality_WhenViewIdsEqual_ShouldBeEqual()
    {
        var id = Guid.NewGuid();
        var a = new AppId { ViewId = id };
        var b = new AppId { ViewId = id };

        Assert.True(a.Equals(b));
        Assert.True(a == b);
        Assert.False(a != b);
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void Equality_WhenViewIdsDifferent_ShouldNotBeEqual()
    {
        var a = new AppId { ViewId = Guid.NewGuid() };
        var b = new AppId { ViewId = Guid.NewGuid() };

        Assert.False(a.Equals(b));
        Assert.False(a == b);
        Assert.True(a != b);
    }
}

