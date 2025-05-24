
using PlainBridge.Api.Domain.Enums;

namespace PlainBridge.Api.Application.DTOs;

public abstract record BaseDto<TKey>
{
    public TKey Id { get; set; }
    public RowStateEnum State { get; set; }
    public string Description { get; set; } = string.Empty;
}
