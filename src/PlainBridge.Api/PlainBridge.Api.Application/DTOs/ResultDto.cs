using PlainBridge.Api.Application.Enums;

namespace PlainBridge.Api.Application.DTOs;


public class ResultDto<T>
{
    public T? Data { get; set; }
     
    public ResultCodeEnum? ResultCode { get; set; }
    public string? ResultDescription { get; set; }

    public string? ErrorDetail { get; set; }
    public IDictionary<string, string[]>? Errors { get; set; }

    public static ResultDto<T> ReturnData(T? data, ResultCodeEnum? resultCode, string? resultDescription, string? errorDetail = "", IDictionary<string, string[]>? Errors = null)
    {
        return new ResultDto<T>
        {
            Data = data, 
            ResultCode = resultCode,
            ResultDescription = resultDescription,
            ErrorDetail = errorDetail,
            Errors = Errors
        };
    }

}