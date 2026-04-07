using Cnss.Shared.Domain.Entities;

namespace Cnss.Cotisation.Domain.Entities;

public sealed class DeclarationItem : Entity<string>
{
    private DeclarationItem()
    {
    }

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

    public string DeclarationIdentifier { get; private set; } = string.Empty;

    public string EmployeeIdentifier { get; private set; } = string.Empty;

    public decimal GrossSalary { get; private set; }

    public decimal Amount => Math.Round(GrossSalary * 0.05m, 2, MidpointRounding.ToEven);

    internal void AssignToDeclaration(string declarationIdentifier)
    {
        DeclarationIdentifier = declarationIdentifier;
    }
}
