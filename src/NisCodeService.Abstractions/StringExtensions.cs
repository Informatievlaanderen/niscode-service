using System;

namespace NisCodeService.Abstractions
{
    public static class StringExtensions
    {
        public static string? WithoutOvoPrefix(this string? s) => s?.Replace("OVO", "", StringComparison.InvariantCultureIgnoreCase);
    }
}
