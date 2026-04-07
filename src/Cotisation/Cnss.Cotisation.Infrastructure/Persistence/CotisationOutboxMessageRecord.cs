namespace Cnss.Cotisation.Infrastructure.Persistence;

public sealed class CotisationOutboxMessageRecord
{
    public Guid Id { get; set; }

    public string EventType { get; set; } = string.Empty;

    public string RoutingKey { get; set; } = string.Empty;

    public string Payload { get; set; } = string.Empty;

    public DateTime OccurredOnUtc { get; set; }

    public string Status { get; set; } = OutboxMessageStatus.Pending;

    public int AttemptCount { get; set; }

    public DateTime? ProcessingStartedOnUtc { get; set; }

    public DateTime? LockedUntilUtc { get; set; }

    public DateTime? ProcessedOnUtc { get; set; }

    public string? LastError { get; set; }
}
