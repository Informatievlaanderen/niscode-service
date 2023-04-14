namespace NisCodeService.Sync.OrganizationRegistry
{
    public class DynamoDbSettings
    {
        public string TableName { get; set; } = "OVONIS";

        public bool CreateTableIfNotExists { get; set; } = false;
    }
}
