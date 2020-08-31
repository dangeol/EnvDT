using System;
using System.Collections.Generic;

namespace EnvDT.Model
{
    public class Laboratory
    {
        public Guid LaboratoryId { get; set; }
        public string LaboratoryName { get; set; }

        public Guid CountryId { get; set; }
        public Country Country { get; set; }
        public List<ParameterLaboratory> ParameterLaboratories { get; set; }
        public List<LabReport> LabReports { get; set; }
    }
}
