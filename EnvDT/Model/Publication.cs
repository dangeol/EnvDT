using System;
using System.Collections.Generic;
using System.Text;

namespace EnvDT.Model
{
    public class Publication
    {
        public int PublicationId { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Publisher { get; set; }
        public string Year { get; set; }
        public string URL { get; set; }
        public string Addendum { get; set; }
        public string Note { get; set; }

        public List<PublCountry> PublCountries { get; set; }
        public List<PublRegion> PublRegions { get; set; }
        public List<RefValue> RefValues { get; set; }
    }
}
