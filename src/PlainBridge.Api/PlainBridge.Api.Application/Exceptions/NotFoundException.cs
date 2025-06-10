
using PlainBridge.Api.Domain.Common;

namespace PlainBridge.Api.Application.Exceptions;

public class NotFoundException(object data)
    : Exception($"data for {data} not found");
 