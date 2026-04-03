using Cnss.Cotisation.Application;
using Cnss.Cotisation.Domain.Repositories;
using Cnss.Cotisation.Infrastructure.Persistence;
using Cnss.Cotisation.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Cnss.Cotisation.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddCotisationInfrastructureLayer(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddCotisationApplicationLayer();

        services.AddDbContext<CotisationDbContext>(options =>
            options.UseNpgsql(
                connectionString,
                npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", CotisationDbContext.Schema)));

        services.AddScoped<IDeclarationRepository, DeclarationRepository>();

        return services;
    }
}
