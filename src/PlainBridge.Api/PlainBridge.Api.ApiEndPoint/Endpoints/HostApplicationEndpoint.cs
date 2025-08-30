using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlainBridge.Api.ApiEndPoint.Abstractions;
using PlainBridge.Api.Application.DTOs;
using PlainBridge.Api.Application.Services.HostApplication;
using PlainBridge.Api.Application.Services.Session;
using PlainBridge.Api.Application.UseCases.ServerApplication.Queries;
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
        app.MapGet("{id:long}", async (long id, CancellationToken cancellationToken, ILoggerFactory loggerFactory, IHostApplicationService hostApplicationService, ISessionService sessionService) =>
        {
            var user = await sessionService.GetCurrentUserAsync(cancellationToken);
            if (user == null)
                throw new NotFoundException("user");

            var data = await hostApplicationService.GetAsync(id, user.Id, cancellationToken);
            if (data == null)
            {
                return Results.NotFound(ResultDto<HostApplicationDto>.ReturnData(
                    null,
                    ResultCodeEnum.NotFound,
                    ResultCodeEnum.NotFound.ToDisplayName()
                ));
            }
            return Results.Ok(ResultDto<HostApplicationDto>.ReturnData(
                data,
                ResultCodeEnum.Success,
                ResultCodeEnum.Success.ToDisplayName()
            ));
        }).WithName("GetHostApplication");

        // CreateAsync
        app.MapPost("", async ([FromBody] HostApplicationDto hostApplication, CancellationToken cancellationToken, ILoggerFactory loggerFactory, IHostApplicationService hostApplicationService, ISessionService sessionService) =>
        {
            var user = await sessionService.GetCurrentUserAsync(cancellationToken);
            if (user == null)
                throw new NotFoundException("user");

            hostApplication.UserId = user.Id;
            var id = await hostApplicationService.CreateAsync(hostApplication, cancellationToken);
            return Results.Created($"/hostApplication/{id}", ResultDto<Guid>.ReturnData(
                id,
                ResultCodeEnum.Success,
                ResultCodeEnum.Success.ToDisplayName()
            ));
        }).WithName("CreateHostApplication");

        // UpdateAsync
        app.MapPatch("{id:long}", async (long id, CancellationToken cancellationToken, [FromBody] HostApplicationDto hostApplication, ILoggerFactory loggerFactory, IHostApplicationService hostApplicationService, ISessionService sessionService) =>
        {
            var user = await sessionService.GetCurrentUserAsync(cancellationToken);
            if (user == null)
                throw new NotFoundException("user");

            hostApplication.UserId = user.Id;
            hostApplication.Id = id;
            await hostApplicationService.UpdateAsync(hostApplication, cancellationToken);
            return Results.Ok(ResultDto<HostApplicationDto>.ReturnData(
                hostApplication,
                ResultCodeEnum.Success,
                ResultCodeEnum.Success.ToDisplayName()
            ));
        }).WithName("UpdateHostApplication");
         
        // DeleteAsync
        app.MapDelete("{id:long}", async (long id, CancellationToken cancellationToken, ILoggerFactory loggerFactory, IHostApplicationService hostApplicationService) =>
        {
            await hostApplicationService.DeleteAsync(id, cancellationToken);
            return Results.Ok(ResultDto<object>.ReturnData(
                null,
                ResultCodeEnum.Success,
                ResultCodeEnum.Success.ToDisplayName()
            ));
        }).WithName("DeleteHostApplication");

        // Toggle isActive -> State
        app.MapPatch("UpdateState/{id:long}/{isActive:bool}", async (long id, bool isActive, CancellationToken cancellationToken, ILoggerFactory loggerFactory, IHostApplicationService hostApplicationService) =>
        {
            await hostApplicationService.UpdateStateAsync(id, isActive, cancellationToken);
            return Results.Ok(ResultDto<HostApplicationDto>.ReturnData(
                null,
                ResultCodeEnum.Success,
                ResultCodeEnum.Success.ToDisplayName()
            ));
        }).WithName("PatchHostApplicationIsActive");
    }

}
