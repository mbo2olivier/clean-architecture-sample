namespace Cnss.Cotisation.Infrastructure.Persistence;

public sealed class CotisationDeclarationRecord
{
    public string Identifier { get; set; } = string.Empty;

    public string EmployerIdentifier { get; set; } = string.Empty;

    public int Year { get; set; }

    public int Month { get; set; }

    public bool IsPublished { get; set; }

    public List<CotisationDeclarationItemRecord> Items { get; set; } = [];
}
