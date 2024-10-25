namespace NisCodeService.Sync.OrganizationRegistry
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions;

    public sealed class LocalHardCodedNisCodeStorage : IKeyValuePairStorage
    {
        public Task Persist(List<OrganisationNisCode> nisCodes, CancellationToken ct)
        {
            foreach (var nisOvoCode in nisCodes)
            {
                //new OrganisationNisCode("11001", "11001", new DateTime(2019, 1, 1), null),
                var stringBuilder = new StringBuilder();
                stringBuilder.Append("new OrganisationNisCode(");
                stringBuilder.Append($"\"{nisOvoCode.NisCode}\", ");
                stringBuilder.Append($"\"{nisOvoCode.OvoCode}\", ");
                if(nisOvoCode.ValidFrom.HasValue)
                    stringBuilder.Append($"new DateTime({nisOvoCode.ValidFrom.Value.Year}, {nisOvoCode.ValidFrom.Value.Month}, {nisOvoCode.ValidFrom.Value.Day}), ");
                else
                    stringBuilder.Append("null, ");

                if(nisOvoCode.ValidTo.HasValue)
                    stringBuilder.Append($"new DateTime({nisOvoCode.ValidTo.Value.Year}, {nisOvoCode.ValidTo.Value.Month}, {nisOvoCode.ValidTo.Value.Day})");
                else
                    stringBuilder.Append("null");
                stringBuilder.Append("),");
                Console.WriteLine(stringBuilder.ToString());
            }

            return Task.CompletedTask;
        }
    }
}
