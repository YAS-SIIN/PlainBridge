
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PlainBridge.Api.Infrastructure.Data.Context;
using PlainBridge.Api.Infrastructure.Messaging;
using PlainBridge.SharedApplication.DTOs;
using PlainBridge.SharedApplication.Enums;
using PlainBridge.SharedApplication.Mediator;

namespace PlainBridge.Api.Application.UseCases.HostApplication.Queries;

public class GetAllHostApplicationsQueryHandler(ILogger<GetAllHostApplicationsQueryHandler> _logger, MainDbContext _dbContext) : IRequestHandler<GetAllHostApplicationsQuery, List<HostApplicationDto>>
{
    public async Task<List<HostApplicationDto>> Handle(GetAllHostApplicationsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling GetAllHostApplicationsQuery.");
        var list = await _dbContext.HostApplications.Include(a => a.User).AsNoTracking().ToListAsync(cancellationToken);
        return list.Select(x => new HostApplicationDto
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
