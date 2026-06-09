using PlainBridge.Shared.Application.DTOs;

namespace PlainBridge.Client.Application.Services.SharePortSocket;

public interface ISharePortSocketService
{
    Task InitializeAsync(string username, List<ServerApplicationDto> appProjects, CancellationToken cancellationToken);
}