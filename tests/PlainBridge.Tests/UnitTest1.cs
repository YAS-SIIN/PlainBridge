using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using PlainBridge.Tests.Utils;

namespace PlainBridge.Tests;

public class UnitTest1 : IClassFixture<AppHostIntegrationTestRunFixture>
{
    public AppHostIntegrationTestRunFixture _fixture { get; }
    public UnitTest1(AppHostIntegrationTestRunFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void Test1()
    {

        Assert.True(true);
    }
}
