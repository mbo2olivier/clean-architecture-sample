using Cnss.Affiliation.Application;
using Cnss.Affiliation.Domain.Repositories;
using Cnss.Affiliation.Infrastructure.Persistence;
using Cnss.Affiliation.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Cnss.Affiliation.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddAffiliationInfrastructureLayer(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddAffiliationApplicationLayer();

        services.AddDbContext<AffiliationDbContext>(options =>
            options.UseNpgsql(
                connectionString,
                npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", AffiliationDbContext.Schema)));

        services.AddScoped<IAffiliationRepository, AffiliationRepository>();

        return services;
    }
}
