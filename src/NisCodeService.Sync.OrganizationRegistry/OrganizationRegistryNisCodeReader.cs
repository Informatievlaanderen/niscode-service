namespace NisCodeService.Sync.OrganizationRegistry
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using Infrastructure;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using Abstractions;
    using Models;

    public class OrganizationRegistryNisCodeReader : INisCodeReader
    {
        private readonly IHttpClientFactory _factory;
        private readonly ServiceOptions _serviceOptions;
        private readonly ILoggerFactory _loggerFactory;

        public OrganizationRegistryNisCodeReader(IHttpClientFactory factory, IOptions<ServiceOptions> serviceOptions, ILoggerFactory loggerFactory)
        {
            _factory = factory;
            _serviceOptions = serviceOptions.Value;
            _loggerFactory = loggerFactory;
        }

        public async Task<Dictionary<string, string>> ReadNisCodes(CancellationToken cancellationToken = default)
        {
            var logger = _loggerFactory.CreateLogger<OrganizationRegistryNisCodeReader>();

            var httpClient = _factory.CreateClient();
            var response = await httpClient.GetAsync(CreateSyncUri(), cancellationToken);

            var scrollId = string.Empty;
            var totalItems = 0;

            var ovoNisCodeMapping = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

            while (ovoNisCodeMapping.Count <= totalItems)
            {
                if (response.Headers.TryGetValues("x-search-metadata", out var metadataJson))
                {
                    var metadata = JsonConvert.DeserializeObject<SearchMetadata>(metadataJson.First());
                    if (metadata is not null)
                    {
                        scrollId = metadata.ScrollId;
                        totalItems = metadata.TotalItems;
                    }

                    await InternalReadNisCodes(response, ovoNisCodeMapping, cancellationToken);

                    response = await httpClient.GetAsync(CreateScrollUri(scrollId), cancellationToken);
                }
                else
                {
                    await InternalReadNisCodes(response, ovoNisCodeMapping, cancellationToken);
                }

                if (ovoNisCodeMapping.Count == totalItems)
                {
                    break;
                }
            }

            return ovoNisCodeMapping;
        }

        private Uri CreateSyncUri()
            => new Uri(new Uri(_serviceOptions.OrganizationRegistrySyncUrl), "/v1/search/organisations?q=keys.keyTypeName:NIS&fields=keys,ovoNumber&scroll=true");
        private Uri CreateScrollUri(string scrollId)
            => new Uri(new Uri(_serviceOptions.OrganizationRegistrySyncUrl), $"/v1/search/organisations/scroll?id={scrollId}");

        private async Task InternalReadNisCodes(
            HttpResponseMessage response,
            IDictionary<string, string> ovoNisCodeMapping,
            CancellationToken cancellationToken)
        {
            var results =
                await response.Content.ReadFromJsonAsync<IEnumerable<Organization>?>(
                    cancellationToken: cancellationToken);
            if (results == null)
            {
                return;
            }

            foreach (var organization in results)
            {
                AddOrganizationToCache(ovoNisCodeMapping, organization);
            }
        }

        private void AddOrganizationToCache(IDictionary<string, string> ovoNisCodeMapping, Organization organization)
        {
            var ovoCode = GetOvoCode(organization).WithoutOvoPrefix();
            if (ovoCode is null)
            {
                return;
            }

            var nisCode = GetNisCode(organization);

            // TODO: What to do with Niscode "0" (actual first result)
            if (nisCode is null)
            {
                return;
            }

            ovoNisCodeMapping[ovoCode] = nisCode;
        }

        private string? GetOvoCode(Organization organization)
        {
            ArgumentNullException.ThrowIfNull(organization);
            ArgumentNullException.ThrowIfNull(organization.Keys);

            var ovoCode = organization.OvoNumber;
            return ovoCode;
        }

        private string? GetNisCode(Organization organization)
        {
            ArgumentNullException.ThrowIfNull(organization);
            ArgumentNullException.ThrowIfNull(organization.Keys);

            var firstNisCode = organization.Keys
                .Where(x => x.KeyTypeName is not null)
                .FirstOrDefault(x => x.KeyTypeName!.Equals("NIS", StringComparison.InvariantCultureIgnoreCase));

            var result = firstNisCode?.Value;
            return result;
        }
    }
}
