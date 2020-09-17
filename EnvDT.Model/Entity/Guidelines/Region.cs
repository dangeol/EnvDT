using System;
using System.Collections.Generic;

namespace EnvDT.Model.Entity
{
    public class Region
    {
        public Guid RegionId { get; set; }
        public string RegionNameEn { get; set; }
        public string RegionNameDe { get; set; }

        public Guid CountryId { get; set; }
        public Country Country { get; set; }
        public List<PublRegion> PublRegions { get; set; }
    }
}
