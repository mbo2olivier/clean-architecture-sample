using Cnss.Shared.Domain.Events;

namespace Cnss.Cotisation.Domain.Events;

public sealed record DeclarationPublishedEvent(
    string DeclarationIdentifier,
    string EmployerIdentifier,
    string Period,
    decimal TotalAmount) : DomainEvent;
