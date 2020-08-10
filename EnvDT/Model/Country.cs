using System;
using System.Collections.Generic;
using System.Text;

namespace EnvDT.Model
{
    public class Country
    {
        public int CountryId { get; set; }
        public string CountryNameEn { get; set; }
        public string CountryNameDe { get; set; }

        public List<PublCountry> PublCountries { get; set; }
        public List<Region> Regions { get; set; }
    }
}
