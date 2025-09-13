using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using PlainBridge.Api.ApiEndPoint.Abstractions;
using PlainBridge.Api.Application.Features.User.Commands;
using PlainBridge.Api.Application.Features.User.Queries;
using PlainBridge.Api.Application.Services.Session;
using PlainBridge.Api.Application.Services.Token; 
using PlainBridge.Api.Infrastructure.DTOs;
using PlainBridge.Api.Infrastructure.Persistence.Cache;
using PlainBridge.SharedApplication.Exceptions;
using PlainBridge.SharedApplication.Mediator;

namespace PlainBridge.Api.ApiEndPoint.Endpoints;

public class LoginEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        var app = builder.MapGroup("");
        app.MapBffManagementEndpoints();


        app.MapGet("/", async (CancellationToken cancellationToken, ILoggerFactory loggerFactory, IHttpContextAccessor _httpContextAccessor, ITokenService _tokenService, ICacheManagement _cacheManagement, ISessionService _sessionService, IMediator _mediator, IOptions<ApplicationSettings> _applicationSettings) =>
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

            await _cacheManagement.SetGetTokenPSubAsync(tokenp, sub, cancellationToken);
            await _cacheManagement.SetGetSubTokenAsync(sub, token, cancellationToken);
            await _cacheManagement.SetGetSubTokenPAsync(sub, tokenp, cancellationToken);
            await _cacheManagement.SetGetTokenPTokenAsync(tokenp, token, cancellationToken);
            await _cacheManagement.SetGetSubIdTokenAsync(sub, idToken!, cancellationToken);
            await _cacheManagement.SetGetTokenPRefreshTokenAsync(tokenp, refreshToken!, cancellationToken);

            try
            {
                await _mediator.Send(new GetUserByExternalIdQuery { ExternalId = sub }, cancellationToken);
            }
            catch (NotFoundException)
            {
                var customerProfile = await _sessionService.GetCurrentUserProfileAsync(cancellationToken);

                if (customerProfile is not null)
                {
                    var user = new CreateUserCommand
                    {
                        ExternalId = sub,
                        UserName = customerProfile.Username,
                        Email = customerProfile.Email,
                        PhoneNumber = customerProfile.PhoneNumber,
                        Name = customerProfile.Name,
                        Family = customerProfile.Family
                    };
                    await _mediator.Send(user, cancellationToken);
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
