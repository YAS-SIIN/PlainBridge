using PlainBridge.Api.Application.DTOs;

namespace PlainBridge.Api.Application.Services.User
{
    public interface IUserService
    {
        Task ChangePasswordAsync(ChangeUserPasswordDto changeUserPassword, CancellationToken cancellationToken);
        Task<Guid> CreateAsync(UserDto user, CancellationToken cancellationToken);
        Task<Guid> CreateLocallyAsync(UserDto user, CancellationToken cancellationToken);
        Task<IList<UserDto>> GetAllAsync(CancellationToken cancellationToken);
        Task<UserDto> GetUserByExternalIdAsync(string externalId, CancellationToken cancellationToken);
        Task UpdateAsync(UserDto user, CancellationToken cancellationToken);
    }
}