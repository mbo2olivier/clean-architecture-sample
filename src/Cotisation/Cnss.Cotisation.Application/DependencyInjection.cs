using Cnss.Cotisation.Application.SubmitDeclaration;
using Cnss.Cotisation.Domain.Factories;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Cnss.Cotisation.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddCotisationApplicationLayer(this IServiceCollection services)
    {
        services.AddMDiator(typeof(DependencyInjection).Assembly);

        services.AddScoped<DeclarationFactory>();

        services.AddTransient<IValidator<SubmitDeclarationRequest>, SubmitDeclarationRequestValidator>();

        return services;
    }
}
