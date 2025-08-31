using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization; 
using Microsoft.AspNetCore.Mvc;
using PlainBridge.Api.ApiEndPoint.Abstractions; 
using PlainBridge.Api.Application.Services.HostApplication;
using PlainBridge.Api.Application.Services.Session;
using PlainBridge.Api.Application.UseCases.HostApplication.Commands;
using PlainBridge.Api.Application.UseCases.HostApplication.Queries; 
using PlainBridge.SharedApplication.DTOs;
using PlainBridge.SharedApplication.Enums;
using PlainBridge.SharedApplication.Exceptions;
using PlainBridge.SharedApplication.Extensions;
using PlainBridge.SharedApplication.Mediator;

namespace PlainBridge.Api.ApiEndPoint.Endpoints;

public class HostApplicationEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        var app = builder.MapGroup("api/HostApplication")
            .RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme});

        // GetAllAsync
        app.MapGet("", async (CancellationToken cancellationToken, ILoggerFactory loggerFactory, IHostApplicationService hostApplicationService, IMediator mediator) =>
        {
            var data = await mediator.Send(new GetAllHostApplicationsQuery(), cancellationToken);
            return Results.Ok(ResultDto<IList<HostApplicationDto>>.ReturnData(
                data,
                ResultCodeEnum.Success,
                ResultCodeEnum.Success.ToDisplayName()
            ));
        }).WithName("GetAllHostApplications");

        // GetAsync
        app.MapGet("{id:long}", async (long id, CancellationToken cancellationToken, ILoggerFactory loggerFactory, IHostApplicationService hostApplicationService, ISessionService sessionService, IMediator mediator) =>
        {
            var user = await sessionService.GetCurrentUserAsync(cancellationToken);
            if (user == null)
                throw new NotFoundException("user");

            var data = await mediator.Send(new GetHostApplicationQuery { Id = id, UserId = user.Id }, cancellationToken);
  
            return Results.Ok(ResultDto<HostApplicationDto>.ReturnData(
                data,
                ResultCodeEnum.Success,
                ResultCodeEnum.Success.ToDisplayName()
            ));
        }).WithName("GetHostApplication");

        // CreateAsync
        app.MapPost("", async ([FromBody] CreateHostApplicationCommand hostApplication, CancellationToken cancellationToken, ILoggerFactory loggerFactory, IHostApplicationService hostApplicationService, ISessionService sessionService, IMediator mediator) =>
        {
            var user = await sessionService.GetCurrentUserAsync(cancellationToken);
            if (user == null)
                throw new NotFoundException("user");

            hostApplication.UserId = user.Id;
            var id = await mediator.Send(hostApplication, cancellationToken);

            return Results.Created($"/hostApplication/{id}", ResultDto<Guid>.ReturnData(
                id,
                ResultCodeEnum.Success,
                ResultCodeEnum.Success.ToDisplayName()
            ));
        }).WithName("CreateHostApplication");

        // UpdateAsync
        app.MapPatch("{id:long}", async (long id, CancellationToken cancellationToken, [FromBody] UpdateHostApplicationCommand hostApplication, ILoggerFactory loggerFactory, IHostApplicationService hostApplicationService, ISessionService sessionService, IMediator mediator) =>
        {
            var user = await sessionService.GetCurrentUserAsync(cancellationToken);
            if (user == null)
                throw new NotFoundException("user");

            hostApplication.UserId = user.Id;
            hostApplication.Id = id;
            var updatedId = await mediator.Send(hostApplication, cancellationToken);
            return Results.Ok(ResultDto<Guid>.ReturnData(
                updatedId,
                ResultCodeEnum.Success,
                ResultCodeEnum.Success.ToDisplayName()
            ));
        }).WithName("UpdateHostApplication");
         
        // DeleteAsync
        app.MapDelete("{id:long}", async (long id, CancellationToken cancellationToken, ILoggerFactory loggerFactory, IHostApplicationService hostApplicationService, IMediator mediator) =>
        {
            await mediator.Send(new DeleteHostApplicationCommand { Id = id }, cancellationToken);
            await hostApplicationService.DeleteAsync(id, cancellationToken);
            return Results.Ok(ResultDto<object>.ReturnData(
                null,
                ResultCodeEnum.Success,
                ResultCodeEnum.Success.ToDisplayName()
            ));
        }).WithName("DeleteHostApplication");

        // Toggle isActive -> State
        app.MapPatch("UpdateState/{id:long}/{isActive:bool}", async (long id, bool isActive, CancellationToken cancellationToken, ILoggerFactory loggerFactory, IHostApplicationService hostApplicationService, IMediator mediator) =>
        {
             await mediator.Send(new UpdateStateHostApplicationCommand { Id = id, IsActive = isActive }, cancellationToken);
            await hostApplicationService.UpdateStateAsync(id, isActive, cancellationToken);
            return Results.Ok(ResultDto<HostApplicationDto>.ReturnData(
                null,
                ResultCodeEnum.Success,
                ResultCodeEnum.Success.ToDisplayName()
            ));
        }).WithName("UpdateStateHostApplication");
    }

}
