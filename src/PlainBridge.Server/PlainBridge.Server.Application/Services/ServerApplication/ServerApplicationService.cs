 
using Microsoft.Extensions.Logging;
 
using PlainBridge.SharedApplication.DTOs;


namespace PlainBridge.Server.Application.Services.ServerApplication;

public class ServerApplicationService
{
    private readonly ILogger<ServerApplicationService> _logger; 

    public ServerApplicationService(ILogger<ServerApplicationService> logger)
    {
        _logger = logger; 
    }
 
}
