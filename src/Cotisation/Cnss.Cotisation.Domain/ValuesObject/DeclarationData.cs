using Cnss.Shared.Domain.ValuesObject;

namespace Cnss.Cotisation.Domain.ValuesObject;

public sealed class DeclarationData : ValueObject
{
    public DeclarationData(
        string employerIdentifier,
        int year,
        int month,
        IReadOnlyCollection<DeclarationItemData> items)
    {
        if (string.IsNullOrWhiteSpace(employerIdentifier))
        {
            throw new ArgumentException("L'identifiant employeur est obligatoire.", nameof(employerIdentifier));
        }

        if (items.Count == 0)
        {
            throw new ArgumentException("Une déclaration doit contenir au moins un élément.", nameof(items));
        }

        EmployerIdentifier = employerIdentifier.Trim().ToUpperInvariant();
        Year = year;
        Month = month;
        Items = items;
    }

    public string EmployerIdentifier { get; }

    public int Year { get; }

    public int Month { get; }

    public IReadOnlyCollection<DeclarationItemData> Items { get; }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return EmployerIdentifier;
        yield return Year;
        yield return Month;

        foreach (var item in Items)
        {
            yield return item;
        }
    }
}
