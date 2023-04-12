namespace NisCodeService.Sync.OrganizationRegistry.Tests
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Amazon;
    using Amazon.DynamoDBv2;
    using Amazon.DynamoDBv2.DocumentModel;
    using Amazon.DynamoDBv2.Model;
    using Amazon.Runtime;
    using Be.Vlaanderen.Basisregisters.DockerUtilities;
    using FluentAssertions;
    using Infrastructure;
    using Microsoft.Extensions.Options;
    using Xunit;

    public class DynamoDbNisCodeStorageTests
    {
        [Fact]
        public async Task DoesPersistKvps()
        {
            using var _ = DockerComposer.Compose("dynamodb.yml", "dynamodb");

            string tableName = "integrationtest";
            var dynamoDb = new AmazonDynamoDBClient(new BasicAWSCredentials("key", "secret"), RegionEndpoint.GetBySystemName("local"));

            // Wait for docker container
            await WaitForDynamoDbToBecomeAvailable(dynamoDb, tableName);

            var sut = new DynamoDbNisCodeStorage(dynamoDb, Options.Create(new ServiceOptions{TableName = tableName}));

            var dictToTest = Enumerable.Range(0, 1000).ToDictionary(i => i.ToString(), i => "noescode");

            await sut.Persist(dictToTest, CancellationToken.None);

            // Assert
            var table = Table.LoadTable(dynamoDb, tableName);
            var actualEntries = await table.Scan(new ScanFilter()).GetRemainingAsync(CancellationToken.None);
            actualEntries.Should().HaveCount(dictToTest.Count);
        }

        private static async Task WaitForDynamoDbToBecomeAvailable(AmazonDynamoDBClient dynamoDb, string tableName)
        {
            foreach (var _ in Enumerable.Range(0, 60))
            {
                try
                {
                    await dynamoDb.DescribeTableAsync(new DescribeTableRequest(tableName));
                    break;
                }
                catch (InternalServerErrorException e)
                {
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
                catch (ResourceNotFoundException e)
                {
                    break;
                }
            }
        }
    }
}
