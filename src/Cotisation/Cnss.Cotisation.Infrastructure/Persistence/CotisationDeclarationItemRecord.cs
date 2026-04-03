namespace Cnss.Cotisation.Infrastructure.Persistence;

public sealed class CotisationDeclarationItemRecord
{
    public string Identifier { get; set; } = string.Empty;

    public string DeclarationIdentifier { get; set; } = string.Empty;

    public string EmployeeIdentifier { get; set; } = string.Empty;

    public decimal GrossSalary { get; set; }

    public decimal Amount { get; set; }
}
