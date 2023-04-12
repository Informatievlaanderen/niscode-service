namespace NisCodeService.Sync.OrganizationRegistry.Tests
{
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions;
    using Extensions;
    using Infrastructure;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging.Abstractions;
    using Microsoft.Extensions.Options;
    using Xunit;

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

            IOptions<ServiceOptions> options = Options.Create<ServiceOptions>(new ServiceOptions()
            {
                OrganizationRegistrySyncUrl = "https://api.wegwijs.vlaanderen.be/v1/search/organisations"
            });

            INisCodeReader reader = new OrganizationRegistryNisCodeReader(
                services.GetRequiredService<IHttpClientFactory>(),
                options,
                new NullLoggerFactory());

            var dictionary = await reader.ReadNisCodes(CancellationToken.None);

            Assert.True(dictionary.ContainsKey(ovoCode ?? "bad ovo code"));
            Assert.Equal(expectedResult, dictionary[ovoCode ?? "bad ovo code"]);
        }
    }
}
