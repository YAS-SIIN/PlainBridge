using Duende.IdentityModel;
using Microsoft.AspNetCore.Identity;
using PlainBridge.IdentityServer.EndPoint.Domain.Entities;
using PlainBridge.IdentityServer.EndPoint.DTOs;
using PlainBridge.SharedApplication.Exceptions;
using System.Security.Claims;

namespace PlainBridge.IdentityServer.EndPoint.Application.Services.User;

public class UserServices(ILogger<UserServices> _logger, UserManager<ApplicationUser> _userManager) : IUserServices
{

    public async Task<ApplicationUser> CreateAsync(UserRequestDto model)
    {
        var user = new ApplicationUser { UserName = model.UserName, Email = model.Email, PhoneNumber = model.PhoneNumber };
        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
        {
            _logger.LogError("User creation failed: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
            throw new ApplicationException("User creation failed");
        }

        result = await _userManager.AddClaimsAsync(user, new List<Claim>
                {
                    new Claim(JwtClaimTypes.GivenName, model.Name),
                    new Claim(JwtClaimTypes.FamilyName, model.Family),
                    new Claim(JwtClaimTypes.PreferredUserName, model.UserName),
                    new Claim(JwtClaimTypes.PhoneNumber, model.PhoneNumber ?? string.Empty),
                    new Claim(JwtClaimTypes.Name, $"{model.Name} {model.Family}"),
                    new Claim(JwtClaimTypes.Email, $"{model.Email}"),
                    new Claim(JwtClaimTypes.Role, "simpleUser")
                });

        if (!result.Succeeded)
        {
            _logger.LogError("User creation failed: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
            throw new ApplicationException("Adding user claims failed");
        }


        _logger.LogInformation("User {Username} created successfully.", model.UserName);


        return user;
    }

    public async Task<ApplicationUser> UpdateAsync(UserRequestDto model)
    {
        var user = await _userManager.FindByIdAsync(model.UserId.ToString());
        if (user is null)
        {
            _logger.LogWarning("User with ID {UserId} not found for update.", model.UserId);
            throw new NotFoundException("User not found");
        }

        var claims = await _userManager.GetClaimsAsync(user);

        await _userManager.ReplaceClaimAsync(user, claims.Single(x => x.Type == JwtClaimTypes.GivenName), new Claim(JwtClaimTypes.GivenName, model.Name));
        await _userManager.ReplaceClaimAsync(user, claims.Single(x => x.Type == JwtClaimTypes.FamilyName), new Claim(JwtClaimTypes.FamilyName, model.Family));
        await _userManager.ReplaceClaimAsync(user, claims.Single(x => x.Type == JwtClaimTypes.PhoneNumber), new Claim(JwtClaimTypes.PhoneNumber, model.PhoneNumber ?? string.Empty));
        await _userManager.ReplaceClaimAsync(user, claims.Single(x => x.Type == JwtClaimTypes.Name), new Claim(JwtClaimTypes.Name, $"{model.Name} {model.Family}"));
    
        return user;

    }

    public async Task<ApplicationUser> ChangePasswordAsync(ChangeUserPasswordRequestDto model)
    {
        var user = await _userManager.FindByIdAsync(model.UserId.ToString());

        if (user is null)
        {
            _logger.LogWarning("User with ID {UserId} not found for update.", model.UserId);
            throw new NotFoundException("User not found");
        }

        var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
        if (!result.Succeeded)
        {
            _logger.LogError("Password change failed for user ID {UserId}: {Errors}", model.UserId, string.Join(", ", result.Errors.Select(e => e.Description)));
            throw new ApplicationException("Changing password failed");
        }
        return user;

    }



}
