using PlainBridge.Api.Application.DTOs;
using PlainBridge.SharedApplication.DTOs;

namespace PlainBridge.Api.Application.Services.HostApplication;

public interface IHostApplicationService
{
        Task<Guid> CreateAsync(HostApplicationDto hostApplication, CancellationToken cancellationToken);
        Task DeleteAsync(long id, CancellationToken cancellationToken);
        Task<IList<HostApplicationDto>> GetAllAsync(CancellationToken cancellationToken);
        Task<HostApplicationDto?> GetAsync(long id, long userId, CancellationToken cancellationToken);
        Task UpdateAsync(HostApplicationDto hostApplication, CancellationToken cancellationToken);
}