using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using NisCodeService.Abstractions;
using Xunit;

namespace NisCodeService.Sync.OrganizationRegistry.Tests
{
    public class OrganizationRegistryNisCodeReaderTests
    {
        [Theory]
        [InlineData("OVO002067", "44021")]
        [InlineData("002007", "13002")]
        public async Task ReadNisCodes(string? ovoCode, string expectedResult)
        {
            ovoCode = ovoCode.WithoutOvoPrefix();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddHttpClient();
            var services = serviceCollection.BuildServiceProvider();

            INisCodeReader reader = new OrganizationRegistryNisCodeReader(services.GetRequiredService<IHttpClientFactory>());
            var dictionary = new Dictionary<string, string>();
            await reader.ReadNisCodes(dictionary, new NullLoggerFactory(), CancellationToken.None);

            Assert.True(dictionary.ContainsKey(ovoCode ?? "bad ovo code"));
            Assert.Equal(expectedResult, dictionary[ovoCode ?? "bad ovo code"]);
        }
    }
}