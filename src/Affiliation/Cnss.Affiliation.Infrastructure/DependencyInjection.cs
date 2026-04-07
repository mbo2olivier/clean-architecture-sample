using Cnss.Affiliation.Application;
using Cnss.Affiliation.Infrastructure.Configuration;
using Cnss.Affiliation.Infrastructure.Messaging;
using Cnss.Affiliation.Domain.Repositories;
using Cnss.Affiliation.Infrastructure.Persistence;
using Cnss.Affiliation.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cnss.Affiliation.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddAffiliationInfrastructureLayer(
        this IServiceCollection services,
        IConfiguration configuration,
        bool enableOutboxProcessor = true)
    {
        services.AddAffiliationApplicationLayer();

        var connectionString = configuration.GetConnectionString("Database")
            ?? "Host=localhost;Port=5432;Database=cnss;Username=cnss;Password=cnss";
        var rabbitMqOptions = configuration.GetSection("RabbitMq").Get<RabbitMqOptions>() ?? new RabbitMqOptions();

        services.AddDbContext<AffiliationDbContext>(options =>
            options.UseNpgsql(
                connectionString,
                npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", AffiliationDbContext.Schema)));

        services.AddSingleton(rabbitMqOptions);
        services.AddScoped<AffiliationOutboxPublisher>();
        services.AddScoped<IAffiliationRepository, AffiliationRepository>();

        if (enableOutboxProcessor)
        {
            services.AddHostedService<AffiliationOutboxProcessor>();
        }

        return services;
    }
}
