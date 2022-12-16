using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using NisCodeService.Ëxtensions;
using Xunit;

namespace NisCodeService.IntegrationTests
{
    public class NisCodeServiceTests
    {
        [Theory]
        [InlineData("OVO002067", true)]
        public async Task GetNisCode(string ovoCode, bool expectedResult)
        {
            var application = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services => services
                        .AddNisCodeService());
                });

            var client = application.CreateClient();

            // get
            var s = await client.GetStringAsync($"/niscode/{ovoCode}");
            Assert.Equal(expectedResult, s is not null);
        }
    }
}