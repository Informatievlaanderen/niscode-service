namespace NisCodeService.HardCoded
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions;

    public class HardCodedNisCodeService : INisCodeService
    {
        private readonly Dictionary<string, string> _validNisCodeByOvoCodes;

        public HardCodedNisCodeService()
        {
            _validNisCodeByOvoCodes = new Dictionary<string, string>(
                HardCodedNisCodes
                    .AllHardCodedNisCodes
                    .Where(x => x.IsValid(DateTime.Now))
                    .ToDictionary(x => x.OvoCode, x=> x.NisCode)
                , StringComparer.InvariantCultureIgnoreCase);
        }

        public Task<Dictionary<string, string>> GetAll(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_validNisCodeByOvoCodes);
        }

        public async Task<string?> Get(string ovoCode, CancellationToken cancellationToken = default)
        {
            var result = (await GetAll(cancellationToken))
                .TryGetValue(ovoCode, out var nisCode);

            return nisCode;
        }
    }
}
