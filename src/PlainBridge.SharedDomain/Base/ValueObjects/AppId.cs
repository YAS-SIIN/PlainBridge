namespace PlainBridge.SharedDomain.Base.ValueObjects;

public sealed class AppId : BaseValueObject<AppId>
{
    public required Guid ViewId { get; init; }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return ViewId;
    }

    public static AppId CreateUniqueId() => new AppId { ViewId = Guid.CreateVersion7() };
     
}
