using PlainBridge.Api.Domain.UserAggregate.ValueObjects;
using Xunit;

namespace PlainBridge.Api.Tests.UnitTests.Domain.UserAggregate.ValueObjects;

[Collection("ApiUnitTestRun")]
public class UserNameTests
{
    [Fact]
    public void Create_WhenValid_ShouldHoldValue()
    {
        var u = UserName.Create("john");
        Assert.Equal("john", u.UserNameValue);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_WhenNullOrWhitespace_ShouldThrowArgumentException(string value)
    {
        Assert.Throws<ArgumentException>(() => UserName.Create(value));
    }
}

