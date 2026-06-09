
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging; 
using PlainBridge.Api.Infrastructure.Persistence.Data.Context;
using PlainBridge.SharedApplication.DTOs;
using PlainBridge.SharedApplication.Enums;
using PlainBridge.SharedApplication.Mediator;

namespace PlainBridge.Api.Application.UseCases.ServerApplication.Queries;

public class GetAllServerApplicationsQueryHandler(ILogger<GetAllServerApplicationsQueryHandler> _logger, MainDbContext _dbContext) : IRequestHandler<GetAllServerApplicationsQuery, List<ServerApplicationDto>>
{
    public async Task<List<ServerApplicationDto>> Handle(GetAllServerApplicationsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all server applications.");
        var serverApplication = await _dbContext.ServerApplications.Include(a => a.User).AsNoTracking().ToListAsync(cancellationToken);

        return serverApplication.Select(x => new ServerApplicationDto
        {
            Id = x.Id,
            AppId = x.AppId.ViewId,
            ServerApplicationAppId = x.ServerApplicationViewId != Guid.Empty ? x.ServerApplicationViewId.ToString() : null,
            UserId = x.UserId,
            UserName = x.User.UserName.UserNameValue,
            Name = x.Name,
            InternalPort = x.InternalPort.Port,
            Description = x.Description,
            State = (RowStateEnum)x.State,
        }).ToList();
    }
}
