using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using PlainBridge.Api.ApiEndPoint.Abstractions;
using PlainBridge.Api.Application.Services.Session;
using PlainBridge.Api.Application.Services.Token;
using PlainBridge.Api.Application.Services.User;
using PlainBridge.Api.Infrastructure.DTOs;
using PlainBridge.SharedApplication.Exceptions;

namespace PlainBridge.Api.ApiEndPoint.Endpoints;

public class LoginEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        var app = builder.MapGroup(""); 
        app.MapBffManagementEndpoints();
     

        app.MapGet("/", async (CancellationToken cancellationToken, ILoggerFactory loggerFactory, IHttpContextAccessor _httpContextAccessor, ITokenService _tokenService, ISessionService _sessionService, IUserService _userService, IOptions<ApplicationSettings> _applicationSettings) =>
        {
            var logger = loggerFactory.CreateLogger(nameof(LoginEndpoint));

            var token = default(string);

            token = _httpContextAccessor.HttpContext!.Request.Query["access_token"];
            if (string.IsNullOrEmpty(token))
                token = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
            var idToken = await _httpContextAccessor.HttpContext.GetTokenAsync("id_token");
            var refreshToken = await _httpContextAccessor.HttpContext.GetTokenAsync("refresh_token");
             

            var jwtSecurityToken = _tokenService.ParseToken(token);
            var sub = jwtSecurityToken.Claims.Single(claim => claim.Type == "sub").Value;
            var tokenp = await _tokenService.GenerateToken(sub, string.Empty, string.Empty);

            logger.LogInformation($"{sub} logged in, Token: {token}, TokenP: {tokenp}");

            await _tokenService.SetGetTokenPSubAsync(tokenp, sub, cancellationToken);
            await _tokenService.SetGetSubTokenAsync(sub, token, cancellationToken);
            await _tokenService.SetGetSubTokenPAsync(sub, tokenp, cancellationToken); 
            await _tokenService.SetGetTokenPTokenAsync(tokenp, token, cancellationToken);
            await _tokenService.SetGetSubIdTokenAsync(sub, idToken!, cancellationToken);
            await _tokenService.SetGetTokenPRefreshTokenAsync(tokenp, refreshToken!, cancellationToken);

            try
            {
                await _userService.GetUserByExternalIdAsync(sub, cancellationToken);
            }
            catch (NotFoundException)
            {
                var customerProfile = await _sessionService.GetCurrentUserProfileAsync(cancellationToken);

                if (customerProfile is not null)
                {
                    var user = new UserDto
                    {
                        ExternalId = sub,
                        Username = customerProfile.Username,
                        Email = customerProfile.Email,
                        PhoneNumber = customerProfile.PhoneNumber,
                        Name = customerProfile.Name,
                        Family = customerProfile.Family
                    };
                    await _userService.CreateLocallyAsync(user, cancellationToken);
                } 
            }

            var baseUri = new Uri(_applicationSettings.Value.PlainBridgeWebUrl);
            var signinPage = new Uri(baseUri, _applicationSettings.Value.PlainBridgeWebRedirectPage.ToString());
            var uri = new Uri(signinPage, $"?access_token={tokenp}");
            _httpContextAccessor.HttpContext.Response.Redirect(uri.ToString(), true);
            return Task.FromResult(0);
        });
    }
}
