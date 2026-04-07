using Cnss.Shared.Infrastructure.Persistence;

namespace Cnss.Affiliation.Infrastructure.Persistence;

public sealed class AffiliationOutboxSaveChangesInterceptor : OutboxSaveChangesInterceptor<AffiliationOutboxMessageRecord>
{
    protected override string BoundaryName => "affiliation";
}
