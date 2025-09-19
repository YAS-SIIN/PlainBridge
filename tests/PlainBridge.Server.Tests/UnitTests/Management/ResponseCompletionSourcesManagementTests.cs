using PlainBridge.Server.Application.Management.ResponseCompletionSources;
using PlainBridge.SharedApplication.DTOs;

namespace PlainBridge.Server.Tests.UnitTests.Management;

[Collection("ServerUnitTestRun")]
public class ResponseCompletionSourcesManagementTests
{

    [Fact]
    public async Task Sources_AddAndRemove_Works()
    {
        var mgr = new ResponseCompletionSourcesManagement();
        var id = Guid.NewGuid().ToString();
        var tcs = new TaskCompletionSource<HttpResponseDto>();

        Assert.True(mgr.Sources.TryAdd(id, tcs));
        Assert.True(mgr.Sources.TryGetValue(id, out var found));

        var dto = new HttpResponseDto { StringContent = "done" };
        found.SetResult(dto);

        var res = await tcs.Task;
        Assert.Equal("done", res.StringContent);
    }
}
