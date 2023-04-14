namespace NisCodeService.Sync.OrganizationRegistry
{
    using System.Net.Http;
    using Abstractions;
    using Infrastructure;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    public class OrganizationRegistryNisCodeReaderFactory : INisCodeReaderFactory
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IOptions<ServiceOptions> _serviceOptions;
        private readonly ILoggerFactory _loggerFactory;

        public OrganizationRegistryNisCodeReaderFactory(IHttpClientFactory httpClientFactory, IOptions<ServiceOptions> serviceOptions, ILoggerFactory loggerFactory)
        {
            _httpClientFactory = httpClientFactory;
            _serviceOptions = serviceOptions;
            _loggerFactory = loggerFactory;
        }

        public INisCodeReader CreateReader()
        {
            return new OrganizationRegistryNisCodeReader(_httpClientFactory, _serviceOptions, _loggerFactory);
        }
    }
}
