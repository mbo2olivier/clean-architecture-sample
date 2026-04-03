using Microsoft.EntityFrameworkCore;

namespace Cnss.Affiliation.Infrastructure.Persistence;

public sealed class AffiliationDbContext : DbContext
{
    public const string Schema = "affiliation";

    public AffiliationDbContext(DbContextOptions<AffiliationDbContext> options)
        : base(options)
    {
    }

    public DbSet<AffiliationEmployerRecord> Employers => Set<AffiliationEmployerRecord>();

    public DbSet<AffiliationEmployeeRecord> Employees => Set<AffiliationEmployeeRecord>();

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
}
