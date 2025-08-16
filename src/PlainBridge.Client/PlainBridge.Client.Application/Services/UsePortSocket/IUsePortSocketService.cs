using PlainBridge.SharedApplication.DTOs;

namespace PlainBridge.Client.Application.Services.UsePortSocket;

public interface IUsePortSocketService
{
    Task InitializeAsync(string username, List<ServerApplicationDto> serverApplications, CancellationToken cancellationToken);
}