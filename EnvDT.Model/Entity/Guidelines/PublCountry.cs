using System;

namespace EnvDT.Model.Entity
{
    public class PublCountry
    {
        public Guid PublicationId { get; set; }
        public Publication Publication { get; set; }

        public Guid CountryId { get; set; }
        public Country Country { get; set; }
    }
}
