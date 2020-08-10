using System;
using System.Collections.Generic;
using System.Text;

namespace EnvDT.Model
{
    public class PublCountry
    {
        public int PublicationId { get; set; }
        public Publication Publication { get; set; }

        public int CountryId { get; set; }
        public Country Country { get; set; }
    }
}
