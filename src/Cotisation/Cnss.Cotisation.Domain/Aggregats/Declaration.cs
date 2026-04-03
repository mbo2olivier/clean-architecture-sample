using Cnss.Cotisation.Domain.Entities;
using Cnss.Cotisation.Domain.Events;
using Cnss.Cotisation.Domain.ValuesObject;
using Cnss.Shared.Domain.Aggregats;

namespace Cnss.Cotisation.Domain.Aggregats;

public sealed class Declaration : AggregateRoot<string>
{
    private readonly List<DeclarationItem> _items = [];

    internal Declaration(string identifier, EmployerIdentifier employerIdentifier, DeclarationPeriod period)
        : base(identifier)
    {
        EmployerIdentifier = employerIdentifier;
        Period = period;
    }

    public string Identifier => Id;

    public EmployerIdentifier EmployerIdentifier { get; }

    public DeclarationPeriod Period { get; }

    public IReadOnlyCollection<DeclarationItem> Items => _items.AsReadOnly();

    public bool IsPublished { get; private set; }

    public decimal TotalAmount => _items.Sum(item => item.Amount);

    internal void AddItem(DeclarationItem item)
    {
        if (IsPublished)
        {
            throw new InvalidOperationException("Une déclaration publiée ne peut plus être modifiée.");
        }

        _items.Add(item);
    }

    public void Publish()
    {
        if (IsPublished)
        {
            throw new InvalidOperationException("La déclaration a déjà été publiée.");
        }

        if (_items.Count == 0)
        {
            throw new InvalidOperationException("Une déclaration doit contenir au moins un élément.");
        }

        IsPublished = true;
        AddDomainEvent(new DeclarationPublishedEvent(Identifier, EmployerIdentifier.Value, Period.ToString(), TotalAmount));
    }

    public static Declaration Restore(
        string identifier,
        EmployerIdentifier employerIdentifier,
        DeclarationPeriod period,
        IReadOnlyCollection<DeclarationItem> items,
        bool isPublished)
    {
        var declaration = new Declaration(identifier, employerIdentifier, period);

        foreach (var item in items)
        {
            declaration._items.Add(item);
        }

        declaration.IsPublished = isPublished;
        declaration.ClearDomainEvents();
        return declaration;
    }
}
