namespace NisCodeService.Sync.OrganizationRegistry
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IKeyValuePairStorage
    {
        public Task Persist(IDictionary<string, string> dictionary, CancellationToken ct);
    }
}
