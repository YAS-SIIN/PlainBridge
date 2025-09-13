


using PlainBridge.SharedDomain.Base.ValueObjects;

namespace PlainBridge.Api.Domain.HostAggregate.ValueObjects;
  
public sealed class HostDomain : BaseValueObject<HostDomain>
{
    public HostDomain() { }
    private HostDomain(string hostDomainName)
    {
        HostDomainName = hostDomainName;
    }
    public string HostDomainName { get; set; }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return HostDomainName;
    }

    public static HostDomain Create(string hostDomain)
    {
        EnsureInternalUrl(hostDomain);
        return new HostDomain(hostDomain);
    }


    private static void EnsureInternalUrl(string internalUrl)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(internalUrl);
        if (internalUrl.Length > 200)
            throw new ApplicationException("InternalUrl must be 200 characters or fewer.");
    }

     

}