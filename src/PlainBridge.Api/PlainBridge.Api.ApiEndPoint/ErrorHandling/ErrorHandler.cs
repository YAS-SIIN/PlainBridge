using Microsoft.AspNetCore.Diagnostics;

using PlainBridge.Api.Application.DTOs;
using PlainBridge.SharedApplication.DTOs;
using PlainBridge.SharedApplication.Enums;
using PlainBridge.SharedApplication.Exceptions;
using PlainBridge.SharedApplication.Extentions;

namespace PlainBridge.Api.ApiService.ErrorHandling;

public class ErrorHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
    HttpContext httpContext,
    Exception exception,
    CancellationToken cancellationToken)
    {
        switch (exception)
        {
            case NotFoundException:
                httpContext.Response.StatusCode = StatusCodes.Status404NotFound; 
                break;
            case ArgumentNullException:
                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                break;
            case ArgumentException:
                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                break;
            case ApplicationException:
                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                break;
            case DuplicatedException:
                httpContext.Response.StatusCode = StatusCodes.Status409Conflict;
                break;
            default:
                break;
        }

        httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
        await httpContext.Response.WriteAsJsonAsync(ResultDto<object>.ReturnData(exception.Message, ResultCodeEnum.Error, ResultCodeEnum.Error.ToDisplayName(), exception?.InnerException?.Message ?? ""), cancellationToken);

        return true;
    }
}
