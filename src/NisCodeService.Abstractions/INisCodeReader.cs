using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NisCodeService.Abstractions
{
    public interface INisCodeReader
    {
        Task<List<OrganisationNisCode>> ReadNisCodes(CancellationToken cancellationToken = default);
    }
}
