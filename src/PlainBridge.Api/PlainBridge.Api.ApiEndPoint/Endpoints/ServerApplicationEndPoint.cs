using PlainBridge.Api.Application.DTOs;
using PlainBridge.Api.Application.Services.ServerApplication;
using PlainBridge.Api.Application.Services.Session;
using PlainBridge.SharedApplication.DTOs;
using PlainBridge.SharedApplication.Enums;
using PlainBridge.SharedApplication.Exceptions;
using PlainBridge.SharedApplication.Extentions;

namespace PlainBridge.Api.ApiEndPoint.Endpoints;

public static class ServerApplicationEndPoint
{
    public static void MapServerApplicationEndpoint(this IEndpointRouteBuilder builder)
    {
        var app = builder.MapGroup("ServerApplication");

        // GetAllAsync
        app.MapGet("", async (CancellationToken cancellationToken, IServerApplicationService serverApplicationService, ISessionService _sessionService) =>
        {
            var user = await _sessionService.GetCurrentUsereAsync(cancellationToken);
            if (user == null)
                throw new NotFoundException("User");

            var data = await serverApplicationService.GetAllAsync(user.Id, cancellationToken);
            return Results.Ok(ResultDto<IList<ServerApplicationDto>>.ReturnData(
                data,
                ResultCodeEnum.Success,
                ResultCodeEnum.Success.ToDisplayName()
            ));
        }).WithName("GetAllServerApplication");


        // GetByIdAsync
        app.MapGet("{id:long}", async (long id, CancellationToken cancellationToken, IServerApplicationService serverApplicationService,  ISessionService _sessionService) =>
        {
            var user = await _sessionService.GetCurrentUsereAsync(cancellationToken);
            if (user == null)
                throw new NotFoundException("User");

            var data = await serverApplicationService.GetAsync(id, user.Id, cancellationToken);
            if (data == null)
            {
                return Results.NotFound(ResultDto<ServerApplicationDto>.ReturnData(
                    null,
                    ResultCodeEnum.NotFound,
                    ResultCodeEnum.NotFound.ToDisplayName()
                ));
            }
            return Results.Ok(ResultDto<ServerApplicationDto>.ReturnData(
                data,
                ResultCodeEnum.Success,
                ResultCodeEnum.Success.ToDisplayName()
            ));
        }).WithName("GetServerApplication");


        // CreateAsync
        app.MapPost("", async (ServerApplicationDto hostApplication, CancellationToken cancellationToken, IServerApplicationService serverApplicationService, ISessionService _sessionService) =>
        {
            var user = await _sessionService.GetCurrentUsereAsync(cancellationToken);
            if (user == null)
                throw new NotFoundException("User");

            hostApplication.UserId = user.Id;
            var id = await serverApplicationService.CreateAsync(hostApplication, cancellationToken);
            return Results.Created($"/persons/{id}", ResultDto<Guid>.ReturnData(
                id,
                ResultCodeEnum.Success,
                ResultCodeEnum.Success.ToDisplayName()
            ));
        }).WithName("CreateServerApplication");

        // UpdateAsync
        app.MapPut("{id:long}", async (long id, CancellationToken cancellationToken, ServerApplicationDto hostApplication, IServerApplicationService serverApplicationService, ISessionService _sessionService) =>
        {
            var user = await _sessionService.GetCurrentUsereAsync(cancellationToken);
            if (user == null)
                throw new NotFoundException("User");

            hostApplication.Id = id;
            hostApplication.UserId = user.Id;
            await serverApplicationService.UpdateAsync(hostApplication, cancellationToken);
            return Results.Ok(ResultDto<ServerApplicationDto>.ReturnData(
                hostApplication,
                ResultCodeEnum.Success,
                ResultCodeEnum.Success.ToDisplayName()
            ));
        }).WithName("UpdateServerApplication");


        // DeleteAsync
        app.MapDelete("{id:long}", async (long id, CancellationToken cancellationToken, IServerApplicationService serverApplicationService, ISessionService _sessionService) =>
        {
            await serverApplicationService.DeleteAsync(id, cancellationToken);
            return Results.Ok(ResultDto<object>.ReturnData(
                null,
                ResultCodeEnum.Success,
                ResultCodeEnum.Success.ToDisplayName()
            ));
        }).WithName("DeleteServerApplication");
    }
}
