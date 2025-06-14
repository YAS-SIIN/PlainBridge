using PlainBridge.Api.Application.DTOs;
using PlainBridge.SharedApplication.DTOs;

namespace PlainBridge.Api.Application.Services.ServerApplication
{
    public interface IServerApplicationService
    {
        Task<Guid> CreateAsync(ServerApplicationDto serverApplication, CancellationToken cancellationToken);
        Task DeleteAsync(long id, CancellationToken cancellationToken);
        Task<IList<ServerApplicationDto>> GetAllAsync(CancellationToken cancellationToken);
        Task<ServerApplicationDto> GetByIdAsync(long id, CancellationToken cancellationToken);
        Task UpdateAsync(ServerApplicationDto serverApplication, CancellationToken cancellationToken);
    }
}