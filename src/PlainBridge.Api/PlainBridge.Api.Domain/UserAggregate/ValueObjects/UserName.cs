 
namespace PlainBridge.Api.Domain.UserAggregate.ValueObjects;

public class UserName
{
    public string Value { get; }

    private UserName(string value)
    {
        Value = value;
    }

    public static UserName Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("User name cannot be empty.", nameof(value));

        return new UserName(value);
    }
}
