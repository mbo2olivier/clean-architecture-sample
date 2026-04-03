using Microsoft.EntityFrameworkCore;

namespace Cnss.Cotisation.Infrastructure.Persistence;

public sealed class CotisationDbContext : DbContext
{
    public const string Schema = "cotisation";

    public CotisationDbContext(DbContextOptions<CotisationDbContext> options)
        : base(options)
    {
    }

    public DbSet<CotisationDeclarationRecord> Declarations => Set<CotisationDeclarationRecord>();

    public DbSet<CotisationDeclarationItemRecord> DeclarationItems => Set<CotisationDeclarationItemRecord>();

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
}
