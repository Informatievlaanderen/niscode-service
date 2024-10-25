namespace NisCodeService.Abstractions
{
    using System;

    public sealed class OrganisationNisCode
    {
        public string NisCode { get; }
        public string OvoCode { get; }
        public DateTime? ValidFrom { get; }
        public DateTime? ValidTo { get; }

        public OrganisationNisCode(string nisCode, string ovoCode, DateTime? validFrom, DateTime? validTo)
        {
            NisCode = nisCode;
            OvoCode = ovoCode;
            ValidFrom = validFrom;
            ValidTo = validTo;
        }

        public bool IsValid(DateTime date)
        {
            if (ValidFrom.HasValue && date < ValidFrom.Value)
                return false;

            if (ValidTo.HasValue && date > ValidTo.Value)
                return false;

            return true;
        }
    }
}
