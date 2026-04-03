using Cnss.Shared.Domain.ValuesObject;

namespace Cnss.Cotisation.Domain.ValuesObject;

public sealed class DeclarationItemData : ValueObject
{
    public DeclarationItemData(string employeeIdentifier, decimal grossSalary)
    {
        if (string.IsNullOrWhiteSpace(employeeIdentifier))
        {
            throw new ArgumentException("L'identifiant employé est obligatoire.", nameof(employeeIdentifier));
        }

        if (grossSalary <= 0)
        {
            throw new ArgumentException("Le salaire brut doit être strictement positif.", nameof(grossSalary));
        }

        EmployeeIdentifier = employeeIdentifier.Trim().ToUpperInvariant();
        GrossSalary = grossSalary;
    }

    public string EmployeeIdentifier { get; }

    public decimal GrossSalary { get; }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return EmployeeIdentifier;
        yield return GrossSalary;
    }
}
