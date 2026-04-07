using Cnss.Shared.Domain.ValuesObject;

namespace Cnss.Cotisation.Domain.ValuesObject;

public sealed class EmployerIdentifier : ValueObject
{
    private EmployerIdentifier()
    {
    }

    public EmployerIdentifier(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("L'identifiant employeur est obligatoire.", nameof(value));
        }

        Value = value.Trim().ToUpperInvariant();
    }

    public string Value { get; private set; } = string.Empty;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
