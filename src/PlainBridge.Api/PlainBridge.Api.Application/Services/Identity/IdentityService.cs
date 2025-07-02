


using IdentityModel.Client;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using PlainBridge.Api.Application.DTOs;
using PlainBridge.SharedApplication.DTOs;

using System.Text;
using System.Text.Json;

namespace PlainBridge.Api.Application.Services.Identity;

public class IdentityService(ILogger<IdentityService> _logger, IHttpClientFactory _httpClientFactory, IOptions<ApplicationSetting> _applicationSetting) : IIdentityService
{


    private async Task<HttpClient> InitializeHttpClientAsync(CancellationToken cancellationToken)
    {
        var httpClient = _httpClientFactory.CreateClient("Default");
        var disco = await httpClient.GetDiscoveryDocumentAsync(new Uri(_applicationSetting.Value.PlainBridgeIdsUrl).ToString(), cancellationToken);
        if (disco.IsError)
            throw new ApplicationException("Failed to get discivery document");

        var tokenResponse = await httpClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
        {
            Address = disco.TokenEndpoint,
            ClientId = _applicationSetting.Value.IdsClientId,
            ClientSecret = _applicationSetting.Value.IdsClientSecret,
            Scope = _applicationSetting.Value.IdsScope
        }, cancellationToken);

        if (tokenResponse.IsError)
            throw new ApplicationException("Failed to get token from identity server");

        httpClient.SetBearerToken(tokenResponse.AccessToken);

        return httpClient;
    }

    public async Task<ResultDto<string>> CreateUserAsync(UserDto user, CancellationToken cancellationToken)
    {
        var httpClient = await InitializeHttpClientAsync(cancellationToken);

        var jsonObject = new
        {
            user.Username,
            user.Password,
            user.Email,
            user.PhoneNumber,
            user.Name,
            user.Family
        };

        var content = new StringContent(JsonSerializer.Serialize(jsonObject), Encoding.UTF8, "application/json");
        var uri = new Uri($"{_applicationSetting.Value.PlainBridgeIdsUrl}/api/User");

        HttpResponseMessage? response;

        response = await httpClient.PostAsync(uri.ToString(), content);

        var responseString = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode || string.IsNullOrEmpty(responseString))
        { 
            return new ResultDto<string>();
        }

        var userCreationResult = JsonSerializer.Deserialize<ResultDto<string>>(responseString, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }) ?? new ResultDto<string>();

        return userCreationResult;
    }

    public async Task<ResultDto<string>> ChangePasswordAsync(ChangeUserPasswordDto changeUserPassword, CancellationToken cancellationToken)
    {
        var httpClient = await InitializeHttpClientAsync(cancellationToken);

        var jsonObject = new
        {
            UserId = changeUserPassword.Id,
            changeUserPassword.CurrentPassword,
            changeUserPassword.NewPassword
        };

        var content = new StringContent(JsonSerializer.Serialize(jsonObject), Encoding.UTF8, "application/json");
        var uri = new Uri($"{_applicationSetting.Value.PlainBridgeIdsUrl}/api/User/ChangePassword");

        HttpResponseMessage? response;

        response = await httpClient.PatchAsync(uri.ToString(), content);
        var responseString = await response.Content.ReadAsStringAsync(cancellationToken);

        var userChangePasswordResult = JsonSerializer.Deserialize<ResultDto<string>>(responseString, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }) ?? new ResultDto<string>();

        return userChangePasswordResult;
    }

    public async Task<ResultDto<string>> UpdateUserAsync(UserDto user, CancellationToken cancellationToken)
    {
        var httpClient = await InitializeHttpClientAsync(cancellationToken);

        var jsonObject = new
        {
            user.Username,
            user.Password,
            user.Email,
            user.PhoneNumber,
            user.Name,
            user.Family
        };

        var content = new StringContent(JsonSerializer.Serialize(jsonObject), Encoding.UTF8, "application/json");
        var uri = new Uri($"{_applicationSetting.Value.PlainBridgeIdsUrl}/api/User");

        HttpResponseMessage? response;

        response = await httpClient.PatchAsync(uri.ToString(), content);
        var responseString = await response.Content.ReadAsStringAsync(cancellationToken);

        var userUpdatingResult = JsonSerializer.Deserialize<ResultDto<string>>(responseString, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }) ?? new ResultDto<string>();

        return userUpdatingResult;
    }

}
