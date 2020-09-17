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
        public string ProjectAddress { get; set; }

        public List<LabReport> LabReports { get; set; }
    }
}
