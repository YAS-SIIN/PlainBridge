using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using PlainBridge.Api.Application.DTOs;
using PlainBridge.Api.Application.Services.HostApplication;
using PlainBridge.Api.Application.Services.Session;
using PlainBridge.SharedApplication.DTOs;
using PlainBridge.SharedApplication.Enums;
using PlainBridge.SharedApplication.Exceptions;
using PlainBridge.SharedApplication.Extentions;

namespace PlainBridge.Api.ApiEndPoint.Endpoints;

public static class HostApplicationEndpoint
{
    public static void MapHostApplicationEndpoint(this IEndpointRouteBuilder builder)
    {
        var app = builder.MapGroup("HostApplication")
            .RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme});

        // GetAllAsync
        app.MapGet("", async (CancellationToken cancellationToken, IHostApplicationService hostApplicationService, ISessionService _sessionService) =>
        {
            var user = await _sessionService.GetCurrentUserAsync(cancellationToken);
            if (user == null)
                throw new NotFoundException("user");

            var data = await hostApplicationService.GetAllAsync(user.Id, cancellationToken);
            return Results.Ok(ResultDto<IList<HostApplicationDto>>.ReturnData(
                data,
                ResultCodeEnum.Success,
                ResultCodeEnum.Success.ToDisplayName()
            ));
        }).WithName("GetAllHostApplications");

        // GetAsync
        app.MapGet("{id:long}", async (long id, CancellationToken cancellationToken, IHostApplicationService hostApplicationService, ISessionService _sessionService) =>
        {
            var user = await _sessionService.GetCurrentUserAsync(cancellationToken);
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
        app.MapPost("", async ([FromBody] HostApplicationDto hostApplication, CancellationToken cancellationToken, IHostApplicationService hostApplicationService, ISessionService sessionService) =>
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
        app.MapPut("{id:long}", async (long id, CancellationToken cancellationToken, [FromBody] HostApplicationDto hostApplication, IHostApplicationService hostApplicationService, ISessionService sessionService) =>
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
        app.MapDelete("{id:long}", async (long id, CancellationToken cancellationToken, IHostApplicationService hostApplicationService) =>
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
