

using IdentityModel.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PlainBridge.Api.Application.DTOs;
using PlainBridge.Api.Application.Services.Token;
using PlainBridge.Api.Application.Services.User;
using System.Text.Json;

namespace PlainBridge.Api.Application.Services.Session;


public class SessionService(ILogger<SessionService> _logger, IHttpContextAccessor _httpContextAccessor, IUserService _userService, ITokenService _tokenService, IOptions<ApplicationSetting> _applicationSetting) : ISessionService
{
    public async Task<UserDto> GetCurrentUserAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting current user from HttpContext.");
        var userId = _httpContextAccessor.HttpContext.User.Claims.SingleOrDefault(claim => claim.Type == "sub");
        if (userId == null || string.IsNullOrEmpty(userId.Value))
        {
            _logger.LogWarning("User ID claim 'sub' not found in HttpContext.");
            return null!;
        }
        _logger.LogInformation("Fetching user by external ID: {UserId}", userId.Value);
        var customer = await _userService.GetUserByExternalIdAsync(userId.Value, cancellationToken);
        _logger.LogInformation("User fetched successfully.");
        return customer;
    }

    public async Task<UserProfileViewDto?> GetCurrentUserProfileAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting current user profile from HttpContext.");
        var userId = _httpContextAccessor.HttpContext.User.Claims.SingleOrDefault(claim => claim.Type == "sub");

        if (userId == null || string.IsNullOrEmpty(userId.Value))
        {
            _logger.LogWarning("User ID claim 'sub' not found in HttpContext.");
            return await Task.FromResult<UserProfileViewDto?>(null);
        }

        _logger.LogInformation("Getting token for user: {UserId}", userId.Value);
        var token = await _tokenService.GetSunTokenAsync(userId.Value);
        var baseUri = new Uri(_applicationSetting.Value.PlainBridgeIdsUrl!);
        var uri = new Uri(baseUri, "connect/userinfo");
        var userInfoRequest = new UserInfoRequest
        {
            Address = uri.ToString(),
            Token = token
        };

        _logger.LogInformation("Requesting user info from: {Address}", userInfoRequest.Address);
        var client = new HttpClient();
        var userInfoResponse = await client.GetUserInfoAsync(userInfoRequest);
        var result = await userInfoResponse.HttpResponse.Content.ReadAsStringAsync();

        _logger.LogInformation("User info received and deserializing.");
        return JsonSerializer.Deserialize<UserProfileViewDto>(result);
    }
}
