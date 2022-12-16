using System.Threading;
using System.Threading.Tasks;
using NisCodeService.Abstractions;

namespace NisCodeService
{
    public class NisCodeService : INisCodeService
    {
        public async Task<string?> Get(string ovoCode, CancellationToken cancellationToken = default)
        {
            await Task.Yield();
            return ovoCode.Length.ToString();
        }
    }
}
