

using PlainBridge.SharedDomain.Base.ValueObjects;

namespace PlainBridge.Api.Domain.HostAggregate.ValueObjects;

 
public sealed class InternalUrl : BaseValueObject<InternalUrl>
{
    public required string InternalUrlValue { get; init; }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return InternalUrlValue;
    }

    public static InternalUrl Create(string internalUrlValue)
    {
        EnsureDomain(internalUrlValue);
        return new InternalUrl { InternalUrlValue = internalUrlValue };
    }


    private static void EnsureDomain(string internalUrlValue)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(internalUrlValue);
        if (internalUrlValue.Length > 200)
            throw new ApplicationException("Domain must be 200 characters or fewer.");
    }

}