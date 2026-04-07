using Cnss.Affiliation.Infrastructure.Configuration;
using Cnss.Affiliation.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Cnss.Affiliation.Infrastructure.Messaging;

public sealed class AffiliationOutboxProcessor : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly RabbitMqOptions _options;
    private readonly ILogger<AffiliationOutboxProcessor> _logger;

    public AffiliationOutboxProcessor(
        IServiceScopeFactory serviceScopeFactory,
        RabbitMqOptions options,
        ILogger<AffiliationOutboxProcessor> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _options = options;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(_options.PollingIntervalInSeconds));

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessBatchAsync(stoppingToken);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to process the affiliation outbox.");
            }

            await timer.WaitForNextTickAsync(stoppingToken);
        }
    }

    private async Task ProcessBatchAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AffiliationDbContext>();
        var publisher = scope.ServiceProvider.GetRequiredService<AffiliationOutboxPublisher>();
        var now = DateTime.UtcNow;

        var candidateIds = await dbContext.OutboxMessages
            .AsNoTracking()
            .Where(x =>
                x.Status != OutboxMessageStatus.Processed &&
                (x.LockedUntilUtc == null || x.LockedUntilUtc < now))
            .OrderBy(x => x.OccurredOnUtc)
            .Take(_options.BatchSize)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        foreach (var candidateId in candidateIds)
        {
            var locked = await dbContext.OutboxMessages
                .Where(x =>
                    x.Id == candidateId &&
                    x.Status != OutboxMessageStatus.Processed &&
                    (x.LockedUntilUtc == null || x.LockedUntilUtc < now))
                .ExecuteUpdateAsync(
                    setters => setters
                        .SetProperty(x => x.Status, OutboxMessageStatus.Processing)
                        .SetProperty(x => x.AttemptCount, x => x.AttemptCount + 1)
                        .SetProperty(x => x.ProcessingStartedOnUtc, now)
                        .SetProperty(x => x.LockedUntilUtc, now.AddSeconds(_options.LockDurationInSeconds)),
                    cancellationToken);

            if (locked != 1)
            {
                continue;
            }

            var message = await dbContext.OutboxMessages.SingleAsync(x => x.Id == candidateId, cancellationToken);

            try
            {
                await publisher.PublishAsync(message.RoutingKey, message.Payload, message.EventType, cancellationToken);

                await dbContext.OutboxMessages
                    .Where(x => x.Id == candidateId)
                    .ExecuteUpdateAsync(
                        setters => setters
                            .SetProperty(x => x.Status, OutboxMessageStatus.Processed)
                            .SetProperty(x => x.ProcessedOnUtc, DateTime.UtcNow)
                            .SetProperty(x => x.LockedUntilUtc, (DateTime?)null)
                            .SetProperty(x => x.LastError, (string?)null),
                        cancellationToken);
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    exception,
                    "Failed to publish affiliation outbox message {OutboxMessageId}.",
                    candidateId);

                await dbContext.OutboxMessages
                    .Where(x => x.Id == candidateId)
                    .ExecuteUpdateAsync(
                        setters => setters
                            .SetProperty(x => x.Status, OutboxMessageStatus.Failed)
                            .SetProperty(x => x.LockedUntilUtc, (DateTime?)null)
                            .SetProperty(x => x.LastError, exception.Message),
                        cancellationToken);
            }
        }
    }
}
