
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using PlainBridge.Server.Application.DTOs;
using PlainBridge.Server.Application.Management.Cache;
using PlainBridge.SharedApplication.DTOs;
using PlainBridge.SharedApplication.Enums;

using System.Text.Json;

namespace PlainBridge.Server.Application.Handler.PlainBridgeApiClient;

public class PlainBridgeApiClientHandler(ILogger<PlainBridgeApiClientHandler> _logger, IOptions<ApplicationSetting> _applicationSetting, IHttpClientFactory _httpClientFactory) : IPlainBridgeApiClientHandler
{
 
    public async Task<IList<HostApplicationDto>?> GetHostApplicationsAsync(CancellationToken cancellationToken)
    {
        var apiClient = _httpClientFactory.CreateClient("Api");
        var uri = new Uri($"{_applicationSetting.Value.ApiAddress}/HostApplication"); // Fixed the Uri creation
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

    public async Task<IList<ServerApplicationDto>?> GetServerApplicationsAsync(CancellationToken cancellationToken)
    {
        var apiClient = _httpClientFactory.CreateClient("Api");
        var uri = new Uri($"{_applicationSetting.Value.ApiAddress}/ServerApplication"); // Fixed the Uri creation
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
