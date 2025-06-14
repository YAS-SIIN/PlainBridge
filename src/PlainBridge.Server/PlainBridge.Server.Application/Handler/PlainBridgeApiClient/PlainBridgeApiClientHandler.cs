
using PlainBridge.Api.Application.DTOs;
using PlainBridge.SharedApplication.DTOs;
using PlainBridge.SharedApplication.Enums;

using System.Text.Json;

namespace PlainBridge.Server.Application.Handler.PlainBridgeApiClient;

public class PlainBridgeApiClientHandler : IPlainBridgeApiClientHandler
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ApplicationSetting _applicationSetting;

    public PlainBridgeApiClientHandler(ApplicationSetting applicationSetting, IHttpClientFactory httpClientFactory)
    {
        _applicationSetting = applicationSetting;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<IList<HostApplicationDto>?> GetProjectsAsync(CancellationToken cancellationToken)
    {
        var apiClient = _httpClientFactory.CreateClient("Api");
        var uri = new Uri($"{_applicationSetting.ApiAddress}/HostApplication"); // Fixed the Uri creation
        var response = await apiClient.GetAsync(uri, cancellationToken); // Added cancellationToken to GetAsync
        if (!response.IsSuccessStatusCode)
            throw new ApplicationException("Failed to get host application");

        var content = await response.Content.ReadAsStringAsync(cancellationToken); // Added cancellationToken to ReadAsStringAsync
        var result = JsonSerializer.Deserialize<ResultDto<IList<HostApplicationDto>>>(content, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        if (result == null)
            throw new ApplicationException("Failed to deserialize response");

        if (result.ResultCode != ResultCodeEnum.Success) // Fixed the condition to check for failure
            throw new ApplicationException(result.ResultDescription);

        return result.Data;
    }

    public async Task<IList<ServerApplicationDto>?> GetAppProjectsAsync(CancellationToken cancellationToken)
    {
        var apiClient = _httpClientFactory.CreateClient("Api");
        var uri = new Uri($"{apiClient.BaseAddress}/ServerApplication"); // Fixed the Uri creation
        var response = await apiClient.GetAsync(uri, cancellationToken); // Added cancellationToken to GetAsync
        if (!response.IsSuccessStatusCode)
            throw new ApplicationException("Failed to get server application");

        var content = await response.Content.ReadAsStringAsync(cancellationToken); // Added cancellationToken to ReadAsStringAsync
        var result = JsonSerializer.Deserialize<ResultDto<IList<ServerApplicationDto>>>(content, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        if (result == null)
            throw new ApplicationException("Failed to deserialize response");

        if (result.ResultCode != ResultCodeEnum.Success) // Fixed the condition to check for failure
            throw new ApplicationException(result.ResultDescription);

        return result.Data;
    }
}
