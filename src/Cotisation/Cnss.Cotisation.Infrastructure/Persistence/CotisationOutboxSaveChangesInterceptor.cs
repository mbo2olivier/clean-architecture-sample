using Cnss.Shared.Infrastructure.Persistence;

namespace Cnss.Cotisation.Infrastructure.Persistence;

public sealed class CotisationOutboxSaveChangesInterceptor : OutboxSaveChangesInterceptor<CotisationOutboxMessageRecord>
{
    protected override string BoundaryName => "cotisation";
}
