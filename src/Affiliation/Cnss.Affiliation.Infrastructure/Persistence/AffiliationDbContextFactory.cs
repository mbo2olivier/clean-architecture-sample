using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Cnss.Affiliation.Infrastructure.Persistence;

public sealed class AffiliationDbContextFactory : IDesignTimeDbContextFactory<AffiliationDbContext>
{
    public AffiliationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AffiliationDbContext>();
        optionsBuilder.UseNpgsql(
            ResolveConnectionString(),
            npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", AffiliationDbContext.Schema));

        return new AffiliationDbContext(optionsBuilder.Options);
    }

    private static string ResolveConnectionString()
    {
        return Environment.GetEnvironmentVariable("CNSS_DATABASE_CONNECTION_STRING")
            ?? "Host=localhost;Port=5432;Database=cnss;Username=cnss;Password=cnss";
    }
}
