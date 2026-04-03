namespace Cnss.Affiliation.Infrastructure.Persistence;

public sealed class AffiliationEmployeeRecord
{
    public string Identifier { get; set; } = string.Empty;

    public string RegistrationNumber { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string? EmployerIdentifier { get; set; }
}
