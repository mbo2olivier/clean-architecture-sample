using Cnss.Shared.Domain.Entities;

namespace Cnss.Cotisation.Domain.Entities;

public sealed class DeclarationItem : Entity<string>
{
    public DeclarationItem(string id, string employeeIdentifier, decimal grossSalary)
        : base(id)
    {
        if (string.IsNullOrWhiteSpace(employeeIdentifier))
        {
            throw new ArgumentException("Le matricule de l'employé est obligatoire.", nameof(employeeIdentifier));
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

    public decimal Amount => Math.Round(GrossSalary * 0.05m, 2, MidpointRounding.ToEven);
}
