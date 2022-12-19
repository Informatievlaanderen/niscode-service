using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using NisCodeService.Abstractions;

namespace NisCodeService
{
    public class NisCodeService : INisCodeService
    {
        private static readonly ConcurrentDictionary<string, string> Cache = new ConcurrentDictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
        private readonly INisCodeReader _reader;

        public NisCodeService(IServiceProvider services, INisCodeReaderFactory readerFactory)
        {
            _reader = readerFactory.CreateReader(services);
        }

        public async Task<string?> Get(string? ovoCode, CancellationToken cancellationToken = default)
        {
            ovoCode = ovoCode.WithoutOvoPrefix();
            if (Cache.ContainsKey(ovoCode ?? "bad ovo code"))
            {
                return Cache[ovoCode ?? "bad ovo code"];
            }

            await _reader.ReadNisCodes(Cache, cancellationToken);
            var result = Cache.ContainsKey(ovoCode ?? "bad ovo code")
                ? Cache[ovoCode]
                : null;
            return result;
        }
    }
}
