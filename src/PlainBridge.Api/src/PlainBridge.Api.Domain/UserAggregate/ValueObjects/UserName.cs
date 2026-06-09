
using PlainBridge.SharedDomain.Base.ValueObjects;

namespace PlainBridge.Api.Domain.UserAggregate.ValueObjects;


public sealed class UserName : BaseValueObject<UserName>
{
    public UserName() { }
    private UserName(string username)
    {
        UserNameValue = username;
    }
    public string UserNameValue { get; set; }
    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return UserNameValue;
    }

    public static UserName Create(string username)
    {
        EnsureUsername(username);
        return new UserName(username);
    }


    private static void EnsureUsername(string username)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);
        if (username.Length > 150)
            throw new ApplicationException("Username must be 150 characters or fewer.");
    }

}
 
