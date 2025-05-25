using PlainBridge.Api.Application.DTOs;

namespace PlainBridge.Api.Application.Services.ServerApplication
{
    public interface IServerApplicationService
    {
        Task<Guid> CreateAsync(ServerApplicationDto serverApplication, CancellationToken cancellationToken);
        Task DeleteAsync(long id, CancellationToken cancellationToken);
        Task<IList<ServerApplicationDto>> GetAllAsync(CancellationToken cancellationToken);
        Task<ServerApplicationDto> GetByIdAsync(long id, CancellationToken cancellationToken);
        Task PatchAsync(ServerApplicationDto serverApplication, CancellationToken cancellationToken);
    }
}