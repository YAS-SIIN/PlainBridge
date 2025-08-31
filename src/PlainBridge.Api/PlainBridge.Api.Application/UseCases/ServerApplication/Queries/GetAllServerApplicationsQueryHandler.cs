
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PlainBridge.Api.Infrastructure.Data.Context;
using PlainBridge.Api.Infrastructure.Messaging;
using PlainBridge.SharedApplication.DTOs;
using PlainBridge.SharedApplication.Enums;
using PlainBridge.SharedApplication.Mediator;

namespace PlainBridge.Api.Application.UseCases.ServerApplication.Queries;

public class GetAllServerApplicationsQueryHandler(ILogger<GetAllServerApplicationsQueryHandler> _logger, MainDbContext _dbContext) : IRequestHandler<GetAllServerApplicationsQuery, List<ServerApplicationDto>>
{
    public async Task<List<ServerApplicationDto>> Handle(GetAllServerApplicationsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling GetAllServerApplicationsQuery.");
        var list = await _dbContext.ServerApplications.Include(a => a.User).AsNoTracking().ToListAsync(cancellationToken);
        return list.Select(x => new ServerApplicationDto
        {
            Id = x.Id,
            AppId = x.AppId.ViewId,
            UserId = x.UserId,
            UserName = x.User.Username,
            Name = x.Name,
            Domain = x.Domain.HostDomainName,
            InternalUrl = x.InternalUrl.InternalUrlValue,
            Description = x.Description,
            State = (RowStateEnum)x.State
        }).ToList();
    }
}
