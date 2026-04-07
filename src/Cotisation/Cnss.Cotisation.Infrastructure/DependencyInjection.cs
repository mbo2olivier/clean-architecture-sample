using Cnss.Cotisation.Application;
using Cnss.Cotisation.Infrastructure.Messaging;
using Cnss.Cotisation.Domain.Repositories;
using Cnss.Cotisation.Infrastructure.Persistence;
using Cnss.Cotisation.Infrastructure.Repositories;
using Cnss.Shared.Infrastructure.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cnss.Cotisation.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddCotisationInfrastructureLayer(
        this IServiceCollection services,
        IConfiguration configuration,
        bool enableOutboxProcessor = true)
    {
        services.AddCotisationApplicationLayer();

        var connectionString = configuration.GetConnectionString("Database")
            ?? "Host=localhost;Port=5432;Database=cnss;Username=cnss;Password=cnss";
        var rabbitMqOptions = configuration.GetSection("RabbitMq").Get<RabbitMqOptions>() ?? new RabbitMqOptions();

        services.AddScoped<CotisationOutboxSaveChangesInterceptor>();

        services.AddDbContext<CotisationDbContext>((serviceProvider, options) =>
            options.UseNpgsql(
                connectionString,
                npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", CotisationDbContext.Schema))
                .AddInterceptors(serviceProvider.GetRequiredService<CotisationOutboxSaveChangesInterceptor>()));

        services.AddSingleton(rabbitMqOptions);
        services.AddScoped<CotisationOutboxPublisher>();
        services.AddScoped<IDeclarationRepository, DeclarationRepository>();

        if (enableOutboxProcessor)
        {
            services.AddHostedService<CotisationOutboxProcessor>();
        }

        return services;
    }
}
