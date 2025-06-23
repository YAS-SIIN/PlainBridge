using PlainBridge.Api.Application.DTOs;
using PlainBridge.Api.Application.Services.ServerApplication;
using PlainBridge.SharedApplication.DTOs;
using PlainBridge.SharedApplication.Enums;
using PlainBridge.SharedApplication.Extentions;

namespace PlainBridge.Api.ApiEndPoint.Endpoints;

public static class ServerApplicationEndPoint
{
    public static void MapServerApplicationEndpoint(this IEndpointRouteBuilder builder)
    {
        var app = builder.MapGroup("ServerApplication");

        // GetAllAsync
        app.MapGet("", async (IServerApplicationService serverApplicationService, CancellationToken cancellationToken) =>
        {
            var data = await serverApplicationService.GetAllAsync(cancellationToken);
            return Results.Ok(ResultDto<IList<ServerApplicationDto>>.ReturnData(
                data,
                ResultCodeEnum.Success,
                ResultCodeEnum.Success.ToDisplayName()
            ));
        }).WithName("GetAllServerApplication");


        // GetByIdAsync
        app.MapGet("{id:long}", async (long id, IServerApplicationService serverApplicationService, CancellationToken cancellationToken) =>
        {
            var data = await serverApplicationService.GetAsync(id, cancellationToken);
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
        app.MapPost("", async (ServerApplicationDto hostApplication, IServerApplicationService serverApplicationService, CancellationToken cancellationToken) =>
        {
            var id = await serverApplicationService.CreateAsync(hostApplication, cancellationToken);
            return Results.Created($"/persons/{id}", ResultDto<Guid>.ReturnData(
                id,
                ResultCodeEnum.Success,
                ResultCodeEnum.Success.ToDisplayName()
            ));
        }).WithName("CreateServerApplication");

        // UpdateAsync
        app.MapPut("{id:long}", async (long id, ServerApplicationDto hostApplication, IServerApplicationService serverApplicationService, CancellationToken cancellationToken) =>
        {
            hostApplication.Id = id;
            await serverApplicationService.UpdateAsync(hostApplication, cancellationToken);
            return Results.Ok(ResultDto<ServerApplicationDto>.ReturnData(
                hostApplication,
                ResultCodeEnum.Success,
                ResultCodeEnum.Success.ToDisplayName()
            ));
        }).WithName("UpdateServerApplication");


        // DeleteAsync
        app.MapDelete("{id:long}", async (long id, IServerApplicationService serverApplicationService, CancellationToken cancellationToken) =>
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
