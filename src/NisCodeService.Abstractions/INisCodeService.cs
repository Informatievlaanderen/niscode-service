using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NisCodeService.Abstractions
{
    public interface INisCodeService
    {
        Task<Dictionary<string, string>> GetAll(CancellationToken cancellationToken = default);
        Task<string?> Get(string ovoCode, CancellationToken cancellationToken = default);
    }
}
