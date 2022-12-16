using System.Threading.Tasks;
using System.Threading;
using NisCodeService.Abstractions;

namespace NisCodeService.Endpoints
{
    public static class Handlers
    {
        public static async Task<string?> Get(string ovoCode, INisCodeService nisCodeService, CancellationToken cancellationToken = default)
            => await nisCodeService.Get(ovoCode, cancellationToken);
    }
}
