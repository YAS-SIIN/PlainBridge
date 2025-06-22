using PlainBridge.Api.Application.DTOs;

namespace PlainBridge.Api.Application.Services.User
{
    public interface IUserService
    {
        Task ChangePasswordAsync(ChangeUserPasswordDto changeUserPassword, CancellationToken cancellationToken);
        Task<Guid> CreateAsync(UserDto user, CancellationToken cancellationToken);
        Task<List<UserDto>> GetAllAsync(CancellationToken cancellationToken);
        Task<UserDto> GetUserByExternalIdAsync(string externalId, CancellationToken cancellationToken);
        Task UpdateProfileAsync(UserDto user, CancellationToken cancellationToken);
    }
}