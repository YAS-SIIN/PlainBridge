namespace PlainBridge.SharedDomain.Base.ValueObjects;

public sealed class AppId : BaseValueObject<AppId>
{
    public AppId() { }
    private AppId(Guid viewId)
    {
        ViewId = viewId;
    }
    public Guid ViewId { get; set; }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return ViewId;
    }

    public static AppId CreateUniqueId() => new AppId(Guid.CreateVersion7());
    public static AppId Create(Guid viewId) => new AppId(viewId);
     
}
