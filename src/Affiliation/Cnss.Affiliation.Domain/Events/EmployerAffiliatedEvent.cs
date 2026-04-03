using Cnss.Shared.Domain.Events;

namespace Cnss.Affiliation.Domain.Events;

public sealed record EmployerAffiliatedEvent(
    string EmployerIdentifier,
    string RegistrationNumber,
    string CompanyName) : DomainEvent;
