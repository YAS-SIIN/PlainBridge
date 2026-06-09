 
namespace PlainBridge.SharedApplication.Exceptions;
 
public class DuplicatedException(object data)
    : Exception($"data {data} is duplicated");