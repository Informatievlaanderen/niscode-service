namespace NisCodeService.Sync.OrganizationRegistry.Tests
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions;
    using Amazon;
    using Amazon.DynamoDBv2;
    using Amazon.DynamoDBv2.DocumentModel;
    using Amazon.DynamoDBv2.Model;
    using Amazon.Runtime;
    using Be.Vlaanderen.Basisregisters.DockerUtilities;
    using DynamoDb;
    using FluentAssertions;
    using Infrastructure;
    using Microsoft.Extensions.Options;
    using Xunit;

    public class DynamoDbNisCodeStorageTests
    {
        [Fact]
        public async Task DoesPersistKvps()
        {
            using var _ = DockerComposer.Compose("dynamodb.yml", "niscode-sync-integration-dynamo-test");

            const string tableName = TableNames.OvoNisCodes;
            var dynamoDb = new AmazonDynamoDBClient(new BasicAWSCredentials("key", "secret"), new AmazonDynamoDBConfig
            {
                RegionEndpoint = RegionEndpoint.GetBySystemName("local"),
                ServiceURL = "http://localhost:8001",
            });

            // Wait for docker container
            await WaitForDynamoDbToBecomeAvailable(dynamoDb, tableName);

            var sut = new DynamoDbNisCodeStorage(dynamoDb);

            var dictToTest = Enumerable.Range(0, 1000)
                .Select(i => new OrganisationNisCode("niscode", i.ToString(), null, null))
                .ToList();

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
