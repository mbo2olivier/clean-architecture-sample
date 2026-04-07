using System.Text.Json;
using Cnss.Shared.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace Cnss.Cotisation.Infrastructure.Persistence;

public sealed class CotisationDbContext : DbContext
{
    public const string Schema = "cotisation";
    private readonly List<CotisationOutboxMessageRecord> _pendingOutboxMessages = [];

    public CotisationDbContext(DbContextOptions<CotisationDbContext> options)
        : base(options)
    {
    }

    public DbSet<CotisationDeclarationRecord> Declarations => Set<CotisationDeclarationRecord>();

    public DbSet<CotisationDeclarationItemRecord> DeclarationItems => Set<CotisationDeclarationItemRecord>();

    public DbSet<CotisationOutboxMessageRecord> OutboxMessages => Set<CotisationOutboxMessageRecord>();

    public void EnqueueOutboxMessages(IEnumerable<IDomainEvent> domainEvents)
    {
        foreach (var domainEvent in domainEvents)
        {
            var eventType = domainEvent.GetType();

            _pendingOutboxMessages.Add(new CotisationOutboxMessageRecord
            {
                Id = Guid.NewGuid(),
                EventType = eventType.FullName ?? eventType.Name,
                RoutingKey = BuildRoutingKey(eventType),
                Payload = JsonSerializer.Serialize((object)domainEvent, eventType),
                OccurredOnUtc = domainEvent.OccurredOn,
                Status = OutboxMessageStatus.Pending
            });
        }
    }

    public override int SaveChanges()
    {
        PersistOutboxMessages();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        PersistOutboxMessages();
        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);

        modelBuilder.Entity<CotisationDeclarationRecord>(builder =>
        {
            builder.ToTable("cot_declarations");

            builder.HasKey(x => x.Identifier);

            builder.Property(x => x.Identifier)
                .HasColumnName("cot_declaration_identifier")
                .HasMaxLength(50);

            builder.Property(x => x.EmployerIdentifier)
                .HasColumnName("cot_declaration_employer_identifier")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.Year)
                .HasColumnName("cot_declaration_year")
                .IsRequired();

            builder.Property(x => x.Month)
                .HasColumnName("cot_declaration_month")
                .IsRequired();

            builder.Property(x => x.IsPublished)
                .HasColumnName("cot_declaration_is_published")
                .IsRequired();

            builder.HasMany(x => x.Items)
                .WithOne()
                .HasForeignKey(x => x.DeclarationIdentifier)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => new { x.EmployerIdentifier, x.Year, x.Month });
        });

        modelBuilder.Entity<CotisationDeclarationItemRecord>(builder =>
        {
            builder.ToTable("cot_declaration_items");

            builder.HasKey(x => x.Identifier);

            builder.Property(x => x.Identifier)
                .HasColumnName("cot_declaration_item_identifier")
                .HasMaxLength(50);

            builder.Property(x => x.DeclarationIdentifier)
                .HasColumnName("cot_declaration_item_declaration_identifier")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.EmployeeIdentifier)
                .HasColumnName("cot_declaration_item_employee_identifier")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.GrossSalary)
                .HasColumnName("cot_declaration_item_gross_salary")
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x => x.Amount)
                .HasColumnName("cot_declaration_item_amount")
                .HasPrecision(18, 2)
                .IsRequired();

            builder.HasIndex(x => x.DeclarationIdentifier);
        });

        modelBuilder.Entity<CotisationOutboxMessageRecord>(builder =>
        {
            builder.ToTable("cot_outbox_messages");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("cot_outbox_message_id");

            builder.Property(x => x.EventType)
                .HasColumnName("cot_outbox_event_type")
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(x => x.RoutingKey)
                .HasColumnName("cot_outbox_routing_key")
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.Payload)
                .HasColumnName("cot_outbox_payload")
                .HasColumnType("jsonb")
                .IsRequired();

            builder.Property(x => x.OccurredOnUtc)
                .HasColumnName("cot_outbox_occurred_on_utc")
                .IsRequired();

            builder.Property(x => x.Status)
                .HasColumnName("cot_outbox_status")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.AttemptCount)
                .HasColumnName("cot_outbox_attempt_count")
                .IsRequired();

            builder.Property(x => x.ProcessingStartedOnUtc)
                .HasColumnName("cot_outbox_processing_started_on_utc");

            builder.Property(x => x.LockedUntilUtc)
                .HasColumnName("cot_outbox_locked_until_utc");

            builder.Property(x => x.ProcessedOnUtc)
                .HasColumnName("cot_outbox_processed_on_utc");

            builder.Property(x => x.LastError)
                .HasColumnName("cot_outbox_last_error")
                .HasMaxLength(4000);

            builder.HasIndex(x => new { x.Status, x.OccurredOnUtc });
            builder.HasIndex(x => x.LockedUntilUtc);
        });

        modelBuilder.Entity<CotisationDeclarationRecord>().HasData(
            new CotisationDeclarationRecord
            {
                Identifier = "DEC-0001",
                EmployerIdentifier = "EMP-0001",
                Year = 2026,
                Month = 3,
                IsPublished = true
            });

        modelBuilder.Entity<CotisationDeclarationItemRecord>().HasData(
            new CotisationDeclarationItemRecord
            {
                Identifier = "DIT-0001",
                DeclarationIdentifier = "DEC-0001",
                EmployeeIdentifier = "SAL-0001",
                GrossSalary = 1500m,
                Amount = 75m
            },
            new CotisationDeclarationItemRecord
            {
                Identifier = "DIT-0002",
                DeclarationIdentifier = "DEC-0001",
                EmployeeIdentifier = "SAL-0002",
                GrossSalary = 2000m,
                Amount = 100m
            });
    }

    private void PersistOutboxMessages()
    {
        if (_pendingOutboxMessages.Count == 0)
        {
            return;
        }

        OutboxMessages.AddRange(_pendingOutboxMessages);
        _pendingOutboxMessages.Clear();
    }

    private static string BuildRoutingKey(Type eventType)
    {
        var eventName = eventType.Name.EndsWith("Event", StringComparison.Ordinal)
            ? eventType.Name[..^"Event".Length]
            : eventType.Name;

        return $"cotisation.{ToKebabCase(eventName)}";
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
