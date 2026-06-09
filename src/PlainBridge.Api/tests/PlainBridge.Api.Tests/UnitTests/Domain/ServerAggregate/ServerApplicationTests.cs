using PlainBridge.Api.Domain.ServerAggregate;
using Xunit;

namespace PlainBridge.Api.Tests.UnitTests.Domain.ServerAggregate;

[Collection("ApiUnitTestRun")]
public class ServerApplicationTests
{
    [Fact]
    public void Create_WhenNameTooLong_ShouldThrowApplicationException()
    {
        var longName = new string('x', 151);
        var ex = Assert.Throws<ApplicationException>(() => ServerApplication.Create(Guid.Empty,
            PlainBridge.Api.Domain.ServerAggregate.Enums.ServerApplicationTypeEnum.SharePort,
            longName,
            8080,
            1,
            "desc"));
        Assert.Equal("Name must be 150 characters or fewer.", ex.Message);
    }

    [Fact]
    public void Create_WhenUsePortWithoutViewId_ShouldThrowApplicationException()
    {
        var ex = Assert.Throws<ApplicationException>(() => ServerApplication.Create(null,
            PlainBridge.Api.Domain.ServerAggregate.Enums.ServerApplicationTypeEnum.UsePort,
            "name",
            8080,
            1,
            null));
        Assert.Equal("ServerApplicationViewId is required when ServerApplicationType is UseAppId.", ex.Message);
    }

    [Fact]
    public void Update_ShouldReplaceNamePortAndDescription()
    {
        var app = ServerApplication.Create(Guid.Empty,
            PlainBridge.Api.Domain.ServerAggregate.Enums.ServerApplicationTypeEnum.SharePort,
            "name",
            8080,
            1,
            "desc");

        app.Update("new-name", 9090, "new-desc");

        Assert.Equal("new-name", app.Name);
        Assert.Equal(9090, app.InternalPort.Port);
        Assert.Equal("new-desc", app.Description);
    }

    [Fact]
    public void ActivateDeactivate_ShouldToggleState()
    {
        var app = ServerApplication.Create(Guid.Empty,
            PlainBridge.Api.Domain.ServerAggregate.Enums.ServerApplicationTypeEnum.SharePort,
            "name",
            8080,
            1,
            "desc");

        app.Activate();
        Assert.Equal(PlainBridge.SharedDomain.Base.Enums.RowStateEnum.Active, app.State);

        app.Deactivate();
        Assert.Equal(PlainBridge.SharedDomain.Base.Enums.RowStateEnum.DeActive, app.State);
    }
}

