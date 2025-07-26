
using System.Text.Json;
using IdentityModel.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PlainBridge.Server.Application.DTOs;
using PlainBridge.Server.Application.Management.Cache;
using PlainBridge.Server.Application.Services.Identity;
using PlainBridge.SharedApplication.DTOs;
using PlainBridge.SharedApplication.Enums;

namespace PlainBridge.Server.Application.Handler.PlainBridgeApiClient;

public class PlainBridgeApiClientHandler(ILogger<PlainBridgeApiClientHandler> _logger, IOptions<ApplicationSettings> _applicationSettings, IHttpClientFactory _httpClientFactory, IIdentityService _identityService) : IPlainBridgeApiClientHandler
{
 
    public async Task<IList<HostApplicationDto>?> GetHostApplicationsAsync(CancellationToken cancellationToken)
    {
        var token = await _identityService.GetTokenAsync(cancellationToken);
        if (token == null) throw new ApplicationException("Token request failed");

        if (token.IsError)
            throw new ApplicationException($"Token request failed, {token.Error}");

        var apiClient = _httpClientFactory.CreateClient("Api");
        apiClient.SetBearerToken(token.AccessToken!);
        var uri = new Uri($"{_applicationSettings.Value.PlainBridgeApiAddress}/HostApplication"); // Fixed the Uri creation
        var response = await apiClient.GetAsync(uri, cancellationToken); // Added cancellationToken to GetAsync
        if (!response.IsSuccessStatusCode)
            return Enumerable.Empty<HostApplicationDto>().ToList();

        var content = await response.Content.ReadAsStringAsync(cancellationToken); // Added cancellationToken to ReadAsStringAsync
        var result = JsonSerializer.Deserialize<ResultDto<IList<HostApplicationDto>>>(content, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        if (result == null)
            return Enumerable.Empty<HostApplicationDto>().ToList();

        if (result.ResultCode != ResultCodeEnum.Success)
            return Enumerable.Empty<HostApplicationDto>().ToList();

        return result.Data;
    }

    public async Task<IList<ServerApplicationDto>?> GetServerApplicationsAsync(CancellationToken cancellationToken)
    {
        var token = await _identityService.GetTokenAsync(cancellationToken);
        if (token == null) throw new ApplicationException("Token request failed");

        if (token.IsError)
            throw new ApplicationException($"Token request failed, {token.Error}");


        var apiClient = _httpClientFactory.CreateClient("Api");
        apiClient.SetBearerToken(token.AccessToken!);
        var uri = new Uri($"{_applicationSettings.Value.PlainBridgeApiAddress}/ServerApplication"); // Fixed the Uri creation
        var response = await apiClient.GetAsync(uri, cancellationToken); // Added cancellationToken to GetAsync
        if (!response.IsSuccessStatusCode)
            return Enumerable.Empty<ServerApplicationDto>().ToList(); 

        var content = await response.Content.ReadAsStringAsync(cancellationToken); // Added cancellationToken to ReadAsStringAsync
        var result = JsonSerializer.Deserialize<ResultDto<IList<ServerApplicationDto>>>(content, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        if (result == null)
            return Enumerable.Empty<ServerApplicationDto>().ToList();

        if (result.ResultCode != ResultCodeEnum.Success)
            return Enumerable.Empty<ServerApplicationDto>().ToList();

        return result.Data;
    }
     
}
