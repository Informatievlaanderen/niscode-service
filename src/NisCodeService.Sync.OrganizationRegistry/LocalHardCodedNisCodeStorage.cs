namespace NisCodeService.Sync.OrganizationRegistry
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public sealed class LocalHardCodedNisCodeStorage : IKeyValuePairStorage
    {
        public Task Persist(IDictionary<string, string> dictionary, CancellationToken ct)
        {
            foreach (var nisOvoCode in dictionary)
            {
                //["OVO003105"] = "11202",
                Console.WriteLine($"[\"{nisOvoCode.Key}\"] = \"{nisOvoCode.Value}\",");
            }

            return Task.CompletedTask;
        }
    }
}
