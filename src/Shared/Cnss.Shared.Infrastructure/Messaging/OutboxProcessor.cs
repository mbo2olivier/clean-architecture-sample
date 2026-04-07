using Cnss.Shared.Infrastructure.Configuration;
using Cnss.Shared.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Cnss.Shared.Infrastructure.Messaging;

public abstract class OutboxProcessor<TDbContext, TOutboxMessageRecord> : BackgroundService
    where TDbContext : DbContext
    where TOutboxMessageRecord : OutboxMessageRecord
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly RabbitMqOptions _options;
    private readonly RabbitMqOutboxPublisher _publisher;
    private readonly ILogger _logger;

    protected OutboxProcessor(
        IServiceScopeFactory serviceScopeFactory,
        RabbitMqOptions options,
        RabbitMqOutboxPublisher publisher,
        ILogger logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _options = options;
        _publisher = publisher;
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
                _logger.LogError(exception, "Failed to process the outbox for boundary {BoundaryName}.", BoundaryName);
            }

            await timer.WaitForNextTickAsync(stoppingToken);
        }
    }

    protected abstract string BoundaryName { get; }

    protected abstract IQueryable<TOutboxMessageRecord> OutboxMessages(TDbContext dbContext);

    private async Task ProcessBatchAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();
        var now = DateTime.UtcNow;
        var messages = OutboxMessages(dbContext);

        var candidateIds = await messages
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
            var locked = await messages
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

            var message = await messages.SingleAsync(x => x.Id == candidateId, cancellationToken);

            try
            {
                await _publisher.PublishAsync(message.RoutingKey, message.Payload, message.EventType, cancellationToken);

                await messages
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
                    "Failed to publish outbox message {OutboxMessageId} for boundary {BoundaryName}.",
                    candidateId,
                    BoundaryName);

                await messages
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
