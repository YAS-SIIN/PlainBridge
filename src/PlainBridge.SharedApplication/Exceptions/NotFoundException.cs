 
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PlainBridge.SharedApplication.Exceptions;
 
public class NotFoundException : Exception
{
    public NotFoundException(object key)
    {
        Data.Add("key", key);
    }

    public NotFoundException(string key, List<KeyValuePair<string, object>> metadata) : base(key)
    {
        foreach (var kvp in metadata)
            Data.Add(kvp.Key, kvp.Value);
    }
}