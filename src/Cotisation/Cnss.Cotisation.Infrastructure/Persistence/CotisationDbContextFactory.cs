using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Cnss.Cotisation.Infrastructure.Persistence;

public sealed class CotisationDbContextFactory : IDesignTimeDbContextFactory<CotisationDbContext>
{
    public CotisationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<CotisationDbContext>();
        optionsBuilder.UseNpgsql(
            ResolveConnectionString(),
            npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", CotisationDbContext.Schema));

        return new CotisationDbContext(optionsBuilder.Options);
    }

    private static string ResolveConnectionString()
    {
        return Environment.GetEnvironmentVariable("CNSS_DATABASE_CONNECTION_STRING")
            ?? "Host=localhost;Port=5432;Database=cnss;Username=cnss;Password=cnss";
    }
}
