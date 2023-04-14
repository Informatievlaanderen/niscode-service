namespace NisCodeService.HardCoded.Extensions
{
    using Abstractions;
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHardCodedNisCodeService(this IServiceCollection services)
        {
            services.AddSingleton<INisCodeService, HardCodedNisCodeService>();
            return services;
        }
    }
}
