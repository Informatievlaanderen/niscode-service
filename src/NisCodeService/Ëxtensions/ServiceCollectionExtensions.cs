using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NisCodeService.Abstractions;

namespace NisCodeService.Ëxtensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddNisCodeServiceHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddCheck("Health", () => HealthCheckResult.Healthy());
        return services;
    }

    public static IServiceCollection AddNisCodeService(this IServiceCollection services)
    {
        services.AddSingleton<INisCodeService, NisCodeService>();
        return services;
    }
}
