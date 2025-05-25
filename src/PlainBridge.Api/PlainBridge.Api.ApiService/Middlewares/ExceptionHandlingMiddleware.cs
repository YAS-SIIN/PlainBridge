

using PlainBridge.Api.Application.DTOs;
using PlainBridge.Api.Application.Enums;
using PlainBridge.Api.Application.Exceptions;
using PlainBridge.Api.Application.Extentions;

using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace PlainBridge.Api.ApiService.Middlewares;

internal sealed class ExceptionHandlingMiddleware : IMiddleware
{
   
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception e)
        { 
            await HandleExceptionAsync(context, e);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
    {
        var errorException = new CustomException(ResultCodeEnum.Error, ResultCodeEnum.Error.ToDisplayName());
        IDictionary<string, string[]> errors = new Dictionary<string, string[]>();
     

        var errorData = ResultDto<object>.ReturnData(null, (int)errorException.ResultCode, exception?.Message, errorException.ErrorDescription, errors);

        httpContext.Response.ContentType = "application/json"; 
        await httpContext.Response.WriteAsync(JsonSerializer.Serialize(errorData));
    }

    private static IDictionary<string, string[]> GetErrors(Exception exception)
    {
        IDictionary<string, string[]> errors = null;

        //if (exception is ValidationException validationException)
        //{
        //    errors = validationException.Message;
        //}

        return errors;
    }
}
