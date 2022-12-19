using System.Collections.Generic;
using System.Threading.Tasks;
using NisCodeService.Abstractions;
using Xunit;

namespace NisCodeService.Sync.HardCoded.Tests
{
    public class HardCodedNisCodeReaderTests
    {
        [Theory]
        [InlineData("OVO002067", "44021")]
        [InlineData("002007", "13002")]
        public async Task ReadNisCodes(string? ovoCode, string expectedResult)
        {
            ovoCode = ovoCode.WithoutOvoPrefix();

            INisCodeReader reader = new HardCodedNisCodeReader();
            var dictionary = new Dictionary<string, string>();
            await reader.ReadNisCodes(dictionary);

            Assert.True(dictionary.ContainsKey(ovoCode ?? "bad ovo code"));
            Assert.Equal(expectedResult, dictionary[ovoCode ?? "bad ovo code"]);
        }
    }
}