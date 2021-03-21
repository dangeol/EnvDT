using System;
using System.Collections.Generic;

namespace EnvDT.Model.Entity
{
    public class Publication
    {
        public Guid PublicationId { get; set; }
        public string Title { get; set; }
        public string Abbreviation { get; set; }
        public string PublIdent { get; set; }
        public int OrderId { get; set; }
        public string Author { get; set; }
        public string Publisher { get; set; }
        public string Year { get; set; }
        public string URL { get; set; }
        public string Addendum { get; set; }
        public string Note { get; set; }
        public bool UsesMediumSubTypes { get; set; }
        public bool UsesConditions { get; set; }

        public List<PublCountry> PublCountries { get; set; }
        public List<PublRegion> PublRegions { get; set; }
        public List<PublParam> PublParams { get; set; }
        public List<ValuationClass> ValuationClasses { get; set; }
    }
}
