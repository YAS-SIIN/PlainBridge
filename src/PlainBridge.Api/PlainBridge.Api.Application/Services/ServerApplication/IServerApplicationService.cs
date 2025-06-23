using PlainBridge.Api.Application.DTOs;
using PlainBridge.SharedApplication.DTOs;

namespace PlainBridge.Api.Application.Services.ServerApplication
{
    public interface IServerApplicationService
    {
        Task<Guid> CreateAsync(ServerApplicationDto serverApplication, CancellationToken cancellationToken);
        Task DeleteAsync(long id, CancellationToken cancellationToken);
        Task<IList<ServerApplicationDto>> GetAllAsync(long userId, CancellationToken cancellationToken);
        Task<ServerApplicationDto> GetAsync(long id, long userId, CancellationToken cancellationToken);
        Task UpdateAsync(ServerApplicationDto serverApplication, CancellationToken cancellationToken);
    }
}