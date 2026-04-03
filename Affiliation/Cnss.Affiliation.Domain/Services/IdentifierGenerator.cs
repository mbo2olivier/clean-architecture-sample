namespace Cnss.Affiliation.Domain.Services;

public sealed class IdentifierGenerator
{
    public string GenerateEmployerIdentifier() => $"EMP-{Guid.NewGuid():N}"[..16];

    public string GenerateEmployeeIdentifier() => $"SAL-{Guid.NewGuid():N}"[..16];
}
