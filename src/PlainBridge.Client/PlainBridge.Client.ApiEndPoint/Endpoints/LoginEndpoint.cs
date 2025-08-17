
using Duende.IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PlainBridge.Api.Application.DTOs;
using PlainBridge.Api.Application.Services.Session;
using PlainBridge.Api.Application.Services.Token;
using PlainBridge.Api.Application.Services.User;
using PlainBridge.Client.Application.Services.Signal;
using PlainBridge.SharedApplication.DTOs;
using PlainBridge.SharedApplication.Enums;
using PlainBridge.SharedApplication.Exceptions;
using PlainBridge.SharedApplication.Extensions;



namespace PlainBridge.Client.ApiEndPoint.Endpoints;

public static class LoginEndpoint 
{
    public static void MapLoginEndpoint(this IEndpointRouteBuilder builder)
    {
        var app = builder.MapGroup("");
        app.MapBffManagementEndpoints();


        app.MapGet("/", static async (CancellationToken cancellationToken, HttpContext context, ILoggerFactory loggerFactory, IHttpContextAccessor httpContextAccessor, ISignalService _signalService, IOptions<ApplicationSettings> _applicationSettings) =>
        {
            var _logger = loggerFactory.CreateLogger(nameof(LoginEndpoint));


            if (!context!.User!.Identity!.IsAuthenticated)
            {
                context.Response.Redirect("/bff/login");
                return "";
            }

            var userId = httpContextAccessor!.HttpContext!.User.Claims.SingleOrDefault(claim => claim.Type == "sub");

            var token = await context.GetTokenAsync("access_token");
            var baseUri = new Uri(_applicationSettings.Value.PlainBridgeIdsUrl);
            var uri = new Uri(baseUri, "connect/userinfo");
            var userInfoRequest = new UserInfoRequest
            {
                Address = uri.ToString(),
                Token = token
            };

            var client = new HttpClient();
            var userInfoResponse = await client.GetUserInfoAsync(userInfoRequest);

            if (userInfoResponse.IsError)
            {
                context.Response.Redirect("/bff/login");
                return "";
            }

            var result = await userInfoResponse!.HttpResponse!.Content.ReadAsStringAsync();

            _signalService.Set();

            return result;
        });
    }
}
