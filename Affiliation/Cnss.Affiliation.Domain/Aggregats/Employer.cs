using Cnss.Affiliation.Domain.Events;
using Cnss.Shared.Domain.Aggregats;

namespace Cnss.Affiliation.Domain.Aggregats;

public sealed class Employer : AggregateRoot<string>
{
    private readonly List<string> _employeeIdentifiers = [];

    private Employer(string identifier, string registrationNumber, string companyName)
        : base(identifier)
    {
        RegistrationNumber = registrationNumber;
        CompanyName = companyName;
    }

    public string Identifier => Id;

    public string RegistrationNumber { get; }

    public string CompanyName { get; private set; }

    public IReadOnlyCollection<string> EmployeeIdentifiers => _employeeIdentifiers.AsReadOnly();

    public static Employer Affiliate(string identifier, string registrationNumber, string companyName)
    {
        EnsureRequired(identifier, nameof(identifier));
        EnsureRequired(registrationNumber, nameof(registrationNumber));
        EnsureRequired(companyName, nameof(companyName));

        var employer = new Employer(identifier.Trim(), registrationNumber.Trim().ToUpperInvariant(), companyName.Trim());
        employer.AddEvent(new EmployerAffiliatedEvent(employer.Identifier, employer.RegistrationNumber, employer.CompanyName));
        return employer;
    }

    public void AttachEmployee(Employee employee)
    {
        ArgumentNullException.ThrowIfNull(employee);

        if (_employeeIdentifiers.Contains(employee.Identifier, StringComparer.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Cet employé est déjà rattaché à cet employeur.");
        }

        employee.AttachToEmployer(Identifier);
        _employeeIdentifiers.Add(employee.Identifier);
        AddEvent(new EmployeeAttachedToEmployeeEvent(Identifier, employee.Identifier));
    }

    private static void EnsureRequired(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("La valeur est obligatoire.", parameterName);
        }
    }
}
