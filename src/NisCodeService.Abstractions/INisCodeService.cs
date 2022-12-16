using System.Threading;
using System.Threading.Tasks;

namespace NisCodeService.Abstractions
{
    public interface INisCodeService
    {
        Task<string?> Get(string ovoCode, CancellationToken cancellationToken = default);
    }
}
