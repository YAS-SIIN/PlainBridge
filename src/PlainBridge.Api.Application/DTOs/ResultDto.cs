namespace PlainBridge.Api.Application.DTOs;


public class ResultDto<T>
{
    public T? Data { get; set; }
     
    public int? ResultCode { get; set; }
    public string? ErrorDescription { get; set; }

    public string? ErrorDetail { get; set; }
    public IDictionary<string, string[]>? Errors { get; set; }

    public static ResultDto<T> ReturnData(T? data, int? resultCode, string? ErrorDescription, string? ErrorDetail = "", IDictionary<string, string[]>? Errors = null)
    {
        return new ResultDto<T>
        {
            Data = data, 
            ResultCode = resultCode,
            ErrorDescription = ErrorDescription,
            ErrorDetail = ErrorDetail,
            Errors = Errors
        };
    }

}