 
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
        var app = builder.MapGroup("User");
        // GetAllAsync
        app.MapGet("", async (CancellationToken cancellationToken, IUserService userService) =>
        { 
            var data = await userService.GetAllAsync(cancellationToken);
            return Results.Ok(ResultDto<IList<UserDto>>.ReturnData(
                data,
                ResultCodeEnum.Success,
                ResultCodeEnum.Success.ToDisplayName()
            ));
        }).WithName("GetAllUsers");


        // GetCurrentUser
        app.MapGet("GetCurrentUser", async (long id, CancellationToken cancellationToken, ISessionService sessionService) =>
        {
            var user = await sessionService.GetCurrentUserAsync(cancellationToken);
            if (user == null)
                throw new NotFoundException("User");
             
            return Results.Ok(ResultDto<UserDto>.ReturnData(
                user,
                ResultCodeEnum.Success,
                ResultCodeEnum.Success.ToDisplayName()
            ));
        }).WithName("GetCurrentUser");


        // CreateAsync
        app.MapPost("", async (UserDto user, CancellationToken cancellationToken, IUserService userService) =>
        { 
            var id = await userService.CreateAsync(user, cancellationToken);
            return Results.Created($"/user/{id}", ResultDto<Guid>.ReturnData(
                id,
                ResultCodeEnum.Success,
                ResultCodeEnum.Success.ToDisplayName()
            ));
        }).WithName("CreateUser");

        // ChangePasswordAsync
        app.MapPatch("ChangePassword", async (ChangeUserPasswordDto changeUserPassword, CancellationToken cancellationToken, IUserService userService, ISessionService sessionService) =>
        {
            var user = await sessionService.GetCurrentUserAsync(cancellationToken);
            if (user == null)
                throw new NotFoundException("User");

            changeUserPassword.Id = user.Id;
            await userService.ChangePasswordAsync(changeUserPassword, cancellationToken);
            return Results.Ok(ResultDto<object>.ReturnData(
                null,
                ResultCodeEnum.Success,
                ResultCodeEnum.Success.ToDisplayName()
            ));
        }).WithName("ChangePassword");


        // UpdateAsync
        app.MapPatch("{id:long}", async (UserDto user, CancellationToken cancellationToken, IUserService userService, ISessionService sessionService) =>
        {
            var currentUser = await sessionService.GetCurrentUserAsync(cancellationToken);
            if (currentUser == null)
                throw new NotFoundException("User");

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
