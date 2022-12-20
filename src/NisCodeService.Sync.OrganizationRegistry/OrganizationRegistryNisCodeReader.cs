using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NisCodeService.Abstractions;
using NisCodeService.Sync.OrganizationRegistry.Models;

namespace NisCodeService.Sync.OrganizationRegistry
{
    public class OrganizationRegistryNisCodeReader : INisCodeReader
    {
        private readonly IHttpClientFactory _factory;

        public OrganizationRegistryNisCodeReader(IHttpClientFactory factory)
        {
            _factory = factory;
        }

        public async Task ReadNisCodes(IDictionary<string, string> cache, ILoggerFactory loggerFactory, CancellationToken cancellationToken = default)
        {
            var logger = loggerFactory.CreateLogger<OrganizationRegistryNisCodeReader>();
            logger.LogInformation("Refresh cache: started at {dateTime}", DateTime.UtcNow);

            cache.Clear();

            const string url = "https://api.wegwijs.vlaanderen.be/v1/search/organisations?q=keys.keyTypeName:NIS&fields=keys,ovoNumber&scroll=true";

            var httpClient = _factory.CreateClient();
            var response = await httpClient.GetAsync(url, cancellationToken);

            var scrollId = string.Empty;
            var totalItems = 0;
            while (cache.Count <= totalItems)
            {
                if (response.Headers.TryGetValues("x-search-metadata", out var metadataJson))
                {
                    var metadata = JsonConvert.DeserializeObject<SearchMetadata>(metadataJson.First());
                    if (metadata is not null)
                    {
                        scrollId = metadata.ScrollId;
                        totalItems = metadata.TotalItems;
                    }
                    await InternalReadNisCodes(response, cache, cancellationToken);

                    if (cache.Count == totalItems)
                    {
                        break;
                    }

                    response = await httpClient.GetAsync($"https://api.wegwijs.vlaanderen.be/v1/search/organisations/scroll?id={scrollId}", cancellationToken);
                }
                else
                {
                    await InternalReadNisCodes(response, cache, cancellationToken);
                }
            }

            logger.LogInformation("Refresh cache: ended at {dateTime}", DateTime.UtcNow);
        }

        private async Task InternalReadNisCodes(HttpResponseMessage response, IDictionary<string, string> cache, CancellationToken cancellationToken)
        {
            var results = await response.Content.ReadFromJsonAsync<IEnumerable<Organization>?>(cancellationToken: cancellationToken);
            if (results == null)
            {
                return;
            }

            foreach (var organization in results)
            {
                AddOrganizationToCache(cache, organization);
            }
        }

        private void AddOrganizationToCache(IDictionary<string, string> cache, Organization organization)
        {
            var ovoCode = GetOvoCode(organization).WithoutOvoPrefix();
            if (ovoCode is null)
            {
                return;
            }

            var nisCode = GetNisCode(organization);
            if (nisCode is null)
            {
                return;
            }

            cache[ovoCode] = nisCode;
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

            var firstNisCode = organization.Keys.FirstOrDefault(x =>
                x.KeyTypeName.Equals("NIS", StringComparison.InvariantCultureIgnoreCase));

            var result = firstNisCode?.Value;
            return result;
        }
    }
}