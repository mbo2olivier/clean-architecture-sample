namespace Cnss.Shared.Domain.Entities;

public abstract class Entity<TId> : IEquatable<Entity<TId>>
    where TId : notnull
{
    protected Entity(TId id)
    {
        Id = id;
    }

    public TId Id { get; }

    public bool Equals(Entity<TId>? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Id.Equals(other.Id);
    }

    public override bool Equals(object? obj) => obj is Entity<TId> other && Equals(other);

    public override int GetHashCode() => Id.GetHashCode();
}
