using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlainBridge.Api.ApiEndPoint.Abstractions;
using PlainBridge.Api.Application.Services.Session;
using PlainBridge.Api.Application.UseCases.ServerApplication.Commands;
using PlainBridge.Api.Application.UseCases.ServerApplication.Queries;
using PlainBridge.SharedApplication.DTOs;
using PlainBridge.SharedApplication.Enums;
using PlainBridge.SharedApplication.Exceptions;
using PlainBridge.SharedApplication.Extensions;
using PlainBridge.SharedApplication.Mediator;

namespace PlainBridge.Api.ApiEndPoint.Endpoints;

public class ServerApplicationEndPoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        var app = builder.MapGroup("api/ServerApplication")
            .RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme });

        // GetAllAsync
        app.MapGet("", async (CancellationToken cancellationToken, ILoggerFactory loggerFactory, IMediator mediator, ISessionService _sessionService) =>
        {
            var data = await mediator.Send(new GetAllServerApplicationsQuery(), cancellationToken);
            return Results.Ok(ResultDto<IList<ServerApplicationDto>>.ReturnData(
                data,
                ResultCodeEnum.Success,
                ResultCodeEnum.Success.ToDisplayName()
            ));
        }).WithName("GetAllServerApplications");


        // GetByIdAsync
        app.MapGet("{id:long}", async (long id, CancellationToken cancellationToken, ILoggerFactory loggerFactory, IMediator mediator,  ISessionService sessionService) =>
        {
            var user = await sessionService.GetCurrentUserAsync(cancellationToken);
            if (user == null)
                throw new NotFoundException("user");

            var data = await mediator.Send(new GetServerApplicationQuery { Id = id }, cancellationToken);

            return Results.Ok(ResultDto<ServerApplicationDto>.ReturnData(
                data,
                ResultCodeEnum.Success,
                ResultCodeEnum.Success.ToDisplayName()
            ));
        }).WithName("GetServerApplication");


        // CreateAsync
        app.MapPost("", async ([FromBody] CreateServerApplicationCommand hostApplication, CancellationToken cancellationToken, ILoggerFactory loggerFactory, IMediator mediator, ISessionService sessionService) =>
        {
            var user = await sessionService.GetCurrentUserAsync(cancellationToken);
            if (user == null)
                throw new NotFoundException("user");

            hostApplication.UserId = user.Id;
            var id = await mediator.Send(hostApplication, cancellationToken); 
            return Results.Created($"/serverApplication/{id}", ResultDto<Guid>.ReturnData(
                id,
                ResultCodeEnum.Success,
                ResultCodeEnum.Success.ToDisplayName()
            ));
        }).WithName("CreateServerApplication");

        // UpdateAsync
        app.MapPatch("{id:long}", async (long id, [FromBody] UpdateServerApplicationCommand hostApplication, CancellationToken cancellationToken, ILoggerFactory loggerFactory, IMediator mediator, ISessionService sessionService) =>
        {
            var user = await sessionService.GetCurrentUserAsync(cancellationToken);
            if (user == null)
                throw new NotFoundException("user");

            hostApplication.Id = id;
            hostApplication.UserId = user.Id;
            var updatedApplicationAppId = await mediator.Send(hostApplication, cancellationToken);
            return Results.Ok(ResultDto<Guid>.ReturnData(
                updatedApplicationAppId,
                ResultCodeEnum.Success,
                ResultCodeEnum.Success.ToDisplayName()
            ));
        }).WithName("UpdateServerApplication");


        // DeleteAsync
        app.MapDelete("{id:long}", async (long id, CancellationToken cancellationToken, ILoggerFactory loggerFactory, IMediator mediator, ISessionService sessionService) =>
        {
            await mediator.Send(new DeleteServerApplicationCommand { Id = id }, cancellationToken);
            return Results.Ok(ResultDto<object>.ReturnData(
                null,
                ResultCodeEnum.Success,
                ResultCodeEnum.Success.ToDisplayName()
            ));
        }).WithName("DeleteServerApplication");

        // Toggle isActive -> State
        app.MapPatch("UpdateState/{id:long}/{isActive:bool}", async (long id, bool isActive, CancellationToken cancellationToken, ILoggerFactory loggerFactory, IMediator mediator) =>
        {
            await mediator.Send(new UpdateStateServerApplicationCommand { Id = id, IsActive = isActive }, cancellationToken);
            return Results.Ok(ResultDto<ServerApplicationDto>.ReturnData(
                null,
                ResultCodeEnum.Success,
                ResultCodeEnum.Success.ToDisplayName()
            ));
        }).WithName("PatchServerApplicationIsActive");
    }
     
}
