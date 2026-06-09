using Microsoft.AspNetCore.Diagnostics;
 
using PlainBridge.Shared.Application.DTOs;
using PlainBridge.Shared.Application.Enums;
using PlainBridge.Shared.Application.Exceptions;
using PlainBridge.Shared.Application.Extensions;

namespace PlainBridge.Api.ApiEndPoint.ErrorHandling;

public class ErrorHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
    HttpContext httpContext,
    Exception exception,
    CancellationToken cancellationToken)
    {
        var resultCode = ResultCodeEnum.Error;
        switch (exception)
        {
            case NotFoundException:
                resultCode = ResultCodeEnum.NotFound;
                httpContext.Response.StatusCode = StatusCodes.Status404NotFound; 
                break;
            case ArgumentNullException:
                resultCode = ResultCodeEnum.NullData;
                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                break;
            case ArgumentException:
                resultCode = ResultCodeEnum.Error;
                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                break;
            case ApplicationException:
                resultCode = ResultCodeEnum.Error;
                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                break;
            case DuplicatedException:
                resultCode = ResultCodeEnum.DuplicatedData;
                httpContext.Response.StatusCode = StatusCodes.Status409Conflict;
                break;
            default:
                break;
        }

        await httpContext.Response.WriteAsJsonAsync(ResultDto<object>.ReturnData(exception.Message, resultCode, resultCode.ToDisplayName(), exception?.InnerException?.Message ?? ""), cancellationToken);

        return true;
    }
}
