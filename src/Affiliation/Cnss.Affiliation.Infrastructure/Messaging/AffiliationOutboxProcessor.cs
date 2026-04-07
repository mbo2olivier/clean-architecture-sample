using Cnss.Affiliation.Infrastructure.Persistence;
using Cnss.Shared.Infrastructure.Configuration;
using Cnss.Shared.Infrastructure.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Cnss.Affiliation.Infrastructure.Messaging;

public sealed class AffiliationOutboxProcessor : OutboxProcessor<AffiliationDbContext, AffiliationOutboxMessageRecord>
{
    public AffiliationOutboxProcessor(
        IServiceScopeFactory serviceScopeFactory,
        RabbitMqOptions options,
        AffiliationOutboxPublisher publisher,
        ILogger<AffiliationOutboxProcessor> logger)
        : base(serviceScopeFactory, options, publisher, logger)
    {
    }

    protected override string BoundaryName => "affiliation";

    protected override IQueryable<AffiliationOutboxMessageRecord> OutboxMessages(AffiliationDbContext dbContext) => dbContext.OutboxMessages;
}
