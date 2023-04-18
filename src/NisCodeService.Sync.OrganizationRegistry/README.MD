# Querying a local instance of DynamoDB

### Examples

`aws dynamodb list-tables`

#### Find NisCode by OvoCode
`aws dynamodb scan --table-name basisregisters-niscode-sync --filter-expression "ovocode = :val" --expression-attribute-values '{":val":{"S":"002071"}}'`

#### Find OvoCode by NisCode
`aws dynamodb scan --table-name basisregisters-niscode-sync --filter-expression "niscode = :val" --expression-attribute-values '{":val":{"S":"11002"}}'`


### Resources
https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/DynamoDBLocal.DownloadingAndRunning.html
