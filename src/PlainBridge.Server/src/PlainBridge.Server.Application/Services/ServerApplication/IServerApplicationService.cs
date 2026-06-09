using PlainBridge.Shared.Application.DTOs;

namespace PlainBridge.Server.Application.Services.ServerApplication;

public interface IServerApplicationService
{ 
    Task UpdateServerApplicationAsync(CancellationToken cancellationToken);
}