

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using PlainBridge.Api.Application.DTOs;
using PlainBridge.Api.Infrastructure.Data.Context;

namespace PlainBridge.Api.Application.Services.ServerApplication;

public class ServerApplicationService
{
    private readonly ILogger<ServerApplicationService> _logger;
    private readonly MainDbContext _dbContext;

    public ServerApplicationService(ILogger<ServerApplicationService> logger, MainDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<IList<ServerApplicationDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        var serverApplication = await _dbContext.ServerApplications.AsNoTracking().ToListAsync(cancellationToken);

        return serverApplication.Select(x => new ServerApplicationDto
        {
            Id = x.Id,
            AppId = x.AppId,
            Name = x.Name,
            InternalPort = x.InternalPort
        }).ToList();
    }

    public async Task<ServerApplicationDto> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        var serverApp = await _dbContext.ServerApplications.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (serverApp == null)
            throw new ApplicationException("ServerApplication not found");
        return new ServerApplicationDto
        {
            Id = serverApp.Id,
            AppId = serverApp.AppId,
            Name = serverApp.Name,
            InternalPort = serverApp.InternalPort
        };
    }

    public async Task<Guid> CreateAsync(ServerApplicationDto serverApplication, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(serverApplication.Name))
            throw new ArgumentNullException(nameof(serverApplication.Name));
        if (serverApplication.InternalPort < 1 || serverApplication.InternalPort > 65535)
            throw new ApplicationException("Port range is not valid");

        var app = new Domain.Entities.ServerApplication
        {
            AppId = Guid.NewGuid(),
            Name = serverApplication.Name,
            InternalPort = serverApplication.InternalPort
        };

        await _dbContext.ServerApplications.AddAsync(app, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return app.AppId;
    }

    public async Task DeleteAsync(long id, CancellationToken cancellationToken)
    {
        var app = await _dbContext.ServerApplications.FindAsync(new object[] { id }, cancellationToken);
        if (app == null)
            throw new ApplicationException("ServerApplication not found");

        _dbContext.ServerApplications.Remove(app);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task PatchAsync(ServerApplicationDto serverApplication, CancellationToken cancellationToken)
    {
        var app = await _dbContext.ServerApplications.FindAsync(serverApplication.Id, cancellationToken);
        if (app == null)
            throw new ApplicationException("ServerApplication not found");

        app.InternalPort = serverApplication.InternalPort;
        app.Name = serverApplication.Name; 

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
