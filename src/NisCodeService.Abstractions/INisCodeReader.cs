using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace NisCodeService.Abstractions;

public interface INisCodeReader
{
    Task ReadNisCodes(IDictionary<string, string> cache, ILoggerFactory loggerFactory, CancellationToken cancellationToken = default);
}