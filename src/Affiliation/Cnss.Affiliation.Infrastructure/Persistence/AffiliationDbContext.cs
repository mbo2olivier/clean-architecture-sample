using Cnss.Affiliation.Domain.Aggregats;
using Microsoft.EntityFrameworkCore;

namespace Cnss.Affiliation.Infrastructure.Persistence;

public sealed class AffiliationDbContext : DbContext
{
    public const string Schema = "affiliation";

    public AffiliationDbContext(DbContextOptions<AffiliationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Employer> Employers => Set<Employer>();

    public DbSet<Employee> Employees => Set<Employee>();

    public DbSet<AffiliationOutboxMessageRecord> OutboxMessages => Set<AffiliationOutboxMessageRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);

        modelBuilder.Entity<Employer>(builder =>
        {
            builder.ToTable("aff_employers");

            builder.HasKey(x => x.Id);

            builder.Ignore(x => x.Identifier);
            builder.Ignore(x => x.EmployeeIdentifiers);
            builder.Ignore(x => x.DomainEvents);

            builder.Property(x => x.Id)
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

            builder.HasIndex(x => x.RegistrationNumber)
                .IsUnique();
        });

        modelBuilder.Entity<Employee>(builder =>
        {
            builder.ToTable("aff_employees");

            builder.HasKey(x => x.Id);

            builder.Ignore(x => x.Identifier);
            builder.Ignore(x => x.DomainEvents);

            builder.Property(x => x.Id)
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

        modelBuilder.Entity<Employer>().HasData(
            new
            {
                Id = "EMP-0001",
                RegistrationNumber = "RCCM-001",
                CompanyName = "ACME SARL"
            });

        modelBuilder.Entity<Employee>().HasData(
            new
            {
                Id = "SAL-0001",
                RegistrationNumber = "MAT-001",
                FirstName = "John",
                LastName = "Doe",
                EmployerIdentifier = "EMP-0001"
            },
            new
            {
                Id = "SAL-0002",
                RegistrationNumber = "MAT-002",
                FirstName = "Jane",
                LastName = "Doe",
                EmployerIdentifier = "EMP-0001"
            });
    }
}
