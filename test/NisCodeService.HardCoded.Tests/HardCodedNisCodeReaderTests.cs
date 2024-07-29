namespace NisCodeService.HardCoded.Tests
{
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions;
    using Xunit;

    public class HardCodedNisCodeReaderTests
    {
        [Theory]
        [InlineData("OVO002067", "44021")]
        [InlineData("ovo002007", "13002")]
        public async Task GetCode(string ovoCode, string expectedResult)
        {
            INisCodeService nisCodeService = new HardCodedNisCodeService();
            var result = await nisCodeService.Get(ovoCode, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public async Task GetAllCodes()
        {
            INisCodeService nisCodeService = new HardCodedNisCodeService();
            var result = await nisCodeService.GetAll();

            Assert.NotEmpty(result);
        }
    }
}
