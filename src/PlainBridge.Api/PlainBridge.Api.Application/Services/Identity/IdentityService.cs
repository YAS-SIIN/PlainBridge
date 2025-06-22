


using IdentityModel.Client;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using PlainBridge.Api.Application.DTOs;
using PlainBridge.SharedApplication.DTOs;

using System.Text;
using System.Text.Json;

namespace PlainBridge.Api.Application.Identity.Customer;

public class IdentityService(ILogger<IdentityService> _logger, IHttpClientFactory _httpClientFactory, IOptions<ApplicationSetting> _applicationSetting) : IIdentityService
{


    private async Task<HttpClient> InitializeHttpClientAsync(CancellationToken cancellationToken)
    {
        var httpClient = _httpClientFactory.CreateClient("Default");
        var disco = await httpClient.GetDiscoveryDocumentAsync(_applicationSetting.Value.IdsUrl.ToString(), cancellationToken);
        if (disco.IsError)
            throw new ApplicationException("Failed to get discivery document");

        var tokenResponse = await httpClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
        {
            Address = disco.TokenEndpoint,
            ClientId = _applicationSetting.Value.IdsClientId,
            ClientSecret = _applicationSetting.Value.IdsClientId,
            Scope = _applicationSetting.Value.IdsScope
        }, cancellationToken);

        if (tokenResponse.IsError)
            throw new ApplicationException("Failed to get token from identity server");

        httpClient.SetBearerToken(tokenResponse.AccessToken);

        return httpClient;
    }

    public async Task<ResultDto<string>> CreateUserAsync(string username, string password, string email, string name, string family, CancellationToken cancellationToken)
    {
        var httpClient = await InitializeHttpClientAsync(cancellationToken);

        var jsonObject = new
        {
            Username = username,
            Password = password,
            Email = email,
            Name = name,
            Family = family
        };

        var content = new StringContent(JsonSerializer.Serialize(jsonObject), Encoding.UTF8, "application/json");
        var uri = new Uri($"{_applicationSetting.Value.IdsUrl}/User");

        HttpResponseMessage? response;

        response = await httpClient.PostAsync(uri.ToString(), content);

        var responseString = await response.Content.ReadAsStringAsync(cancellationToken);

        var userCreationResult = JsonSerializer.Deserialize<ResultDto<string>>(responseString, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }) ?? new ResultDto<string>();

        return userCreationResult;
    }

    public async Task<ResultDto<string>> ChangePasswordAsync(string userId, string currentPassword, string newPassword, CancellationToken cancellationToken)
    {
        var httpClient = await InitializeHttpClientAsync(cancellationToken);

        var jsonObject = new
        {
            UserId = userId,
            CurrentPassword = currentPassword,
            NewPassword = newPassword
        };

        var content = new StringContent(JsonSerializer.Serialize(jsonObject), Encoding.UTF8, "application/json");
        var uri = new Uri($"{_applicationSetting.Value.IdsUrl}/User/ChangePassword");

        HttpResponseMessage? response;

        response = await httpClient.PatchAsync(uri.ToString(), content);
        var responseString = await response.Content.ReadAsStringAsync(cancellationToken);

        var userChangePasswordResult = JsonSerializer.Deserialize<ResultDto<string>>(responseString, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }) ?? new ResultDto<string>();

        return userChangePasswordResult;
    }

    public async Task<ResultDto<string>> UpdateUserAsync(string userId, string name, string family, CancellationToken cancellationToken)
    {
        var httpClient = await InitializeHttpClientAsync(cancellationToken);

        var jsonObject = new
        {
            UserId = userId,
            Name = name,
            Family = family
        };

        var content = new StringContent(JsonSerializer.Serialize(jsonObject), Encoding.UTF8, "application/json");
        var uri = new Uri($"{_applicationSetting.Value.IdsUrl}/User");

        HttpResponseMessage? response;

        response = await httpClient.PatchAsync(uri.ToString(), content);
        var responseString = await response.Content.ReadAsStringAsync(cancellationToken);

        var userUpdatingResult = JsonSerializer.Deserialize<ResultDto<string>>(responseString, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }) ?? new ResultDto<string>();

        return userUpdatingResult;
    }

}
