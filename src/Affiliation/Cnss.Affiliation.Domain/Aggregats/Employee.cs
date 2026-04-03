using Cnss.Shared.Domain.Aggregats;

namespace Cnss.Affiliation.Domain.Aggregats;

public sealed class Employee : AggregateRoot<string>
{
    private Employee(
        string identifier,
        string registrationNumber,
        string firstName,
        string lastName)
        : base(identifier)
    {
        RegistrationNumber = registrationNumber;
        FirstName = firstName;
        LastName = lastName;
    }

    public string Identifier => Id;

    public string RegistrationNumber { get; }

    public string FirstName { get; }

    public string LastName { get; }

    public string? EmployerIdentifier { get; private set; }

    public static Employee Create(string identifier, string registrationNumber, string firstName, string lastName)
    {
        EnsureRequired(identifier, nameof(identifier));
        EnsureRequired(registrationNumber, nameof(registrationNumber));
        EnsureRequired(firstName, nameof(firstName));
        EnsureRequired(lastName, nameof(lastName));

        return new Employee(
            identifier.Trim(),
            registrationNumber.Trim().ToUpperInvariant(),
            firstName.Trim(),
            lastName.Trim());
    }

    public static Employee Restore(
        string identifier,
        string registrationNumber,
        string firstName,
        string lastName,
        string? employerIdentifier)
    {
        var employee = Create(identifier, registrationNumber, firstName, lastName);

        if (!string.IsNullOrWhiteSpace(employerIdentifier))
        {
            employee.EmployerIdentifier = employerIdentifier.Trim();
        }

        employee.ClearDomainEvents();
        return employee;
    }

    internal void AttachToEmployer(string employerIdentifier)
    {
        EnsureRequired(employerIdentifier, nameof(employerIdentifier));
        EmployerIdentifier = employerIdentifier.Trim();
    }

    private static void EnsureRequired(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("La valeur est obligatoire.", parameterName);
        }
    }
}
