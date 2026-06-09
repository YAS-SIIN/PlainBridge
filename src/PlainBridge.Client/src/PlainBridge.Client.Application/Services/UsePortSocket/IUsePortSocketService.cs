using PlainBridge.Shared.Application.DTOs;

namespace PlainBridge.Client.Application.Services.UsePortSocket;

public interface IUsePortSocketService
{
    Task InitializeAsync(string username, List<ServerApplicationDto> serverApplications, CancellationToken cancellationToken);
}