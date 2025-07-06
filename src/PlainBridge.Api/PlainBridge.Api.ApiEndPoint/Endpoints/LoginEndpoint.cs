using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using PlainBridge.Api.Application.DTOs;
using PlainBridge.Api.Application.Services.Session;
using PlainBridge.Api.Application.Services.Token;
using PlainBridge.Api.Application.Services.User;
using PlainBridge.SharedApplication.Exceptions;

namespace PlainBridge.Api.ApiEndPoint.Endpoints;

public static class LoginEndpoint
{
    public static void MapLoginEndpoint(this IEndpointRouteBuilder builder)
    {
        var app = builder.MapGroup(""); 
        app.MapBffManagementEndpoints();
        app.MapControllers();

        app.MapGet("/", async (CancellationToken cancellationToken, HttpContext context, ILoggerFactory loggerFactory, IHttpContextAccessor httpContextAccessor, ITokenService tokenService, ISessionService sessionService, IUserService customerService, IOptions<ApplicationSetting> _applicationSetting) =>
        {
            var _logger = loggerFactory.CreateLogger(nameof(LoginEndpoint));

            var token = default(string);

            token = httpContextAccessor.HttpContext!.Request.Query["access_token"];
            if (string.IsNullOrEmpty(token))
                token = await httpContextAccessor.HttpContext.GetTokenAsync("access_token");
            var idToken = await httpContextAccessor.HttpContext.GetTokenAsync("id_token");
            var refreshToken = await httpContextAccessor.HttpContext.GetTokenAsync("refresh_token");
             

            var jwtSecurityToken = tokenService.ParseToken(token);
            var sub = jwtSecurityToken.Claims.Single(claim => claim.Type == "sub").Value;
            var tokenp = await tokenService.GenerateToken(sub, string.Empty, string.Empty);

            _logger.LogInformation($"{sub} logged in, Token: {token}, TokenP: {tokenp}");

            await tokenService.SetTokenPSubAsync(tokenp, sub);
            await tokenService.SetSubTokenAsync(sub, token);
            await tokenService.SetSubTokenPAsync(sub, tokenp);
            await tokenService.SetTokenPTokenAsync(tokenp, token);
            await tokenService.SetSubIdTokenAsync(sub, idToken!);
            await tokenService.SetTokenPRefreshTokenAsync(tokenp, refreshToken!);

            try
            {
                await customerService.GetUserByExternalIdAsync(sub, cancellationToken);
            }
            catch (NotFoundException)
            {
                var customerProfile = await sessionService.GetCurrentUserProfileAsync(cancellationToken);
                var user = new UserDto
                {
                    ExternalId = sub,
                    Username = customerProfile.Username,
                    Email = customerProfile.Email,
                    Name = customerProfile.Name,
                    Family = customerProfile.Family
                };
                await customerService.CreateLocallyAsync(user, cancellationToken);
            }

            var baseUri = new Uri(_applicationSetting.Value.PlainBridgeWebRedirectUrl!);
            var uri = new Uri(baseUri, $"?access_token={tokenp}");
            httpContextAccessor.HttpContext.Response.Redirect(uri.ToString(), true);
            return Task.FromResult(0);
        });
    }
}
