using Microsoft.Extensions.DependencyInjection;
using NisCodeService.Abstractions;

namespace NisCodeService.Sync.HardCoded.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHardCodedNisCodeReader(this IServiceCollection services)
        {
            services.AddSingleton<INisCodeReader, HardCodedNisCodeReader>();
            services.AddSingleton<INisCodeReaderFactory, HardCodedNisCodeReaderFactory>();
            return services;
        }
    }
}
