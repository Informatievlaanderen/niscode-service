using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NisCodeService.Abstractions
{
    using System;

    public interface INisCodeService
    {
        Task<Dictionary<string, string>> GetAll(CancellationToken cancellationToken = default);
        Task<string?> Get(string ovoCode, CancellationToken cancellationToken = default);
        Task<string?> Get(string ovoCode, DateTime validFrom, CancellationToken cancellationToken = default);
    }
}
