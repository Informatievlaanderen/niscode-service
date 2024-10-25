namespace NisCodeService.Sync.OrganizationRegistry.Infrastructure
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Amazon;
    using Amazon.DynamoDBv2;
    using Amazon.Internal;
    using Amazon.Runtime;
    using Be.Vlaanderen.Basisregisters.Aws.DistributedMutex;
    using Extensions;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Serilog;
    using Serilog.Debugging;
    using Serilog.Extensions.Logging;

    public static class HostBuilderExtensions
    {
        public static IHostBuilder ConfigureHostBuilder(this IHostBuilder builder)
        {
            return builder.ConfigureAppConfiguration((hostContext, config) =>
                {
                    config
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                        .AddJsonFile($"appsettings.{Environment.MachineName.ToLowerInvariant()}.json", optional: true,
                            reloadOnChange: false)
                        .AddEnvironmentVariables();
                })
                .ConfigureLogging((hostContext, loggingBuilder) =>
                {
                    SelfLog.Enable(Console.WriteLine);

                    Log.Logger = new LoggerConfiguration()
                        .ReadFrom.Configuration(hostContext.Configuration)
                        .Enrich.FromLogContext()
                        .Enrich.WithMachineName()
                        .Enrich.WithThreadId()
                        .Enrich.WithEnvironmentUserName()
                        .CreateLogger();

                    loggingBuilder.ClearProviders();
                    loggingBuilder.AddSerilog(Log.Logger);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    var loggerFactory = new SerilogLoggerFactory(Log.Logger);

                    services.AddOrganizationRegistryNisCodeReader();
                    services.AddHostedService<OrganizationRegistrySync>();

                    services.Configure<ServiceOptions>(hostContext.Configuration);

                    // if (hostContext.Configuration["ASPNETCORE_ENVIRONMENT"] == "Development")
                    // {
                    //     services.AddSingleton<IAmazonDynamoDB>(sp =>
                    //     {
                    //         var clientConfig = new AmazonDynamoDBConfig { ServiceURL = "http://localhost:8000" };
                    //         return new AmazonDynamoDBClient(clientConfig);
                    //     });
                    // }
                    // else
                    // {
                        services.AddSingleton<IAmazonDynamoDB, AmazonDynamoDBClient>();
                    // }
                    services.AddSingleton<IKeyValuePairStorage, LocalHardCodedNisCodeStorage>();
                });
        }
    }

    public sealed class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                AppDomain.CurrentDomain.FirstChanceException += (sender, eventArgs) =>
                    Log.Debug(
                        eventArgs.Exception,
                        "FirstChanceException event raised in {AppDomain}.",
                        AppDomain.CurrentDomain.FriendlyName);

                AppDomain.CurrentDomain.UnhandledException += (sender, eventArgs) =>
                    Log.Fatal((Exception) eventArgs.ExceptionObject, "Encountered a fatal exception, exiting program.");

                Log.Information("Starting Organization Sync Process");

                var host = new HostBuilder()
                    .ConfigureHostBuilder()
                    .UseConsoleLifetime()
                    .Build();

                var logger = host.Services.GetRequiredService<ILogger<Program>>();
                var loggerFactory = host.Services.GetRequiredService<ILoggerFactory>();
                var configuration = host.Services.GetRequiredService<IConfiguration>();


                await DistributedLock<Program>.RunAsync(
                        async () =>
                        {
                            await host.RunAsync().ConfigureAwait(false);
                        },
                        DistributedLockOptions.LoadFromConfiguration(configuration),
                        logger)
                    .ConfigureAwait(false);
            }
            catch (Exception e)
            {
                // logger.LogCritical(e, "Encountered a fatal exception, exiting program.");
                Log.CloseAndFlush();

                // Allow some time for flushing before shutdown.
                await Task.Delay(500, default);
                throw;
            }
            finally
            {
                // logger.LogInformation("Stopping...");
            }
        }
    }
}
