﻿namespace NisCodeService.DynamoDb
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions;
    using Amazon.DynamoDBv2;
    using Amazon.DynamoDBv2.Model;

    public class DynamoDbNisCodeService : INisCodeService
    {
        private readonly IAmazonDynamoDB _amazonDynamoDb;

        public DynamoDbNisCodeService(IAmazonDynamoDB amazonDynamoDb)
        {
            _amazonDynamoDb = amazonDynamoDb;
        }

        public async Task<Dictionary<string, string>> GetAll(CancellationToken cancellationToken = default)
        {
            //Get all items from table
            var query = new ScanRequest
            {
                TableName = TableNames.OvoNisCodes
            };
            var response = await _amazonDynamoDb.ScanAsync(query, cancellationToken);

            return response.Items
                .ToDictionary(
                    item => item[ColumnNames.OvoCode].S,
                    item => item[ColumnNames.NisCode].S);
        }

        public async Task<string?> Get(string ovoCode, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(ovoCode))
            {
                return null;
            }

            //Get item column niscode by ovoCode
            var query = new GetItemRequest
            {
                TableName = TableNames.OvoNisCodes,
                Key = new Dictionary<string, AttributeValue>
                {
                    { ColumnNames.OvoCode, new AttributeValue { S = ovoCode.ToUpper() } }
                },
                ProjectionExpression = ColumnNames.NisCode,
                ConsistentRead = false
            };

            var response = await _amazonDynamoDb.GetItemAsync(query, cancellationToken);
            return response.Item?.GetValueOrDefault(ColumnNames.NisCode)?.S;
        }

        public Task<string?> Get(string ovoCode, DateTime validFrom, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
