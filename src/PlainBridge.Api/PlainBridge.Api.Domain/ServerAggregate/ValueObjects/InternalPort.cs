
using PlainBridge.SharedDomain.Base.ValueObjects;

namespace PlainBridge.Api.Domain.ServerAggregate.ValueObjects;


public sealed class InternalPort : BaseValueObject<InternalPort>
{
    public required int Port { get; init; }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Port;
    }

    public static InternalPort Create(int port)
    {
        EnsurePort(port);
        return new InternalPort { Port = port };
    }


    private static void EnsurePort(int port)
    {
        if (port is < 1 or > 65535)
            throw new ApplicationException("Port range is not valid (1-65535).");
    }

}