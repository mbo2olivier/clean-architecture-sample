using Cnss.Affiliation.Application.AffiliateEmployer;
using Cnss.Affiliation.Application.AttachEmployeeToEmployer;
using Cnss.Affiliation.Application.GetEmployerDetails;
using Cnss.Affiliation.Application.GetEmployerEmployeesDetails;
using Cnss.Affiliation.Domain.Services;
using Cnss.Shared.Application;
using Cnss.Shared.Application.GetEmployerDetails;
using Cnss.Shared.Application.GetEmployerEmployeesDetails;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Cnss.Affiliation.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddAffiliationApplicationLayer(this IServiceCollection services)
    {
        services.AddSharedApplicationLayer();
        services.AddMDiator(typeof(DependencyInjection).Assembly);

        services.AddScoped<IdentifierGenerator>();

        services.AddTransient<IValidator<AffiliateEmployerRequest>, AffiliateEmployerRequestValidator>();
        services.AddTransient<IValidator<AttachEmployeeToEmployerRequest>, AttachEmployeeToEmployerRequestValidator>();
        services.AddTransient<IValidator<GetEmployerDetailsRequest>, GetEmployerDetailsRequestValidator>();
        services.AddTransient<IValidator<GetEmployerEmployeesDetailsRequest>, GetEmployerEmployeesDetailsRequestValidator>();

        return services;
    }
}
