 
namespace PlainBridge.Shared.Application.Exceptions;
 
public class DuplicatedException(object data)
    : Exception($"data {data} is duplicated");