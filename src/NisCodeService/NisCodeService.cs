using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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

        public NisCodeService(INisCodeReaderFactory readerFactory, ILoggerFactory loggerFactory)
        {
            _reader = readerFactory.CreateReader();
            _loggerFactory = loggerFactory;
        }

        public async Task<Dictionary<string, string>> GetAll(CancellationToken cancellationToken = default) => await _reader.ReadNisCodes(cancellationToken);

        public async Task<string?> Get(string? ovoCode, CancellationToken cancellationToken = default)
        {
            ovoCode = ovoCode.WithoutOvoPrefix();
            if (Cache.ContainsKey(ovoCode ?? "bad ovo code"))
            {
                return Cache[ovoCode ?? "bad ovo code"];
            }

            await _reader.ReadNisCodes(cancellationToken);
            var result = Cache.ContainsKey(ovoCode ?? "bad ovo code")
                ? Cache[ovoCode ?? "bad ovo code"]
                : null;
            return result;
        }
    }
}
