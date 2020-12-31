using System;
using System.Collections.Generic;

namespace EnvDT.Model.Entity
{
    public class Laboratory
    {
        public Guid LaboratoryId { get; set; }
        public string LabCompany { get; set; }
        public string LabName { get; set; }

        public Guid CountryId { get; set; }
        public Country Country { get; set; }
        public ConfigXlsx ConfigXlsx { get; set; }
        public List<LabReport> LabReports { get; set; }
        
    }
}
