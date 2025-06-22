using Duende.IdentityModel;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using PlainBridge.IdentityServer.EndPoint.Dto;
using PlainBridge.SharedApplication.DTOs;
using PlainBridge.SharedApplication.Enums;
using PlainBridge.SharedApplication.Exceptions;
using PlainBridge.SharedApplication.Extentions;

using System.Security.Claims; 

namespace PlainBridge.IdentityServer.EndPoint.Endpoints;

public static class UserEndpoint
{
    public static void MapUserEndpoint(this IEndpointRouteBuilder builder)
    {
        var app = builder.MapGroup("UserEndpoint");

        // GetAllAsync
        app.MapPost("", async ([FromBody] UserInputDto model, UserManager<IdentityUser> _userManager, CancellationToken cancellationToken) =>
        {

            var user = new IdentityUser { UserName = model.Username, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded) throw new ApplicationException("User creation failed");

            result = await _userManager.AddClaimsAsync(user, new List<Claim>
                {
                    new Claim(JwtClaimTypes.GivenName, model.Name),
                    new Claim(JwtClaimTypes.FamilyName, model.Family),
                    new Claim(JwtClaimTypes.Name, $"{model.Name} {model.Family}"),
                    new Claim(JwtClaimTypes.Role, "customer")
                });


            if (!result.Succeeded) throw new ApplicationException("Adding user claims failed");

            return Results.Ok(ResultDto<string>.ReturnData(
                user.Id,
                ResultCodeEnum.Success,
                ResultCodeEnum.Success.ToDisplayName()
            ));
        }).WithName("CreateUser");


        app.MapPatch("", async ([FromBody] UserInputDto model, UserManager<IdentityUser> _userManager, CancellationToken cancellationToken) =>
        {
            var user = await _userManager.FindByIdAsync(model.UserId.ToString());
            if (user == null) throw new NotFoundException("User not found");


            var claims = await _userManager.GetClaimsAsync(user);

            await _userManager.ReplaceClaimAsync(user, claims.Single(x => x.Type == JwtClaimTypes.GivenName), new Claim(JwtClaimTypes.GivenName, model.Name));
            await _userManager.ReplaceClaimAsync(user, claims.Single(x => x.Type == JwtClaimTypes.FamilyName), new Claim(JwtClaimTypes.FamilyName, model.Family));
            await _userManager.ReplaceClaimAsync(user, claims.Single(x => x.Type == JwtClaimTypes.Name), new Claim(JwtClaimTypes.Name, $"{model.Name} {model.Family}"));

            return Results.Ok(ResultDto<string>.ReturnData(
                user.Id,
                ResultCodeEnum.Success,
                ResultCodeEnum.Success.ToDisplayName()
            ));

        }).WithName("UpdateUser");


        app.MapPatch("ChangePassword", async ([FromBody] ChangePasswordDto model, UserManager<IdentityUser> _userManager, CancellationToken cancellationToken) =>
        {
            var user = await _userManager.FindByIdAsync(model.UserId.ToString());

            if (user == null) throw new NotFoundException("User not found");

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (!result.Succeeded) throw new ApplicationException("Changing password failed");

            return Results.Ok(ResultDto<string>.ReturnData(
                user.Id,
                ResultCodeEnum.Success,
                ResultCodeEnum.Success.ToDisplayName()
            ));
        }).WithName("ChangePassword");

    }
}
