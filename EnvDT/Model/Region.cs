using System;
using System.Collections.Generic;
using System.Text;

namespace EnvDT.Model
{
    public class Region
    {
        public int RegionId { get; set; }
        public string RegionNameEn { get; set; }
        public string RegionNameDe { get; set; }

        public int CountryId { get; set; }
        public Country Country { get; set; }
        public List<PublRegion> PublRegions { get; set; }
    }
}
