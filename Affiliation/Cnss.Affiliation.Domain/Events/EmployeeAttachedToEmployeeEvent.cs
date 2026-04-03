using Cnss.Shared.Domain.Events;

namespace Cnss.Affiliation.Domain.Events;

public sealed record EmployeeAttachedToEmployeeEvent(
    string EmployerIdentifier,
    string EmployeeIdentifier) : DomainEvent;
