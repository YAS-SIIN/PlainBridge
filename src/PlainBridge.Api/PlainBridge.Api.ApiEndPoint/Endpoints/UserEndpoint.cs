
using Microsoft.AspNetCore.Mvc;
using PlainBridge.Api.ApiEndPoint.Abstractions;
using PlainBridge.Api.Application.DTOs;
using PlainBridge.Api.Application.Features.User.Commands;
using PlainBridge.Api.Application.Features.User.Queries;
using PlainBridge.Api.Application.Services.Session; 
using PlainBridge.SharedApplication.DTOs;
using PlainBridge.SharedApplication.Enums;
using PlainBridge.SharedApplication.Exceptions;
using PlainBridge.SharedApplication.Extensions;
using PlainBridge.SharedApplication.Mediator;

namespace PlainBridge.Api.ApiEndPoint.Endpoints;

public class UserEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        var app = builder.MapGroup("api/User");
        // GetAllAsync
        app.MapGet("", async (CancellationToken cancellationToken, ILoggerFactory loggerFactory, IMediator mediator) =>
        {
            var data = await mediator.Send(new GetAllUsersQuery(), cancellationToken);
            return Results.Ok(ResultDto<IList<UserDto>>.ReturnData(
                data,
                ResultCodeEnum.Success,
                ResultCodeEnum.Success.ToDisplayName()
            ));
        }).WithName("GetAllUsers").RequireAuthorization(new Microsoft.AspNetCore.Authorization.AuthorizeAttribute { AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme });


        // GetCurrentUser
        app.MapGet("GetCurrentUser", async (CancellationToken cancellationToken, ILoggerFactory loggerFactory, ISessionService sessionService) =>
        {
            var user = await sessionService.GetCurrentUserAsync(cancellationToken);
            if (user == null)
                throw new NotFoundException("user");

            return Results.Ok(ResultDto<UserDto>.ReturnData(
                user,
                ResultCodeEnum.Success,
                ResultCodeEnum.Success.ToDisplayName()
            ));
        }).WithName("GetCurrentUser")
        .RequireAuthorization(new Microsoft.AspNetCore.Authorization.AuthorizeAttribute { AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme });

        // CreateAsync
        app.MapPost("", async ([FromBody] CreateUserCommand user, CancellationToken cancellationToken, ILoggerFactory loggerFactory, IMediator mediator) =>
        {
            var id = await mediator.Send(user, cancellationToken);
            return Results.Created($"/user/{id}", ResultDto<Guid>.ReturnData(
                id,
                ResultCodeEnum.Success,
                ResultCodeEnum.Success.ToDisplayName()
            ));
        }).WithName("CreateUser");

        // ChangePasswordAsync
        app.MapPatch("ChangePassword", async ([FromBody] ChangeUserPasswordCommand changeUserPassword, CancellationToken cancellationToken, ILoggerFactory loggerFactory, ISessionService sessionService, IMediator mediator) =>
        {
            var user = await sessionService.GetCurrentUserAsync(cancellationToken);
            if (user == null)
                throw new NotFoundException("user");

            changeUserPassword.Id = user!.Id!;
            await mediator.Send(changeUserPassword, cancellationToken);
            return Results.Ok(ResultDto<object>.ReturnData(
                null,
                ResultCodeEnum.Success,
                ResultCodeEnum.Success.ToDisplayName()
            ));
        }).WithName("ChangePassword")
        .RequireAuthorization(new Microsoft.AspNetCore.Authorization.AuthorizeAttribute { AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme });


        // UpdateAsync
        app.MapPatch("{id:long}", async ([FromBody] UpdateUserCommand user, CancellationToken cancellationToken, ILoggerFactory loggerFactory, IMediator mediator, ISessionService sessionService) =>
        {
            var currentUser = await sessionService.GetCurrentUserAsync(cancellationToken);
            if (currentUser == null)
                throw new NotFoundException("user");

            user.Id = currentUser.Id;
            await mediator.Send(user, cancellationToken);
            return Results.Ok(ResultDto<object>.ReturnData(
                null,
                ResultCodeEnum.Success,
                ResultCodeEnum.Success.ToDisplayName()
            ));
        }).WithName("UpdateUser")
        .RequireAuthorization(new Microsoft.AspNetCore.Authorization.AuthorizeAttribute { AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme });


        // ChangePasswordAsync
        app.MapPost("RefreshToken", async (CancellationToken cancellationToken, ILoggerFactory loggerFactory, ISessionService sessionService) =>
        {
            await sessionService.RefreshTokenAsync(cancellationToken);
           
            return Results.Ok(ResultDto<object>.ReturnData(
                null,
                ResultCodeEnum.Success,
                ResultCodeEnum.Success.ToDisplayName()
            ));
        }).WithName("RefreshToken")
        .RequireAuthorization(new Microsoft.AspNetCore.Authorization.AuthorizeAttribute { AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme });

    }
}
