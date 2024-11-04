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
        private readonly Dictionary<string, List<OrganisationNisCode>> _allNisCodeByOvoCodes;

        public HardCodedNisCodeService()
        {
            _allNisCodeByOvoCodes = new Dictionary<string, List<OrganisationNisCode>>(
                HardCodedNisCodes.AllHardCodedNisCodes
                    .GroupBy(x => x.OvoCode)
                    .ToDictionary(x => x.Key, x=> x.ToList())
                , StringComparer.InvariantCultureIgnoreCase);
        }

        public Task<Dictionary<string, string>> GetAll(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(
                new Dictionary<string, string>(
                    HardCodedNisCodes
                        .AllHardCodedNisCodes
                        .Where(x => x.IsValid(DateTime.Now))
                        .ToDictionary(x => x.OvoCode, x=> x.NisCode)
                    , StringComparer.InvariantCultureIgnoreCase)
                );
        }

        public Task<string?> Get(string ovoCode, CancellationToken cancellationToken = default)
        {
            _allNisCodeByOvoCodes.TryGetValue(ovoCode, out var nisCodes);

            return Task.FromResult(nisCodes?.SingleOrDefault(x => x.IsValid(DateTime.Now))?.NisCode);
        }

        public Task<string?> Get(string ovoCode, DateTime validFrom, CancellationToken cancellationToken = default)
        {
            _allNisCodeByOvoCodes.TryGetValue(ovoCode, out var nisCodes);

            return Task.FromResult(nisCodes?.SingleOrDefault(x => x.IsValid(validFrom))?.NisCode);
        }
    }
}
