using Microsoft.Extensions.DependencyInjection;
using NisCodeService.Abstractions;

namespace NisCodeService.Sync.OrganizationRegistry.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddOrganizationRegistryNisCodeReader(this IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddSingleton<INisCodeReader, OrganizationRegistryNisCodeReader>();
            services.AddSingleton<INisCodeReaderFactory, OrganizationRegistryNisCodeReaderFactory>();

            return services;
        }
    }
}
