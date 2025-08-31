

using PlainBridge.SharedApplication.Enums;

namespace PlainBridge.Api.Application.UseCases;

public class BaseCommand<TId>
{
    public TId Id { get; set; }
    public RowStateEnum State { get; set; }
    public string? Description { get; set; } = string.Empty;
}
