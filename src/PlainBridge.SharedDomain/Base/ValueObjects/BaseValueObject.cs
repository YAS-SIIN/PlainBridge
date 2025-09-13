namespace PlainBridge.SharedDomain.Base.ValueObjects;


public abstract class BaseValueObject<T> where T : BaseValueObject<T>
{
    public abstract IEnumerable<object> GetEqualityComponents();



    public override bool Equals(object? obj)
        => obj is not null &&
           obj is T valueObject &&
           obj.GetType() == GetType() &&
           GetEqualityComponents().SequenceEqual(valueObject.GetEqualityComponents());

    public static bool operator ==(BaseValueObject<T> left, BaseValueObject<T> right)
        => left.Equals(right);

    public static bool operator !=(BaseValueObject<T> left, BaseValueObject<T> right)
        => !left.Equals(right);

    protected static bool EqualOperator(BaseValueObject<T> left, BaseValueObject<T> right)
    {
        if (ReferenceEquals(left, null) ^ ReferenceEquals(right, null))
        {
            return false;
        }
        return ReferenceEquals(left, right) || left.Equals(right);
    }

    protected static bool NotEqualOperator(BaseValueObject<T> left, BaseValueObject<T> right)
    {
        return !(EqualOperator(left, right));
    }

    public override int GetHashCode()
        => GetEqualityComponents()
                .Select(x => x?.GetHashCode() ?? 0)
                .Aggregate((x, y) => x ^ y);
}
