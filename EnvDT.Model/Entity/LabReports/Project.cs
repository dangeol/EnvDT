using System;
using System.Collections.Generic;

namespace EnvDT.Model.Entity
{
    public class Project
    {
        public Guid ProjectId { get; set; }
        public string ProjectNumber { get; set; }
        public string ProjectClient { get; set; }
        public string ProjectName { get; set; }
        public Guid CountryId { get; set; }
        public Country Country { get; set; }
        public Guid RegionId { get; set; }
        public Region Region { get; set; }
        public string ProjectAddress { get; set; }

        public List<LabReport> LabReports { get; set; }
    }
}
