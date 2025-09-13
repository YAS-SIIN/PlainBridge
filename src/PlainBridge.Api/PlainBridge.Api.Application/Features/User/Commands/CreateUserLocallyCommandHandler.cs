using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PlainBridge.Api.Infrastructure.Persistence.Data.Context;
using PlainBridge.SharedApplication.Exceptions;
using PlainBridge.SharedApplication.Mediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlainBridge.Api.Application.Features.User.Commands;

public class CreateUserLocallyCommandHandler(ILogger<CreateUserLocallyCommandHandler> _logger, MainDbContext _dbContext) : IRequestHandler<CreateUserLocallyCommand, Guid>
{
    public async Task<Guid> Handle(CreateUserLocallyCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating user: {Username}, {Email}", request.UserName, request.Email);
        var existedUser = await _dbContext.Users.AnyAsync(x => x.UserName.UserNameValue == request.UserName || x.Email.Equals(request.Email), cancellationToken);
        if (existedUser)
        {
            _logger.LogWarning("User already exists with username: {Username} or email: {Email}", request.UserName, request.Email);
            throw new DuplicatedException($"{request.UserName} or {request.Email}");
        }
        var newUser = Domain.UserAggregate.User.Create(request.ExternalId, request.UserName, request.Email, request.PhoneNumber, request.Name, request.Family, request.ExternalId);

        var createdUser = await _dbContext.Users.AddAsync(newUser, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        newUser = createdUser.Entity;
        _logger.LogInformation("User created successfully: {Username}", request.UserName);
        return newUser.AppId.ViewId;
    }
}
