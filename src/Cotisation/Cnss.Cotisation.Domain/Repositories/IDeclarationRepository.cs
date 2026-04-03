using Cnss.Cotisation.Domain.Aggregats;

namespace Cnss.Cotisation.Domain.Repositories;

public interface IDeclarationRepository
{
    Task AddAsync(Declaration declaration, CancellationToken cancellationToken = default);

    Task<Declaration?> GetAsync(string declarationIdentifier, CancellationToken cancellationToken = default);
}
