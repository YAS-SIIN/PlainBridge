using PlainBridge.SharedApplication.DTOs;

namespace PlainBridge.Server.Application.Services.ServerApplication;

public interface IServerApplicationService
{ 
    Task UpdateServerApplicationAsync(CancellationToken cancellationToken);
}