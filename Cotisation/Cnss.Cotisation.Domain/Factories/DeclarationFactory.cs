using Cnss.Cotisation.Domain.Aggregats;
using Cnss.Cotisation.Domain.Entities;
using Cnss.Cotisation.Domain.ValuesObject;

namespace Cnss.Cotisation.Domain.Factories;

public sealed class DeclarationFactory
{
    public Declaration Create(DeclarationData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        var declaration = new Declaration(
            $"DEC-{Guid.NewGuid():N}"[..16],
            new EmployerIdentifier(data.EmployerIdentifier),
            new DeclarationPeriod(data.Year, data.Month));

        foreach (var item in data.Items)
        {
            declaration.AddItem(new DeclarationItem(
                $"DIT-{Guid.NewGuid():N}"[..16],
                item.EmployeeIdentifier,
                item.GrossSalary));
        }

        return declaration;
    }
}
