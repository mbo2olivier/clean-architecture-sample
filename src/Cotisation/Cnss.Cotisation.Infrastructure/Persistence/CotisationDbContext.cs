using Cnss.Cotisation.Domain.Aggregats;
using Cnss.Cotisation.Domain.Entities;
using Cnss.Cotisation.Domain.ValuesObject;
using Microsoft.EntityFrameworkCore;

namespace Cnss.Cotisation.Infrastructure.Persistence;

public sealed class CotisationDbContext : DbContext
{
    public const string Schema = "cotisation";

    public CotisationDbContext(DbContextOptions<CotisationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Declaration> Declarations => Set<Declaration>();

    public DbSet<DeclarationItem> DeclarationItems => Set<DeclarationItem>();

    public DbSet<CotisationOutboxMessageRecord> OutboxMessages => Set<CotisationOutboxMessageRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);

        modelBuilder.Entity<Declaration>(builder =>
        {
            builder.ToTable("cot_declarations");

            builder.HasKey(x => x.Id);

            builder.Ignore(x => x.Identifier);
            builder.Ignore(x => x.DomainEvents);
            builder.Ignore(x => x.TotalAmount);

            builder.Property(x => x.Id)
                .HasColumnName("cot_declaration_identifier")
                .HasMaxLength(50);

            builder.OwnsOne(x => x.EmployerIdentifier, employerIdentifierBuilder =>
            {
                employerIdentifierBuilder.WithOwner().HasForeignKey("DeclarationId");
                employerIdentifierBuilder.Property(x => x.Value)
                    .HasColumnName("cot_declaration_employer_identifier")
                    .HasMaxLength(50)
                    .IsRequired();
                employerIdentifierBuilder.HasData(new
                {
                    DeclarationId = "DEC-0001",
                    Value = "EMP-0001"
                });
            });

            builder.OwnsOne(x => x.Period, periodBuilder =>
            {
                periodBuilder.WithOwner().HasForeignKey("DeclarationId");
                periodBuilder.Property(x => x.Year)
                    .HasColumnName("cot_declaration_year")
                    .IsRequired();
                periodBuilder.Property(x => x.Month)
                    .HasColumnName("cot_declaration_month")
                    .IsRequired();
                periodBuilder.HasData(new
                {
                    DeclarationId = "DEC-0001",
                    Year = 2026,
                    Month = 3
                });
            });

            builder.Property(x => x.IsPublished)
                .HasColumnName("cot_declaration_is_published")
                .IsRequired();

            builder.HasMany(x => x.Items)
                .WithOne()
                .HasForeignKey(x => x.DeclarationIdentifier)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(x => x.Items)
                .UsePropertyAccessMode(PropertyAccessMode.Field);

        });

        modelBuilder.Entity<DeclarationItem>(builder =>
        {
            builder.ToTable("cot_declaration_items");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("cot_declaration_item_identifier")
                .HasMaxLength(50);

            builder.Property(x => x.DeclarationIdentifier)
                .HasColumnName("cot_declaration_item_declaration_identifier")
                .HasMaxLength(50)
                .IsRequired();

            builder.Ignore(x => x.Amount);

            builder.Property(x => x.EmployeeIdentifier)
                .HasColumnName("cot_declaration_item_employee_identifier")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.GrossSalary)
                .HasColumnName("cot_declaration_item_gross_salary")
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

        modelBuilder.Entity<Declaration>().HasData(
            new
            {
                Id = "DEC-0001",
                IsPublished = true
            });

        modelBuilder.Entity<DeclarationItem>().HasData(
            new
            {
                Id = "DIT-0001",
                DeclarationIdentifier = "DEC-0001",
                EmployeeIdentifier = "SAL-0001",
                GrossSalary = 1500m
            },
            new
            {
                Id = "DIT-0002",
                DeclarationIdentifier = "DEC-0001",
                EmployeeIdentifier = "SAL-0002",
                GrossSalary = 2000m
            });
    }
}
