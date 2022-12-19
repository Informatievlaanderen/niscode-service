using System;
using NisCodeService.Abstractions;

namespace NisCodeService.Sync.HardCoded
{
    public class HardCodedNisCodeReaderFactory : INisCodeReaderFactory
    {
        public INisCodeReader CreateReader(IServiceProvider services) => new HardCodedNisCodeReader();
    }
}
