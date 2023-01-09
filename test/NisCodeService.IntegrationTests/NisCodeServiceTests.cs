using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using NisCodeService.Extensions;
using NisCodeService.Sync.OrganizationRegistry.Extensions;
using Xunit;

namespace NisCodeService.IntegrationTests
{
    using System.Collections.Generic;
    using System.Net.Http.Json;
    using Abstractions;

    public class NisCodeServiceTests
    {
        private readonly WebApplicationFactory<Program> _application;

        public NisCodeServiceTests()
        {
            _application = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services => services
                        .AddOrganizationRegistryNisCodeReader()
                        .AddNisCodeService());
                });
        }

        [Theory]
        [InlineData("OVO002067", "44021")]
        [InlineData("002007", "13002")]
        public async Task GetAllNisCodes(string ovoCode, string expectedResult)
        {
            var client = _application.CreateClient();
            var response = await client.GetAsync("/niscode");
            Assert.True(response.IsSuccessStatusCode);
            var content = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
            Assert.NotNull(content);
            Assert.Equal(333, content.Count);
            Assert.Equal(expectedResult, content[ovoCode.WithoutOvoPrefix()!]);
        }

        [Theory]
        [InlineData("OVO002067", "44021")]
        [InlineData("002007", "13002")]
        public async Task GetNisCode(string ovoCode, string expectedResult)
        {
            var client = _application.CreateClient();
            var result = await client.GetStringAsync($"/niscode/{ovoCode}");
            Assert.Equal(expectedResult, result);
        }
    }
}
