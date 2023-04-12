namespace NisCodeService.IntegrationTests
{
    using System.Collections.Generic;
    using System.Net.Http.Json;
    using System.Threading.Tasks;
    using Abstractions;
    using Extensions;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.Extensions.Logging;
    using Microsoft.VisualStudio.TestPlatform.TestHost;
    using Sync.OrganizationRegistry.Extensions;
    using Xunit;

    public class NisCodeServiceTests
    {
        private readonly TestServer _testServer;

        public NisCodeServiceTests()
        {
            var hostBuilder = new WebHostBuilder()
                .ConfigureServices(serviceCollection =>
                {
                    serviceCollection
                        .AddOrganizationRegistryNisCodeReader()
                        .AddNisCodeService();
                })
                .UseStartup<Program>()
                .ConfigureLogging(loggingBuilder => loggingBuilder.AddConsole())
                .UseTestServer();

            _testServer = new TestServer(hostBuilder);
        }

        [Theory]
        [InlineData("OVO002067", "44021")]
        [InlineData("002007", "13002")]
        public async Task GetAllNisCodes(string ovoCode, string expectedResult)
        {
            var client = _testServer.CreateClient();
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
            var client = _testServer.CreateClient();
            var result = await client.GetStringAsync($"/niscode/{ovoCode}");
            Assert.Equal(expectedResult, result);
        }
    }
}
