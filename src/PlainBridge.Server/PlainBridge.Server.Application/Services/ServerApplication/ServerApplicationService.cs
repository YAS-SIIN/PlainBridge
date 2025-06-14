

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using PlainBridge.Api.Application.DTOs; 
using PlainBridge.Api.Application.Handler.Bus;
using PlainBridge.Api.Infrastructure.Data.Context;
 

namespace PlainBridge.Server.Application.Services.ServerApplication;

public class ServerApplicationService
{
    private readonly ILogger<ServerApplicationService> _logger; 

    public ServerApplicationService(ILogger<ServerApplicationService> logger)
    {
        _logger = logger; 
    }

}
