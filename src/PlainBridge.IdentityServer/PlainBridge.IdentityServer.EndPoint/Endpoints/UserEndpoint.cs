using Duende.Bff;
using Duende.IdentityModel;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PlainBridge.IdentityServer.EndPoint.Application.Services.User;
using PlainBridge.IdentityServer.EndPoint.Domain.Entities;
using PlainBridge.IdentityServer.EndPoint.DTOs;
using PlainBridge.SharedApplication.DTOs;
using PlainBridge.SharedApplication.Enums;
using PlainBridge.SharedApplication.Extensions;


namespace PlainBridge.IdentityServer.EndPoint.Endpoints;

public static class UserEndpoint
{
    public static void MapUserEndpoint(this IEndpointRouteBuilder builder)
    {
        var app = builder.MapGroup("User");

        // GetAllAsync
        app.MapPost("", async ([FromBody] UserRequestDto model, IUserServices _userServices, CancellationToken cancellationToken) =>
        {

            var result = await _userServices.CreateAsync(model);

            return Results.Ok(ResultDto<string>.ReturnData(
                result.Id,
                ResultCodeEnum.Success,
                ResultCodeEnum.Success.ToDisplayName()
            ));
        }).WithName("CreateUser");


        app.MapPatch("", async ([FromBody] UserRequestDto model, IUserServices _userServices, CancellationToken cancellationToken) =>
        {

            var result = await _userServices.UpdateAsync(model);

            return Results.Ok(ResultDto<string>.ReturnData(
                result.Id,
                ResultCodeEnum.Success,
                ResultCodeEnum.Success.ToDisplayName()
            ));

        }).WithName("UpdateUser");


        app.MapPatch("ChangePassword", async ([FromBody] ChangeUserPasswordRequestDto model, IUserServices _userServices, CancellationToken cancellationToken) =>
        {

            var result = await _userServices.ChangePasswordAsync(model);

            return Results.Ok(ResultDto<string>.ReturnData(
                result.Id,
                ResultCodeEnum.Success,
                ResultCodeEnum.Success.ToDisplayName()
            ));
        }).WithName("ChangePassword");

    }
}
