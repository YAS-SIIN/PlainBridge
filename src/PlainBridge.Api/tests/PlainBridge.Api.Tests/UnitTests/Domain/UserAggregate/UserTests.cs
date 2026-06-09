using PlainBridge.Api.Domain.UserAggregate;
using Xunit;

namespace PlainBridge.Api.Tests.UnitTests.Domain.UserAggregate;

[Collection("ApiUnitTestRun")]
public class UserTests
{
    [Fact]
    public void Create_WhenNameTooLong_ShouldThrowApplicationException()
    {
        var longName = new string('x', 151);
        var ex = Assert.Throws<ApplicationException>(() => User.Create(
            externalId: "ext",
            username: "user",
            email: "user@example.com",
            phoneNumber: "+989121234567",
            name: longName,
            family: "family",
            description: null));
        Assert.Equal("Name must be 150 characters or fewer.", ex.Message);
    }

    [Fact]
    public void Create_WhenFamilyTooLong_ShouldThrowApplicationException()
    {
        var longFamily = new string('x', 151);
        var ex = Assert.Throws<ApplicationException>(() => User.Create(
            externalId: "ext",
            username: "user",
            email: "user@example.com",
            phoneNumber: "+989121234567",
            name: "name",
            family: longFamily,
            description: null));
        Assert.Equal("Family must be 150 characters or fewer.", ex.Message);
    }

    [Fact]
    public void Create_WhenUsernameTooLong_ShouldThrowApplicationException()
    {
        var longUsername = new string('x', 151);
        var ex = Assert.Throws<ApplicationException>(() => User.Create(
            externalId: "ext",
            username: longUsername,
            email: "user@example.com",
            phoneNumber: "+989121234567",
            name: "name",
            family: "family",
            description: null));
        Assert.Equal("Username must be 150 characters or fewer.", ex.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_WhenEmailEmptyOrWhitespace_ShouldThrowArgumentException(string email)
    {
        var ex = Assert.Throws<ArgumentException>(() => User.Create(
            externalId: "ext",
            username: "user",
            email: email,
            phoneNumber: "+989121234567",
            name: "name",
            family: "family",
            description: null));
        Assert.NotNull(ex.Message);
       
    }

    [Fact]
    public void Create_WhenEmailFormatInvalid_ShouldThrowApplicationException()
    {
        var ex = Assert.Throws<ApplicationException>(() => User.Create(
            externalId: "ext",
            username: "user",
            email: "not-an-email",
            phoneNumber: "+989121234567",
            name: "name",
            family: "family",
            description: null));
        Assert.Equal("Email format is not valid.", ex.Message);
    }

    [Theory]
    [InlineData("+1")] // too short for regex
    [InlineData("123")] // missing leading + and country code rules
    [InlineData("+1234567890123456")] // too long > 15 digits
    public void Create_WhenPhoneNumberInvalid_ShouldThrowApplicationException(string phone)
    {
        var ex = Assert.Throws<ApplicationException>(() => User.Create(
            externalId: "ext",
            username: "user",
            email: "user@example.com",
            phoneNumber: phone,
            name: "name",
            family: "family",
            description: null));
        Assert.Equal("Phone number format is not valid.", ex.Message);
    }

    [Fact]
    public void Create_WhenValid_ShouldPopulateFieldsAndSetActive()
    {
        var u = User.Create(
            externalId: "ext",
            username: "user",
            email: "user@example.com",
            phoneNumber: "+989121234567",
            name: "name",
            family: "family",
            description: "desc");

        Assert.Equal("user", u.UserName.UserNameValue);
        Assert.Equal("user@example.com", u.Email);
        Assert.Equal("name", u.Name);
        Assert.Equal("family", u.Family);
        Assert.Equal(PlainBridge.SharedDomain.Base.Enums.RowStateEnum.Active, u.State);
        Assert.NotEqual(Guid.Empty, u.AppId.ViewId);
    }

    [Fact]
    public void Update_ShouldChangeNameFamilyAndDescription()
    {
        var u = User.Create("ext","user","user@example.com","+989121234567","name","family", null);
        u.Update("new-name", "new-family", "new-desc");

        Assert.Equal("new-name", u.Name);
        Assert.Equal("new-family", u.Family);
        Assert.Equal("new-desc", u.Description);
    }

    [Fact]
    public void ActivateDeactivate_ShouldToggleState()
    {
        var u = User.Create("ext","user","user@example.com","+989121234567","name","family", null);
        u.Deactivate();
        Assert.Equal(PlainBridge.SharedDomain.Base.Enums.RowStateEnum.DeActive, u.State);
        u.Activate();
        Assert.Equal(PlainBridge.SharedDomain.Base.Enums.RowStateEnum.Active, u.State);
    }
}

