using Cnss.Cotisation.Domain.Aggregats;
using Cnss.Cotisation.Domain.Repositories;
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
        await _dbContext.Declarations.AddAsync(declaration, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Declaration?> GetAsync(string declarationIdentifier, CancellationToken cancellationToken = default)
    {
        var declarationRecord = await _dbContext.Declarations
            .AsNoTracking()
            .Include(x => x.Items)
            .SingleOrDefaultAsync(x => x.Id == declarationIdentifier, cancellationToken);

        return declarationRecord;
    }
}
