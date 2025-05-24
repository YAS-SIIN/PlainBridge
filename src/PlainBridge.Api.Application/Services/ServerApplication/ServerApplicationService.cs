

using Microsoft.Extensions.Logging;

using PlainBridge.Api.Infrastructure.Data.Context;

namespace PlainBridge.Api.Application.Services.ServerApplication;

public class ServerApplicationService
{ 
    private readonly ILogger<ServerApplicationService> _logger;
    private readonly MainDbContext _dbContext; 

    public ServerApplicationService(ILogger<ServerApplicationService> logger, MainDbContext dbContext )
    {
        _logger = logger;
        _dbContext = dbContext; 
    }

}
