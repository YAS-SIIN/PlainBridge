

using IdentityModel.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using PlainBridge.Api.Application.DTOs;
using PlainBridge.Api.Application.Services.Token;
using PlainBridge.Api.Application.Services.User;
using System.Text.Json;

namespace PlainBridge.Api.Application.Services.Session;


public class SessionService(IHttpContextAccessor _httpContextAccessor, IUserService _userService, ITokenService _tokenService, IOptions<ApplicationSetting> _applicationSetting) : ISessionService
{


    public async Task<UserDto> GetCurrentUsereAsync(CancellationToken cancellationToken)
    {
        var userId = _httpContextAccessor.HttpContext.User.Claims.SingleOrDefault(claim => claim.Type == "sub");
        var customer = await _userService.GetUserByExternalIdAsync(userId.Value, cancellationToken);

        return customer;
    }

    public async Task<UserProfileViewDto?> GetCurrentUserProfileAsync(CancellationToken cancellationToken)
    {
        var userId = _httpContextAccessor.HttpContext.User.Claims.SingleOrDefault(claim => claim.Type == "sub");

        if (userId == null || string.IsNullOrEmpty(userId.Value))
        {
            return await Task.FromResult<UserProfileViewDto?>(null);
        }

        var token = await _tokenService.GetSunTokenAsync(userId.Value);
        var baseUri = new Uri(_applicationSetting.Value.PlainBridgeIdsUrl!);
        var uri = new Uri(baseUri, "connect/userinfo");
        var userInfoRequest = new UserInfoRequest
        {
            Address = uri.ToString(),
            Token = token
        };

        var client = new HttpClient();
        var userInfoResponse = await client.GetUserInfoAsync(userInfoRequest);
        var result = await userInfoResponse.HttpResponse.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<UserProfileViewDto>(result);
    }
}