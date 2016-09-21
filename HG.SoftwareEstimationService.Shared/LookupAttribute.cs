using System;
using HG.SoftwareEstimationService.Enums;

namespace HG.SoftwareEstimationService.Shared
{
    public class LookupAttribute : Attribute
    {
        public readonly Lookup Lookup;

        public LookupAttribute(Lookup lookup)
        {
            Lookup = lookup;
        }
    }
}
