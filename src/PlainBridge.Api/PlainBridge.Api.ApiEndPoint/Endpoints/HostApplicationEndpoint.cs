using PlainBridge.Api.Application.DTOs;
using PlainBridge.Api.Application.Services.HostApplication;
using PlainBridge.SharedApplication.DTOs;
using PlainBridge.SharedApplication.Enums;
using PlainBridge.SharedApplication.Extentions;

namespace PlainBridge.Api.ApiService.Endpoints;

public static class HostApplicationEndpoint
{
    public static void MapHostApplicationEndpoint(this IEndpointRouteBuilder builder)
    {
        var app = builder.MapGroup("HostApplication");

        // GetAllAsync
        app.MapGet("", async (IHostApplicationService hostApplicationService, CancellationToken cancellationToken) =>
        {
                var data = await hostApplicationService.GetAllAsync(cancellationToken);
                return Results.Ok(ResultDto<IList<HostApplicationDto>>.ReturnData(
                    data,
                    ResultCodeEnum.Success,
                    ResultCodeEnum.Success.ToDisplayName()
                ));
        }).WithName("GetAllHostApplication");


        // GetByIdAsync
        app.MapGet("{id:long}", async (long id, IHostApplicationService hostApplicationService, CancellationToken cancellationToken) =>
        {
            var data = await hostApplicationService.GetByIdAsync(id, cancellationToken);
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
        app.MapPost("", async (HostApplicationDto hostApplication, IHostApplicationService hostApplicationService, CancellationToken cancellationToken) =>
        {
                var id = await hostApplicationService.CreateAsync(hostApplication, cancellationToken);
                return Results.Created($"/persons/{id}", ResultDto<Guid>.ReturnData(
                    id,
                    ResultCodeEnum.Success,
                    ResultCodeEnum.Success.ToDisplayName()
                ));
        }).WithName("CreateHostApplication");

        // UpdateAsync
        app.MapPut("{id:long}", async (long id, HostApplicationDto hostApplication, IHostApplicationService hostApplicationService, CancellationToken cancellationToken) =>
        {
                hostApplication.Id = id;
                await hostApplicationService.UpdateAsync(hostApplication, cancellationToken);
                return Results.Ok(ResultDto<HostApplicationDto>.ReturnData(
                    hostApplication,
                    ResultCodeEnum.Success,
                    ResultCodeEnum.Success.ToDisplayName()
                ));
        }).WithName("UpdateHostApplication");


        // DeleteAsync
        app.MapDelete("{id:long}", async (long id, IHostApplicationService hostApplicationService, CancellationToken cancellationToken) =>
        {
                await hostApplicationService.DeleteAsync(id, cancellationToken);
                return Results.Ok(ResultDto<object>.ReturnData(
                    null,
                    ResultCodeEnum.Success,
                    ResultCodeEnum.Success.ToDisplayName()
                ));
        }).WithName("DeleteHostApplication");
    }
}
