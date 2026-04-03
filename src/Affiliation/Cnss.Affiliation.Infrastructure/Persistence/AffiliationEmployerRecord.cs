namespace Cnss.Affiliation.Infrastructure.Persistence;

public sealed class AffiliationEmployerRecord
{
    public string Identifier { get; set; } = string.Empty;

    public string RegistrationNumber { get; set; } = string.Empty;

    public string CompanyName { get; set; } = string.Empty;

    public string[] EmployeeIdentifiers { get; set; } = [];
}
