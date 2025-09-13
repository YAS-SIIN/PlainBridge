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
        _logger.LogInformation("Creating user: {Username}, {Email}", request.Username, request.Email);
        var existedUser = await _dbContext.Users.AnyAsync(x => x.Username == request.Username || x.Email == request.Email, cancellationToken);
        if (existedUser)
        {
            _logger.LogWarning("User already exists with username: {Username} or email: {Email}", request.Username, request.Email);
            throw new DuplicatedException($"{request.Username} or {request.Email}");
        }
        var newUser = Domain.UserAggregate.User.Create(request.ExternalId, request.Username, request.Email, request.PhoneNumber, request.Name, request.Family, request.ExternalId);

        var createdUser = await _dbContext.Users.AddAsync(newUser, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        newUser = createdUser.Entity;
        _logger.LogInformation("User created successfully: {Username}", request.Username);
        return newUser.AppId.ViewId;
    }
}
