namespace NisCodeService.HardCoded
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions;

    public class HardCodedNisCodeService : INisCodeService
    {
        public Task<Dictionary<string, string>> GetAll(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(HardCodedNisCodes.NisCodesByOvoCode);
        }

        public Task<string?> Get(string ovoCode, CancellationToken cancellationToken = default)
        {
            HardCodedNisCodes.NisCodesByOvoCode.TryGetValue(ovoCode, out var nisCode);

            return Task.FromResult(nisCode);
        }
    }
}
