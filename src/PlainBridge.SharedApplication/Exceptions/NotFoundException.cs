
using PlainBridge.Api.Domain.Common;

namespace PlainBridge.SharedApplication.Exceptions;

public class NotFoundException(object data)
    : Exception($"data for {data} not found");
 