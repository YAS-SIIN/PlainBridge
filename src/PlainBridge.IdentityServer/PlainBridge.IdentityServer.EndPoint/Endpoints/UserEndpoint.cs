using Duende.IdentityModel;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PlainBridge.IdentityServer.EndPoint.Domain.Entities;
using PlainBridge.IdentityServer.EndPoint.DTOs;
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
        var app = builder.MapGroup("User");

        // GetAllAsync
        app.MapPost("", async ([FromBody] UserDto model, UserManager<IdentityUser> _userManager, CancellationToken cancellationToken) =>
        {

            var user = new ApplicationUser { UserName = model.Username, Email = model.Email, PhoneNumber = model.PhoneNumber };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded) throw new ApplicationException("User creation failed");

            result = await _userManager.AddClaimsAsync(user, new List<Claim>
                {
                    new Claim(JwtClaimTypes.GivenName, model.Name),
                    new Claim(JwtClaimTypes.FamilyName, model.Family),
                    new Claim(JwtClaimTypes.PreferredUserName, model.Username),
                    new Claim(JwtClaimTypes.PhoneNumber, model.PhoneNumber ?? string.Empty),
                    new Claim(JwtClaimTypes.Name, $"{model.Name} {model.Family}"),
                    new Claim(JwtClaimTypes.Email, $"{model.Email}"),
                    new Claim(JwtClaimTypes.Role, "simpleUser")
                });


            if (!result.Succeeded) throw new ApplicationException("Adding user claims failed");

            return Results.Ok(ResultDto<string>.ReturnData(
                user.Id,
                ResultCodeEnum.Success,
                ResultCodeEnum.Success.ToDisplayName()
            ));
        }).WithName("CreateUser");


        app.MapPatch("", async ([FromBody] UserDto model, UserManager<IdentityUser> _userManager, CancellationToken cancellationToken) =>
        {
            var user = await _userManager.FindByIdAsync(model.UserId.ToString());
            if (user == null) throw new NotFoundException("User not found");

            var claims = await _userManager.GetClaimsAsync(user);

            await _userManager.ReplaceClaimAsync(user, claims.Single(x => x.Type == JwtClaimTypes.GivenName), new Claim(JwtClaimTypes.GivenName, model.Name));
            await _userManager.ReplaceClaimAsync(user, claims.Single(x => x.Type == JwtClaimTypes.FamilyName), new Claim(JwtClaimTypes.FamilyName, model.Family));
            await _userManager.ReplaceClaimAsync(user, claims.Single(x => x.Type == JwtClaimTypes.PhoneNumber), new Claim(JwtClaimTypes.PhoneNumber, model.PhoneNumber ?? string.Empty));
            await _userManager.ReplaceClaimAsync(user, claims.Single(x => x.Type == JwtClaimTypes.Name), new Claim(JwtClaimTypes.Name, $"{model.Name} {model.Family}")); 

            return Results.Ok(ResultDto<string>.ReturnData(
                user.Id,
                ResultCodeEnum.Success,
                ResultCodeEnum.Success.ToDisplayName()
            ));

        }).WithName("UpdateUser");


        app.MapPatch("ChangePassword", async ([FromBody] ChangeUserPasswordDto model, UserManager<IdentityUser> _userManager, CancellationToken cancellationToken) =>
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
