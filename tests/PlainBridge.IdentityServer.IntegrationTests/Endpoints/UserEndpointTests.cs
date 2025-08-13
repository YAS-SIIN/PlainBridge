

using System.Text;
using System.Text.Json;
using Elastic.Clients.Elasticsearch;
using PlainBridge.IdentityServer.EndPoint.DTOs;
using PlainBridge.SharedApplication.DTOs;
using PlainBridge.SharedApplication.Enums;

namespace PlainBridge.IdentityServer.IntegrationTests.Endpoints;

[Collection("IDSIntegrationTestRun")]
public class UserEndpointTests : IClassFixture<TestRunFixture>
{
    private readonly TestRunFixture _fixture;
    public UserEndpointTests(TestRunFixture fixture) => _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
     
    #region CreateUser

    [Theory]
    [MemberData(nameof(UserEndpointData.SetDataFor_CreateUser_WhenEveryThingIsOk_ShouldBeSucceeded), MemberType = typeof(UserEndpointData))]
    public async Task CreateUser_WhenEveryThingIsOk_ShouldBeSucceeded(UserDto userDto)
    {
        var payload = JsonSerializer.Serialize(userDto);

        HttpContent httpContent = new StringContent(payload, Encoding.UTF8, "application/json");
        var response = await _fixture.InjectedHttpClient.PostAsync("/api/user", httpContent, CancellationToken.None);
        response.EnsureSuccessStatusCode();
        var res = await response.Content.ReadAsStringAsync();
        Assert.NotNull(res);
        var responseData = JsonSerializer.Deserialize<ResultDto<object>>(res, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        Assert.NotNull(responseData);
        Assert.Equal(ResultCodeEnum.Success, responseData.ResultCode);
    }

    [Theory]
    [MemberData(nameof(UserEndpointData.SetDataFor_CreateUser_WhenPasswordIsNotValid_ShouldThrowException), MemberType = typeof(UserEndpointData))]
    public async Task CreateUser_WhenPasswordIsNotValid_ShouldThrowException(UserDto userDto)
    {
        var payload = JsonSerializer.Serialize(userDto);

        HttpContent httpContent = new StringContent(payload, Encoding.UTF8, "application/json");
        var response = await _fixture.InjectedHttpClient.PostAsync("/api/user", httpContent, CancellationToken.None);
        Assert.False(response.IsSuccessStatusCode);
        var res = await response.Content.ReadAsStringAsync();
        Assert.NotNull(res);
    }

    [Theory]
    [MemberData(nameof(UserEndpointData.SetDataFor_CreateUser_WhenUsernameIsEmpty_ShouldThrowException), MemberType = typeof(UserEndpointData))]
    public async Task CreateUser_WhenUsernameIsEmpty_ShouldThrowException(UserDto userDto)
    {
        var payload = JsonSerializer.Serialize(userDto);

        HttpContent httpContent = new StringContent(payload, Encoding.UTF8, "application/json");
        var response = await _fixture.InjectedHttpClient.PostAsync("/api/user", httpContent, CancellationToken.None);
        Assert.False(response.IsSuccessStatusCode);
        var res = await response.Content.ReadAsStringAsync();
        Assert.NotNull(res);
    }

    [Theory]
    [MemberData(nameof(UserEndpointData.SetDataFor_CreateUser_WhenUsernameIsDuplicated_ShouldThrowException), MemberType = typeof(UserEndpointData))]
    public async Task CreateUser_WhenUsernameIsDuplicated_ShouldThrowException(UserDto userDto)
    {
        var payload = JsonSerializer.Serialize(userDto);

        HttpContent httpContent = new StringContent(payload, Encoding.UTF8, "application/json");
        var response = await _fixture.InjectedHttpClient.PostAsync("/api/user", httpContent, CancellationToken.None);
        Assert.False(response.IsSuccessStatusCode);
        var res = await response.Content.ReadAsStringAsync();
        Assert.NotNull(res);
    }

    #endregion

    #region UpdateUser

    [Theory]
    [MemberData(nameof(UserEndpointData.SetDataFor_UpdateUser_WhenEveryThingIsOk_ShouldBeSucceeded), MemberType = typeof(UserEndpointData))]
    public async Task UpdateUser_WhenEveryThingIsOk_ShouldBeSucceeded(UserDto userDto)
    {
        userDto.UserId = _fixture.CreatedUer.UserId;
        var payload = JsonSerializer.Serialize(userDto);

        HttpContent httpContent = new StringContent(payload, Encoding.UTF8, "application/json");
        var response = await _fixture.InjectedHttpClient.PatchAsync("/api/user", httpContent, CancellationToken.None);
        response.EnsureSuccessStatusCode();
        var res = await response.Content.ReadAsStringAsync();
        Assert.NotNull(res);
        var responseData = JsonSerializer.Deserialize<ResultDto<object>>(res, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        Assert.NotNull(responseData);
        Assert.Equal(ResultCodeEnum.Success, responseData.ResultCode);
    }

    [Theory]
    [MemberData(nameof(UserEndpointData.SetDataFor_UpdateUser_WhenUserIdNotExist_ShouldThrowException), MemberType = typeof(UserEndpointData))]
    public async Task UpdateUser_WhenUserIdNotExist_ShouldThrowException(UserDto userDto)
    {
        var payload = JsonSerializer.Serialize(userDto);

        HttpContent httpContent = new StringContent(payload, Encoding.UTF8, "application/json");
        var response = await _fixture.InjectedHttpClient.PatchAsync("/api/user", httpContent, CancellationToken.None);
        Assert.False(response.IsSuccessStatusCode);
        var res = await response.Content.ReadAsStringAsync();
        Assert.NotNull(res);
    }

    #endregion

    #region ChangePassword

    [Theory]
    [MemberData(nameof(UserEndpointData.SetDataFor_ChangePassword_WhenEveryThingIsOk_ShouldBeSucceeded), MemberType = typeof(UserEndpointData))]
    public async Task ChangePassword_WhenEveryThingIsOk_ShouldBeSucceeded(ChangeUserPasswordDto userDto)
    {
        userDto.UserId = _fixture.CreatedUer.UserId;
        var payload = JsonSerializer.Serialize(userDto);

        HttpContent httpContent = new StringContent(payload, Encoding.UTF8, "application/json");
        var response = await _fixture.InjectedHttpClient.PatchAsync("/api/user/ChangePassword", httpContent, CancellationToken.None);
        response.EnsureSuccessStatusCode();
        var res = await response.Content.ReadAsStringAsync();
        Assert.NotNull(res);
        var responseData = JsonSerializer.Deserialize<ResultDto<object>>(res, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        Assert.NotNull(responseData);
        Assert.Equal(ResultCodeEnum.Success, responseData.ResultCode);
    }

    [Theory]
    [MemberData(nameof(UserEndpointData.SetDataFor_ChangePassword_WhenUserIdNotExist_ShouldThrowException), MemberType = typeof(UserEndpointData))]
    public async Task ChangePassword_WhenUserIdNotExist_ShouldThrowException(ChangeUserPasswordDto userDto)
    {
        var payload = JsonSerializer.Serialize(userDto);

        HttpContent httpContent = new StringContent(payload, Encoding.UTF8, "application/json");
        var response = await _fixture.InjectedHttpClient.PatchAsync("/api/user/ChangePassword", httpContent, CancellationToken.None);
        Assert.False(response.IsSuccessStatusCode);
        var res = await response.Content.ReadAsStringAsync();
        Assert.NotNull(res);
    }

    [Theory]
    [MemberData(nameof(UserEndpointData.SetDataFor_ChangePassword_WhenPasswordIsNotValid_ShouldThrowException), MemberType = typeof(UserEndpointData))]
    public async Task ChangePassword_WhenPasswordIsNotValid_ShouldThrowException(ChangeUserPasswordDto userDto)
    {
        userDto.UserId = _fixture.CreatedUer.UserId;
        var payload = JsonSerializer.Serialize(userDto);

        HttpContent httpContent = new StringContent(payload, Encoding.UTF8, "application/json");
        var response = await _fixture.InjectedHttpClient.PatchAsync("/api/user/ChangePassword", httpContent, CancellationToken.None);
        Assert.False(response.IsSuccessStatusCode);
        var res = await response.Content.ReadAsStringAsync();
        Assert.NotNull(res);
    }

    #endregion

}
