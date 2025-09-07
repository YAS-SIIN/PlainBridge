
using Microsoft.Extensions.DependencyInjection;
using PlainBridge.Api.Application.Services.Session;
using PlainBridge.Api.Application.Services.Token;
using PlainBridge.Api.Application.Services.User;
using PlainBridge.SharedApplication.Extensions;

namespace PlainBridge.Api.Application;


public static class DependencyResolver
{
    public static IServiceCollection AddApiApplicationProjectServices(this IServiceCollection services)
    {

        // Add services to the container 

         
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ISessionService, SessionService>();
        services.AddScoped<ITokenService, TokenService>();

         
        services.AddMediator(typeof(DependencyResolver).Assembly);



        return services;
    }

}
