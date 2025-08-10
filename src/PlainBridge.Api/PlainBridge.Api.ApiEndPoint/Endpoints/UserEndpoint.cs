
using Microsoft.AspNetCore.Mvc;

using PlainBridge.Api.Application.DTOs; 
using PlainBridge.Api.Application.Services.Session;
using PlainBridge.Api.Application.Services.User;
using PlainBridge.SharedApplication.DTOs;
using PlainBridge.SharedApplication.Enums;
using PlainBridge.SharedApplication.Exceptions;
using PlainBridge.SharedApplication.Extentions;

namespace PlainBridge.Api.ApiEndPoint.Endpoints;

public static class UserEndpoint
{
    public static void MapUserEndpoint(this IEndpointRouteBuilder builder)
    {
        var app = builder.MapGroup("User")
            .RequireAuthorization(new Microsoft.AspNetCore.Authorization.AuthorizeAttribute { AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme });
        // GetAllAsync
        app.MapGet("", async (CancellationToken cancellationToken, ILoggerFactory loggerFactory, IUserService userService) =>
        { 
            var data = await userService.GetAllAsync(cancellationToken);
            return Results.Ok(ResultDto<IList<UserDto>>.ReturnData(
                data,
                ResultCodeEnum.Success,
                ResultCodeEnum.Success.ToDisplayName()
            ));
        }).WithName("GetAllUsers");


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
        }).WithName("GetCurrentUser");
         
        // CreateAsync
        app.MapPost("", async ([FromBody] UserDto user, CancellationToken cancellationToken, ILoggerFactory loggerFactory, IUserService userService) =>
        { 
            var id = await userService.CreateAsync(user, cancellationToken);
            return Results.Created($"/user/{id}", ResultDto<Guid>.ReturnData(
                id,
                ResultCodeEnum.Success,
                ResultCodeEnum.Success.ToDisplayName()
            ));
        }).WithName("CreateUser");

        // ChangePasswordAsync
        app.MapPatch("ChangePassword", async ([FromBody] ChangeUserPasswordDto changeUserPassword, CancellationToken cancellationToken, ILoggerFactory loggerFactory, IUserService userService, ISessionService sessionService) =>
        {
            var user = await sessionService.GetCurrentUserAsync(cancellationToken);
            if (user == null)
                throw new NotFoundException("user");

            changeUserPassword.Id = user.Id;
            await userService.ChangePasswordAsync(changeUserPassword, cancellationToken);
            return Results.Ok(ResultDto<object>.ReturnData(
                null,
                ResultCodeEnum.Success,
                ResultCodeEnum.Success.ToDisplayName()
            ));
        }).WithName("ChangePassword");


        // UpdateAsync
        app.MapPatch("{id:long}", async ([FromBody] UserDto user, CancellationToken cancellationToken, ILoggerFactory loggerFactory, IUserService userService, ISessionService sessionService) =>
        {
            var currentUser = await sessionService.GetCurrentUserAsync(cancellationToken);
            if (currentUser == null)
                throw new NotFoundException("user");

            user.Id = currentUser.Id;
            await userService.UpdateAsync(user, cancellationToken);
            return Results.Ok(ResultDto<object>.ReturnData(
                null,
                ResultCodeEnum.Success,
                ResultCodeEnum.Success.ToDisplayName()
            ));
        }).WithName("UpdateUser");
    }
}
