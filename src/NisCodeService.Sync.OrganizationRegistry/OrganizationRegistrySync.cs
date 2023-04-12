namespace NisCodeService.Sync.OrganizationRegistry
{
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    public class OrganizationRegistrySync : BackgroundService
    {
        private readonly INisCodeReaderFactory _nisCodeReaderFactory;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IKeyValuePairStorage _kvpStorage;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly ILogger<OrganizationRegistrySync> _logger;

        public OrganizationRegistrySync(
            INisCodeReaderFactory nisCodeReaderFactory,
            ILoggerFactory loggerFactory,
            IKeyValuePairStorage kvpStorage,
            IHostApplicationLifetime hostApplicationLifetime)
        {
            _nisCodeReaderFactory = nisCodeReaderFactory;
            _loggerFactory = loggerFactory;
            _kvpStorage = kvpStorage;
            _hostApplicationLifetime = hostApplicationLifetime;
            _logger = loggerFactory.CreateLogger<OrganizationRegistrySync>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var nisCodeReader = _nisCodeReaderFactory.CreateReader();

            _logger.LogInformation("Sync Started");
            var result = await nisCodeReader.ReadNisCodes(stoppingToken);

            await _kvpStorage.Persist(result, stoppingToken);

            _logger.LogInformation("Sync Ended");

            _hostApplicationLifetime.StopApplication();
        }
    }
}
