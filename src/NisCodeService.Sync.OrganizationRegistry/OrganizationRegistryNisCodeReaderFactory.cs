using System;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using NisCodeService.Abstractions;

namespace NisCodeService.Sync.OrganizationRegistry
{
    public class OrganizationRegistryNisCodeReaderFactory : INisCodeReaderFactory 
    {
        public INisCodeReader CreateReader(IServiceProvider services)
        {
            return new OrganizationRegistryNisCodeReader(services.GetRequiredService<IHttpClientFactory>());
        }
    }
}
