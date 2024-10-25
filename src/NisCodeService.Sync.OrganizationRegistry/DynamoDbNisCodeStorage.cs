namespace NisCodeService.Sync.OrganizationRegistry
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions;
    using Amazon.DynamoDBv2;
    using Amazon.DynamoDBv2.DocumentModel;
    using Amazon.DynamoDBv2.Model;
    using DynamoDb;

    public sealed class DynamoDbNisCodeStorage : IKeyValuePairStorage
    {
        private readonly IAmazonDynamoDB _dynamoDb;

        public DynamoDbNisCodeStorage(IAmazonDynamoDB dynamoDb)
        {
            _dynamoDb = dynamoDb;
        }

        public async Task Persist(List<OrganisationNisCode> nisCodes, CancellationToken ct)
        {
            var table = await GetTableAsync();

            var dictionary = new Dictionary<string, string>(
                nisCodes
                    .Where(x => x.IsValid(DateTime.Now))
                    .ToDictionary(x => x.OvoCode, x => x.NisCode),
                StringComparer.InvariantCultureIgnoreCase);

            await DeleteItemsNotInDictionary(dictionary, table, ct);

            await UpsertItemsInDictionary(dictionary, table, ct);
        }

        private static async Task DeleteItemsNotInDictionary(IDictionary<string, string> dictionary, Table table, CancellationToken ct)
        {
            var allItems = await table.Scan(new ScanFilter()).GetRemainingAsync(ct);

            var itemsToDelete = allItems.Select(x => x[ColumnNames.OvoCode].AsString())
                .Except(dictionary.Keys, StringComparer.InvariantCultureIgnoreCase)
                .ToList();

            foreach (var batchedOvoCodesToDelete in itemsToDelete.Chunk(25))
            {
                var writer = table.CreateBatchWrite();

                foreach (var itemOvoCode in batchedOvoCodesToDelete)
                {
                    var documentToDelete = allItems.Single(x => x[ColumnNames.OvoCode] == itemOvoCode);
                    writer.AddItemToDelete(documentToDelete);
                }

                await writer.ExecuteAsync(ct);
            }
        }

        private static async Task UpsertItemsInDictionary(IDictionary<string, string> dictionary, Table table, CancellationToken ct)
        {
            var now = DateTime.UtcNow;

            foreach (var batchedItemsToAdd in dictionary.Chunk(25))
            {
                var writer = table.CreateBatchWrite();

                foreach (var itemToAdd in batchedItemsToAdd)
                {
                    var doc = new Document
                    {
                        [ColumnNames.OvoCode] = itemToAdd.Key,
                        [ColumnNames.NisCode] = itemToAdd.Value,
                        [ColumnNames.Timestamp] = now
                    };

                    writer.AddDocumentToPut(doc);
                }

                await writer.ExecuteAsync(ct);
            }
        }

        private async Task<Table> GetTableAsync()
        {
            await CreateTableAsync();

            return Table.LoadTable(_dynamoDb, TableNames.OvoNisCodes);
        }

        private async Task CreateTableAsync()
        {
            try
            {
                if (await DoesTableExists())
                {
                    return;
                }

                await _dynamoDb.CreateTableAsync(new CreateTableRequest
                {
                    TableName = TableNames.OvoNisCodes,
                    AttributeDefinitions = new List<AttributeDefinition>
                    {
                        new AttributeDefinition
                        {
                            AttributeType = ScalarAttributeType.S,
                            AttributeName = ColumnNames.OvoCode
                        }
                    },
                    KeySchema = new List<KeySchemaElement>
                    {
                        new KeySchemaElement
                        {
                            KeyType = KeyType.HASH,
                            AttributeName = ColumnNames.OvoCode
                        }
                    },
                    BillingMode = BillingMode.PAY_PER_REQUEST
                });

                // need to wait a bit since the table has just been created
                await Task.Delay(TimeSpan.FromSeconds(10));
            }
            catch (ResourceInUseException)
            {
                // ignore, already exists
            }
        }

        public async Task<bool> DoesTableExists()
        {
            // https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/LowLevelDotNetWorkingWithTables.html#LowLeveldotNetListTables
            // Initial value for the first page of table names.
            string? lastEvaluatedTableName = null;
            do
            {
                // Create a request object to specify optional parameters.
                var request = new ListTablesRequest { ExclusiveStartTableName = lastEvaluatedTableName };

                var result = await _dynamoDb.ListTablesAsync(request);
                if (result.TableNames.Any(t => t == TableNames.OvoNisCodes))
                {
                    return true;
                }

                lastEvaluatedTableName = result.LastEvaluatedTableName;
            } while (lastEvaluatedTableName != null);

            return false;
        }
    }
}
