using System.Text.Json;
using Cnss.Shared.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace Cnss.Affiliation.Infrastructure.Persistence;

public sealed class AffiliationDbContext : DbContext
{
    public const string Schema = "affiliation";
    private readonly List<AffiliationOutboxMessageRecord> _pendingOutboxMessages = [];

    public AffiliationDbContext(DbContextOptions<AffiliationDbContext> options)
        : base(options)
    {
    }

    public DbSet<AffiliationEmployerRecord> Employers => Set<AffiliationEmployerRecord>();

    public DbSet<AffiliationEmployeeRecord> Employees => Set<AffiliationEmployeeRecord>();

    public DbSet<AffiliationOutboxMessageRecord> OutboxMessages => Set<AffiliationOutboxMessageRecord>();

    public void EnqueueOutboxMessages(IEnumerable<IDomainEvent> domainEvents)
    {
        foreach (var domainEvent in domainEvents)
        {
            var eventType = domainEvent.GetType();

            _pendingOutboxMessages.Add(new AffiliationOutboxMessageRecord
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

        modelBuilder.Entity<AffiliationEmployerRecord>(builder =>
        {
            builder.ToTable("aff_employers");

            builder.HasKey(x => x.Identifier);

            builder.Property(x => x.Identifier)
                .HasColumnName("aff_employer_identifier")
                .HasMaxLength(50);

            builder.Property(x => x.RegistrationNumber)
                .HasColumnName("aff_employer_registration_number")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.CompanyName)
                .HasColumnName("aff_employer_company_name")
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.EmployeeIdentifiers)
                .HasColumnName("aff_employer_employee_identifiers")
                .HasColumnType("text[]")
                .IsRequired();

            builder.HasIndex(x => x.RegistrationNumber)
                .IsUnique();
        });

        modelBuilder.Entity<AffiliationEmployeeRecord>(builder =>
        {
            builder.ToTable("aff_employees");

            builder.HasKey(x => x.Identifier);

            builder.Property(x => x.Identifier)
                .HasColumnName("aff_employee_identifier")
                .HasMaxLength(50);

            builder.Property(x => x.RegistrationNumber)
                .HasColumnName("aff_employee_registration_number")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.FirstName)
                .HasColumnName("aff_employee_first_name")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.LastName)
                .HasColumnName("aff_employee_last_name")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.EmployerIdentifier)
                .HasColumnName("aff_employee_employer_identifier")
                .HasMaxLength(50);

            builder.HasIndex(x => x.RegistrationNumber)
                .IsUnique();

            builder.HasIndex(x => x.EmployerIdentifier);
        });

        modelBuilder.Entity<AffiliationOutboxMessageRecord>(builder =>
        {
            builder.ToTable("aff_outbox_messages");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("aff_outbox_message_id");

            builder.Property(x => x.EventType)
                .HasColumnName("aff_outbox_event_type")
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(x => x.RoutingKey)
                .HasColumnName("aff_outbox_routing_key")
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.Payload)
                .HasColumnName("aff_outbox_payload")
                .HasColumnType("jsonb")
                .IsRequired();

            builder.Property(x => x.OccurredOnUtc)
                .HasColumnName("aff_outbox_occurred_on_utc")
                .IsRequired();

            builder.Property(x => x.Status)
                .HasColumnName("aff_outbox_status")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.AttemptCount)
                .HasColumnName("aff_outbox_attempt_count")
                .IsRequired();

            builder.Property(x => x.ProcessingStartedOnUtc)
                .HasColumnName("aff_outbox_processing_started_on_utc");

            builder.Property(x => x.LockedUntilUtc)
                .HasColumnName("aff_outbox_locked_until_utc");

            builder.Property(x => x.ProcessedOnUtc)
                .HasColumnName("aff_outbox_processed_on_utc");

            builder.Property(x => x.LastError)
                .HasColumnName("aff_outbox_last_error")
                .HasMaxLength(4000);

            builder.HasIndex(x => new { x.Status, x.OccurredOnUtc });
            builder.HasIndex(x => x.LockedUntilUtc);
        });

        modelBuilder.Entity<AffiliationEmployerRecord>().HasData(
            new AffiliationEmployerRecord
            {
                Identifier = "EMP-0001",
                RegistrationNumber = "RCCM-001",
                CompanyName = "ACME SARL",
                EmployeeIdentifiers = ["SAL-0001", "SAL-0002"]
            });

        modelBuilder.Entity<AffiliationEmployeeRecord>().HasData(
            new AffiliationEmployeeRecord
            {
                Identifier = "SAL-0001",
                RegistrationNumber = "MAT-001",
                FirstName = "John",
                LastName = "Doe",
                EmployerIdentifier = "EMP-0001"
            },
            new AffiliationEmployeeRecord
            {
                Identifier = "SAL-0002",
                RegistrationNumber = "MAT-002",
                FirstName = "Jane",
                LastName = "Doe",
                EmployerIdentifier = "EMP-0001"
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

        return $"affiliation.{ToKebabCase(eventName)}";
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
