using PlainBridge.Api.Application.Enums;

using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Xml.Linq;

namespace PlainBridge.Api.Application.Exceptions;

public class CustomException : Exception
{
    [DataMember(Name = "error_code")]
    public ResultCodeEnum ResultCode { get; set; }
      
    [DataMember(Name = "error_description")]
    public string ErrorDescription { get; set; }

    [DataMember(Name = "trace_id")]
    public string TraceId { get; set; }
     

    public CustomException(ResultCodeEnum resultCode, string errorDescription, string traceId = "" ) : base(errorDescription)
    { 
        ResultCode = resultCode;
        ErrorDescription = errorDescription; 
        TraceId = traceId;
    }
}
