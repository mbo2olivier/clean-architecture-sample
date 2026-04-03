using Microsoft.Extensions.DependencyInjection;

namespace Cnss.Shared.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddSharedApplicationLayer(this IServiceCollection services)
    {
        return services;
    }
}
