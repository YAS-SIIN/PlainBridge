using PlainBridge.Api.Domain.HostAggregate;
using Xunit;

namespace PlainBridge.Api.Tests.UnitTests.Domain.HostAggregate;

[Collection("ApiUnitTestRun")]
public class HostApplicationTests
{
    [Fact]
    public void Create_WhenNameTooLong_ShouldThrow()
    {
        var longName = new string('x', 151);
        var ex = Assert.Throws<ApplicationException>(() => HostApplication.Create(longName,
            "domain",
            "http://internal",
            1,
            "desc"));
        Assert.Equal("Name must be 150 characters or fewer.", ex.Message);
    }

    [Fact]
    public void Create_WhenValid_ShouldPopulateFields()
    {
        var app = HostApplication.Create("name",
            "domain",
            "http://internal",
            1,
            "desc");

        Assert.Equal("name", app.Name);
        Assert.Equal("domain", app.Domain.HostDomainName);
        Assert.Equal("http://internal", app.InternalUrl.InternalUrlValue);
        Assert.Equal(1, app.UserId);
        Assert.Equal(PlainBridge.SharedDomain.Base.Enums.RowStateEnum.DeActive, app.State);
        Assert.NotEqual(Guid.Empty, app.AppId.ViewId);
    }

    [Fact]
    public void Update_ShouldReplaceNameDomainUrlAndDescription()
    {
        var app = HostApplication.Create("name","d","http://i",1,"desc");

        app.Update("new-name", "new-domain", "http://new", "new-desc");

        Assert.Equal("new-name", app.Name);
        Assert.Equal("new-domain", app.Domain.HostDomainName);
        Assert.Equal("http://new", app.InternalUrl.InternalUrlValue);
        Assert.Equal("new-desc", app.Description);
    }

    [Fact]
    public void ActivateDeactivate_ShouldToggleState()
    {
        var app = HostApplication.Create("name","d","http://i",1,"desc");

        app.Activate();
        Assert.Equal(PlainBridge.SharedDomain.Base.Enums.RowStateEnum.Active, app.State);

        app.Deactivate();
        Assert.Equal(PlainBridge.SharedDomain.Base.Enums.RowStateEnum.DeActive, app.State);
    }
}

