namespace NisCodeService.DynamoDb.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Xunit;

    public class DynamoDbNisCodeServiceTests : IClassFixture<IntegrationTestFixture>
    {
        private readonly IntegrationTestFixture _fixture;

        public DynamoDbNisCodeServiceTests(IntegrationTestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task GetAll_ReturnsSameValues()
        {
            // Arrange
            var sut = new DynamoDbNisCodeService(_fixture.DynamoDb);

            // Act
            var entries = await sut.GetAll();

            // Assert
            entries.Should().BeEquivalentTo(_fixture.OvoNisCodesDictionary);
        }

        [Fact]
        public async Task Get_ReturnsSameValue()
        {
            // Arrange
            var r = new Random();
            var ovoCode = _fixture.OvoNisCodesDictionary.Keys.ElementAt(r.Next(0, 999));
            var sut = new DynamoDbNisCodeService(_fixture.DynamoDb);

            // Act
            var entry = await sut.Get(ovoCode);

            // Assert
            entry.Should().Be(_fixture.OvoNisCodesDictionary[ovoCode]);
        }

        [Fact]
        public async Task GetCaseInsensitive_ReturnsSameValue()
        {
            // Arrange
            var r = new Random();
            var ovoCode = _fixture.OvoNisCodesDictionary.Keys.ElementAt(r.Next(0, 999));
            var sut = new DynamoDbNisCodeService(_fixture.DynamoDb);

            // Act
            var lowerCaseEntry = await sut.Get(ovoCode.ToLowerInvariant());
            var upperCaseEntry = await sut.Get(ovoCode.ToUpperInvariant());

            // Assert
            lowerCaseEntry.Should().Be(_fixture.OvoNisCodesDictionary[ovoCode]);
            upperCaseEntry.Should().Be(_fixture.OvoNisCodesDictionary[ovoCode]);
        }
    }
}
