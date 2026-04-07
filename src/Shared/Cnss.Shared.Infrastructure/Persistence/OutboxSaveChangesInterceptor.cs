using System.Text.Json;
using Cnss.Shared.Domain.Aggregats;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Cnss.Shared.Infrastructure.Persistence;

public abstract class OutboxSaveChangesInterceptor<TOutboxMessageRecord> : SaveChangesInterceptor
    where TOutboxMessageRecord : OutboxMessageRecord, new()
{
    protected abstract string BoundaryName { get; }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        PersistOutboxMessages(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        PersistOutboxMessages(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void PersistOutboxMessages(DbContext? dbContext)
    {
        if (dbContext is null)
        {
            return;
        }

        var aggregateRoots = dbContext.ChangeTracker
            .Entries()
            .Select(entry => entry.Entity)
            .OfType<IAggregateRoot>()
            .Where(aggregateRoot => aggregateRoot.DomainEvents.Count > 0)
            .Distinct()
            .ToArray();

        if (aggregateRoots.Length == 0)
        {
            return;
        }

        var outboxMessages = aggregateRoots
            .SelectMany(aggregateRoot => aggregateRoot.DomainEvents)
            .Select(domainEvent =>
            {
                var eventType = domainEvent.GetType();

                return new TOutboxMessageRecord
                {
                    Id = Guid.NewGuid(),
                    EventType = eventType.FullName ?? eventType.Name,
                    RoutingKey = BuildRoutingKey(eventType),
                    Payload = JsonSerializer.Serialize((object)domainEvent, eventType),
                    OccurredOnUtc = domainEvent.OccurredOn,
                    Status = OutboxMessageStatus.Pending
                };
            })
            .ToArray();

        dbContext.Set<TOutboxMessageRecord>().AddRange(outboxMessages);

        foreach (var aggregateRoot in aggregateRoots)
        {
            aggregateRoot.ClearDomainEvents();
        }
    }

    private string BuildRoutingKey(Type eventType)
    {
        var eventName = eventType.Name.EndsWith("Event", StringComparison.Ordinal)
            ? eventType.Name[..^"Event".Length]
            : eventType.Name;

        return $"{BoundaryName}.{ToKebabCase(eventName)}";
    }

    private static string ToKebabCase(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var characters = new List<char>(value.Length + 8);

        for (var index = 0; index < value.Length; index++)
        {
            var character = value[index];

            if (char.IsUpper(character) && index > 0)
            {
                characters.Add('-');
            }

            characters.Add(char.ToLowerInvariant(character));
        }

        return new string(characters.ToArray());
    }
}
