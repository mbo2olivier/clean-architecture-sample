using Cnss.Cotisation.Domain.Aggregats;
using Cnss.Cotisation.Domain.Entities;
using Cnss.Cotisation.Domain.Repositories;
using Cnss.Cotisation.Domain.ValuesObject;
using Cnss.Cotisation.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Cnss.Cotisation.Infrastructure.Repositories;

public sealed class DeclarationRepository : IDeclarationRepository
{
    private readonly CotisationDbContext _dbContext;

    public DeclarationRepository(CotisationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Declaration declaration, CancellationToken cancellationToken = default)
    {
        await _dbContext.Declarations.AddAsync(MapDeclaration(declaration), cancellationToken);
        _dbContext.EnqueueOutboxMessages(declaration.DomainEvents);
        await _dbContext.SaveChangesAsync(cancellationToken);
        declaration.ClearDomainEvents();
    }

    public async Task<Declaration?> GetAsync(string declarationIdentifier, CancellationToken cancellationToken = default)
    {
        var declarationRecord = await _dbContext.Declarations
            .AsNoTracking()
            .Include(x => x.Items)
            .SingleOrDefaultAsync(x => x.Identifier == declarationIdentifier, cancellationToken);

        return declarationRecord is null ? null : MapDeclaration(declarationRecord);
    }

    private static CotisationDeclarationRecord MapDeclaration(Declaration declaration)
    {
        return new CotisationDeclarationRecord
        {
            Identifier = declaration.Identifier,
            EmployerIdentifier = declaration.EmployerIdentifier.Value,
            Year = declaration.Period.Year,
            Month = declaration.Period.Month,
            IsPublished = declaration.IsPublished,
            Items = declaration.Items
                .Select(item => new CotisationDeclarationItemRecord
                {
                    Identifier = item.Id,
                    DeclarationIdentifier = declaration.Identifier,
                    EmployeeIdentifier = item.EmployeeIdentifier,
                    GrossSalary = item.GrossSalary,
                    Amount = item.Amount
                })
                .ToList()
        };
    }

    private static Declaration MapDeclaration(CotisationDeclarationRecord declarationRecord)
    {
        var items = declarationRecord.Items
            .Select(item => new DeclarationItem(
                item.Identifier,
                item.EmployeeIdentifier,
                item.GrossSalary))
            .ToArray();

        return Declaration.Restore(
            declarationRecord.Identifier,
            new EmployerIdentifier(declarationRecord.EmployerIdentifier),
            new DeclarationPeriod(declarationRecord.Year, declarationRecord.Month),
            items,
            declarationRecord.IsPublished);
    }
}
