using System;
using System.Collections.Generic;

namespace EnvDT.Model.Entity
{
    public class Country
    {
        public Guid CountryId { get; set; }
        public string CountryNameEn { get; set; }
        public string CountryNameDe { get; set; }

        public List<PublCountry> PublCountries { get; set; }
        public List<Region> Regions { get; set; }
        public List<Laboratory> Laboratories { get; set; }
        public List<Project> Projects { get; set; }
    }
}
