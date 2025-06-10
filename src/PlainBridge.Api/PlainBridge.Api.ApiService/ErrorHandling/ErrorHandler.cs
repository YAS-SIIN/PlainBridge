using Microsoft.AspNetCore.Diagnostics;

using PlainBridge.Api.Application.Exceptions;

namespace PlainBridge.Api.ApiService.ErrorHandling;

public class ErrorHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
    HttpContext httpContext,
    Exception exception,
    CancellationToken cancellationToken)
    {
        if (exception is not NotFoundException) return false;

        httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
        await httpContext.Response.WriteAsJsonAsync(exception.Message, cancellationToken);

        return true;
    }
}
