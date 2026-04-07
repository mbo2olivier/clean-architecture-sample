using Cnss.Shared.Domain.Events;

namespace Cnss.Shared.Domain.Aggregats;

public interface IAggregateRoot
{
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }

    void ClearDomainEvents();
}
