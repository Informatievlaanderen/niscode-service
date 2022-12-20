using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using NisCodeService.Extensions;
using NisCodeService.Sync.OrganizationRegistry.Extensions;
using Xunit;

namespace NisCodeService.IntegrationTests
{
    public class NisCodeServiceTests
    {
        [Theory]
        [InlineData("OVO002067", "44021")]
        [InlineData("002007", "13002")]
        public async Task GetNisCode(string ovoCode, string expectedResult)
        {
            var application = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services => services
                        .AddOrganizationRegistryNisCodeReader()
                        .AddNisCodeService());
                });

            var client = application.CreateClient();

            // get
            var result = await client.GetStringAsync($"/niscode/{ovoCode}");
            Assert.Equal(expectedResult, result);
        }
    }
}