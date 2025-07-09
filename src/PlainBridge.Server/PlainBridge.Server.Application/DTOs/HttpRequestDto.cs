 

namespace PlainBridge.Server.Application.DTOs;

public record HttpRequestDto
{
    public string RequestUrl { get; set; }
    public string Method { get; set; }
    public IEnumerable<KeyValuePair<string, IEnumerable<string>>> Headers { get; set; }
    public byte[] Bytes { get; set; }
}
