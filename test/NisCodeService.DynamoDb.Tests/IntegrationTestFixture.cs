namespace NisCodeService.DynamoDb.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Amazon;
    using Amazon.DynamoDBv2;
    using Amazon.DynamoDBv2.Model;
    using Amazon.Runtime;
    using Be.Vlaanderen.Basisregisters.DockerUtilities;
    using Ductus.FluentDocker.Services;
    using Xunit;

    public class IntegrationTestFixture : IAsyncLifetime
    {
        private ICompositeService? _dynamoDocker;

        public IAmazonDynamoDB? DynamoDb { get; private set; }
        public Dictionary<string, string>? OvoNisCodesDictionary { get; private set; }

        public async Task InitializeAsync()
        {
            _dynamoDocker = DockerComposer.Compose("dynamodb.yml", "dynamodb-integration-test");
            DynamoDb = new AmazonDynamoDBClient(new BasicAWSCredentials("key", "secret"), new AmazonDynamoDBConfig
            {
                RegionEndpoint = RegionEndpoint.GetBySystemName("local"),
                ServiceURL = "http://localhost:8002",
            });

            OvoNisCodesDictionary = Enumerable.Range(0, 1000).ToDictionary(i => $"OVO{i:000000}", i => "niscode");

            await WaitForDynamoDbToBecomeAvailable(TableNames.OvoNisCodes);
            await SeedDynamo();
        }

        public Task DisposeAsync()
        {
            DynamoDb?.Dispose();
            _dynamoDocker?.Dispose();
            return Task.CompletedTask;
        }

        private async Task WaitForDynamoDbToBecomeAvailable(string tableName)
        {
            foreach (var _ in Enumerable.Range(0, 60))
            {
                try
                {
                    await DynamoDb!.DescribeTableAsync(new DescribeTableRequest(tableName));
                    break;
                }
                catch (InternalServerErrorException)
                {
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
                catch (ResourceNotFoundException)
                {
                    break;
                }
            }
        }

        private async Task SeedDynamo()
        {
            await DynamoDb!.CreateTableAsync(new CreateTableRequest
            {
                TableName = TableNames.OvoNisCodes,
                AttributeDefinitions = new List<AttributeDefinition>
                {
                    new AttributeDefinition(ColumnNames.OvoCode, ScalarAttributeType.S)
                },
                KeySchema = new List<KeySchemaElement>
                {
                    new KeySchemaElement(ColumnNames.OvoCode, KeyType.HASH)
                },
                BillingMode = BillingMode.PAY_PER_REQUEST
            });

            var batches = OvoNisCodesDictionary!
                .Chunk(25)
                .ToList();

            foreach (var batch in batches)
            {
                var request = new BatchWriteItemRequest
                {
                    RequestItems = new Dictionary<string, List<WriteRequest>>
                    {
                        {
                            TableNames.OvoNisCodes,
                            batch.Select(item =>
                                new WriteRequest(new PutRequest(
                                    new Dictionary<string, AttributeValue>
                                    {
                                        { ColumnNames.OvoCode, new AttributeValue(item.Key) },
                                        { ColumnNames.NisCode, new AttributeValue(item.Value) },
                                        { ColumnNames.Timestamp, new AttributeValue(DateTime.UtcNow.ToString("O")) }
                                    }
                                ))
                            ).ToList()
                        }
                    }
                };

                DynamoDb.BatchWriteItemAsync(request).Wait();
            }
        }
    }
}
