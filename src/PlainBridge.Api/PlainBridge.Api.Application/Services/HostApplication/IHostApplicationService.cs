using PlainBridge.Api.Application.DTOs;

namespace PlainBridge.Api.Application.Services.HostApplication;

public interface IHostApplicationService
{
    Task<Guid> CreateAsync(HostApplicationDto hostApplication, CancellationToken cancellationToken);
    Task DeleteAsync(long id, CancellationToken cancellationToken);
    Task<IList<HostApplicationDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<HostApplicationDto?> GetByIdAsync(long id, CancellationToken cancellationToken);
    Task UpdateAsync(HostApplicationDto hostApplication, CancellationToken cancellationToken);
}