namespace NisCodeService.Sync.OrganizationRegistry
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions;

    public interface IKeyValuePairStorage
    {
        public Task Persist(List<OrganisationNisCode> nisCodes, CancellationToken ct);
    }
}
