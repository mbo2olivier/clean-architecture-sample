namespace Cnss.Shared.Domain.Abstractions;

public abstract class ValueObject : IEquatable<ValueObject>
{
    protected abstract IEnumerable<object?> GetAtomicValues();

    public bool Equals(ValueObject? other)
    {
        if (other is null || other.GetType() != GetType())
        {
            return false;
        }

        return GetAtomicValues().SequenceEqual(other.GetAtomicValues());
    }

    public override bool Equals(object? obj) => obj is ValueObject other && Equals(other);

    public override int GetHashCode()
    {
        return GetAtomicValues()
            .Aggregate(17, (current, value) => HashCode.Combine(current, value));
    }
}
