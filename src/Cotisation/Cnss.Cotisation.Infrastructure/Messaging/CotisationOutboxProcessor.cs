using Cnss.Cotisation.Infrastructure.Persistence;
using Cnss.Shared.Infrastructure.Configuration;
using Cnss.Shared.Infrastructure.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Cnss.Cotisation.Infrastructure.Messaging;

public sealed class CotisationOutboxProcessor : OutboxProcessor<CotisationDbContext, CotisationOutboxMessageRecord>
{
    public CotisationOutboxProcessor(
        IServiceScopeFactory serviceScopeFactory,
        RabbitMqOptions options,
        CotisationOutboxPublisher publisher,
        ILogger<CotisationOutboxProcessor> logger)
        : base(serviceScopeFactory, options, publisher, logger)
    {
    }

    protected override string BoundaryName => "cotisation";

    protected override IQueryable<CotisationOutboxMessageRecord> OutboxMessages(CotisationDbContext dbContext) => dbContext.OutboxMessages;
}
