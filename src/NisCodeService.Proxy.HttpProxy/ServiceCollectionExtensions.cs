namespace NisCodeService.Proxy.HttpProxy;

using System;
using Abstractions;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHttpProxyNisCodeService(
        this IServiceCollection services,
        string baseUrl)
    {
        services.AddHttpClient<INisCodeService, HttpProxyNisCodeService>(c =>
        {
            c.BaseAddress = new Uri(baseUrl.TrimEnd('/'));
        });

        return services;
    }
}
