using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NisCodeService.Abstractions;

namespace NisCodeService.Endpoints
{
    public static class Handlers
    {
        public static async Task<IDictionary<string, string>> GetAll(INisCodeService nisCodeService, CancellationToken cancellationToken = default)
            => await nisCodeService.GetAll(cancellationToken);

        public static async Task<string?> Get(string ovoCode, INisCodeService nisCodeService, CancellationToken cancellationToken = default)
            => await nisCodeService.Get(ovoCode.WithoutOvoPrefix() ?? "bad ovo code", cancellationToken);
    }
}
