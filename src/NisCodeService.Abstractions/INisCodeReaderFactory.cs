using System;

namespace NisCodeService.Abstractions
{
    public interface INisCodeReaderFactory
    {
        INisCodeReader CreateReader(IServiceProvider services);
    }
}
