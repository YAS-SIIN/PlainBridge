

using IdentityModel.Client;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

using PlainBridge.Api.Application.DTOs;
using PlainBridge.Api.Application.Services.User;

using System.Text.Json;

namespace PlainBridge.Api.Application.Services.Session;


public class SessionService(IHttpContextAccessor _httpContextAccessor, IUserService _userService, IConfiguration configuration)
{
  

    public async Task<UserDto> GetCurrentUser(CancellationToken cancellationToken)
    {
        var userId = _httpContextAccessor.HttpContext.User.Claims.SingleOrDefault(claim => claim.Type == "sub");
        var customer = await _userService.GetUserByExternalIdAsync(userId.Value, cancellationToken);

        return customer;
    }
     
}