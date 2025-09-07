
using System.Text.Json;
using Duende.IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using PlainBridge.Api.Infrastructure.DTOs;
using PlainBridge.Client.Application.Services.Signal;
using PlainBridge.SharedApplication.DTOs;



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

            var fileName = "profile";

            if (File.Exists(fileName))
            {
                var content = File.ReadAllText(fileName);
                var profile = JsonSerializer.Deserialize<UserProfileViewDto>(content);

                if (profile is null)
                {
                    _logger.LogError("Profile is null");
                    throw new ApplicationException("Profile is null");
                }

                return "Authorized";
            }

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

            File.WriteAllText(fileName, result);
            _signalService.Set();

            return result;
        });
    }
}
