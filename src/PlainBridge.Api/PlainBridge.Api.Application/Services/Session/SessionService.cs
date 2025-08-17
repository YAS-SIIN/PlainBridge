

using IdentityModel.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PlainBridge.Api.Application.DTOs;
using PlainBridge.Api.Application.Services.Token;
using PlainBridge.Api.Application.Services.User;
using PlainBridge.SharedApplication.DTOs;
using System.Text.Json;

namespace PlainBridge.Api.Application.Services.Session;


public class SessionService(ILogger<SessionService> _logger, IHttpContextAccessor _httpContextAccessor, IUserService _userService, ITokenService _tokenService, IHttpClientFactory _httpClientFactory, IOptions<ApplicationSettings> _applicationSettings) : ISessionService
{
    public async Task<UserDto> GetCurrentUserAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting current user from HttpContext.");

         
        // First, try to find the 'sub' claim (for user authentication)
        var userId = _httpContextAccessor.HttpContext.User.Claims.SingleOrDefault(claim => claim.Type == "sub");
        
        // If no 'sub' claim, try ClaimTypes.NameIdentifier (in case of claim mapping)
        if (userId == null)
        {
            userId = _httpContextAccessor.HttpContext.User.Claims.SingleOrDefault(claim => claim.Type == System.Security.Claims.ClaimTypes.NameIdentifier);
        }
        
        if (userId == null || string.IsNullOrEmpty(userId.Value))
        {
            // Check if this is a client credentials token (has client_id but no sub)
            var clientId = _httpContextAccessor.HttpContext.User.Claims.SingleOrDefault(claim => claim.Type == "client_id");
            if (clientId != null)
            {
                _logger.LogWarning("Request authenticated with client credentials (client_id: {ClientId}), but no user identity available. User-specific operations not supported.", clientId.Value);
            }
            else
            {
                _logger.LogWarning("No user identity claim found in HttpContext. User might not be authenticated or using unsupported authentication method.");
            }
            return await Task.FromResult<UserDto>(null!);
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
        var token = await _tokenService.GetSubTokenAsync(userId.Value);
        var baseUri = new Uri(_applicationSettings.Value.PlainBridgeIdsUrl!);
        var uri = new Uri(baseUri, "connect/userinfo");
        var userInfoRequest = new UserInfoRequest
        {
            Address = uri.ToString(),
            Token = token
        };

        _logger.LogInformation("Requesting user info from: {Address}", userInfoRequest.Address);
        var httpClientx = _httpClientFactory.CreateClient("Default"); 
        var userInfoResponse = await httpClientx.GetUserInfoAsync(userInfoRequest);
        var result = await userInfoResponse.HttpResponse.Content.ReadAsStringAsync();

        _logger.LogInformation("User info received and deserializing.");
        return JsonSerializer.Deserialize<UserProfileViewDto>(result);
    }
}
