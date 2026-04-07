using Cnss.Shared.Domain.ValuesObject;

namespace Cnss.Cotisation.Domain.ValuesObject;

public sealed class DeclarationPeriod : ValueObject
{
    private DeclarationPeriod()
    {
    }

    public DeclarationPeriod(int year, int month)
    {
        if (year < 2000)
        {
            throw new ArgumentException("L'année de déclaration est invalide.", nameof(year));
        }

        if (month is < 1 or > 12)
        {
            throw new ArgumentException("Le mois de déclaration est invalide.", nameof(month));
        }

        Year = year;
        Month = month;
    }

    public int Year { get; private set; }

    public int Month { get; private set; }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Year;
        yield return Month;
    }

    public override string ToString() => $"{Year:D4}-{Month:D2}";
}
