using Cnss.Shared.Application.GetEmployerDetails;
using Cnss.Shared.Application.GetEmployerEmployeesDetails;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Cnss.Shared.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddSharedApplicationLayer(this IServiceCollection services)
    {
        services.AddMDiator(typeof(DependencyInjection).Assembly);

        services.AddTransient<IValidator<GetEmployerDetailsRequest>, GetEmployerDetailsRequestValidator>();
        services.AddTransient<IValidator<GetEmployerEmployeesDetailsRequest>, GetEmployerEmployeesDetailsRequestValidator>();

        return services;
    }
}
