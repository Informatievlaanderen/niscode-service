using System.Collections.Generic;

namespace NisCodeService.Sync.OrganizationRegistry.Models
{
    using System;

    public class OrganizationRegistryResponse
    {
        public IEnumerable<Organization>? Organizations { get; set; }
    }

    public class Organization
    {
        public int ChangeId { get; set; }
        public string? ChangeTime { get; set; }
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? OvoNumber { get; set; }
        public Validity? Validity { get; set; }
        public IEnumerable<Key>? Keys { get; set; }
    }

    public class Key
    {
        public string? OrganisationKeyId { get; set; }
        public string? KeyTypeId { get; set; }
        public string? KeyTypeName { get; set; }
        public string? Value { get; set; }
        public Validity? Validity { get; set; }
    }

    public class Validity
    {
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
    }
}
