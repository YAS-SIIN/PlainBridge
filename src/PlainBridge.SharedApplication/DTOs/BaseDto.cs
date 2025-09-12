

using PlainBridge.SharedApplication.Enums;

namespace PlainBridge.SharedApplication.DTOs;

public abstract record BaseDto<TId>
{
    public TId? Id { get; set; }
    public RowStateEnum State { get; set; }
    public string? Description { get; set; } = string.Empty;
}
