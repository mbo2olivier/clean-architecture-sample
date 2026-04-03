namespace Cnss.Shared.Domain.Events;

public abstract record DomainEvent(DateTime OccurredOn) : IDomainEvent
{
    protected DomainEvent()
        : this(DateTime.UtcNow)
    {
    }
}
