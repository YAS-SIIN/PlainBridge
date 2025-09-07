 
using System.IdentityModel.Tokens.Jwt; 
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PlainBridge.Api.Application.Services.ServerApplication;
using PlainBridge.Api.Application.Services.Session;
using PlainBridge.Api.Application.Services.Token;
using PlainBridge.Api.Application.Services.User;
using PlainBridge.Api.Infrastructure.Messaging;
using PlainBridge.SharedApplication.Extensions;
using PlainBridge.Api.Infrastructure;

namespace PlainBridge.Api.Application;


public static class DependencyResolver
{
    public static IServiceCollection AddApiApplicationProjectServices(this IServiceCollection services)
    {

        // Add services to the container 


        services.AddScoped<IServerApplicationService, ServerApplicationService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ISessionService, SessionService>();
        services.AddScoped<ITokenService, TokenService>();

         
        services.AddMediator(typeof(DependencyResolver).Assembly);



        return services;
    }

}
