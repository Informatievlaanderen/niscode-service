namespace NisCodeService.DynamoDb.Extensions
{
    using Abstractions;
    using Amazon.DynamoDBv2;
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDynamoDbNisCodeService(this IServiceCollection services)
        {
            services.AddSingleton<IAmazonDynamoDB, AmazonDynamoDBClient>();
            services.AddSingleton<INisCodeService, DynamoDbNisCodeService>();
            return services;
        }
    }
}
