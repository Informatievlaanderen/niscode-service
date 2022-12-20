using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NisCodeService.Abstractions;

namespace NisCodeService
{
    public class NisCodeService : INisCodeService
    {
        private static readonly ConcurrentDictionary<string, string> Cache = new ConcurrentDictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

        private readonly INisCodeReader _reader;
        private readonly ILoggerFactory _loggerFactory;

        public NisCodeService(IServiceProvider services, INisCodeReaderFactory readerFactory)
        {
            _reader = readerFactory.CreateReader(services);
            _loggerFactory = services.GetRequiredService<ILoggerFactory>();
        }

        public async Task<string?> Get(string? ovoCode, CancellationToken cancellationToken = default)
        {
            ovoCode = ovoCode.WithoutOvoPrefix();
            if (Cache.ContainsKey(ovoCode ?? "bad ovo code"))
            {
                return Cache[ovoCode ?? "bad ovo code"];
            }

            await _reader.ReadNisCodes(Cache, _loggerFactory, cancellationToken);
            var result = Cache.ContainsKey(ovoCode ?? "bad ovo code")
                ? Cache[ovoCode ?? "bad ovo code"]
                : null;
            return result;
        }
    }
}
