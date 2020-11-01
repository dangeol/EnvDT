using System;
using System.Collections.Generic;

namespace EnvDT.Model.Entity
{
    public class LabReport
    {
        public Guid LabReportId { get; set; }
        public string ReportLabIdent { get; set; }

        public Guid LaboratoryId { get; set; }
        public Laboratory Laboratory { get; set; }
        public Guid ProjectId { get; set; }
        public Project Project { get; set; }
        public List<LabReportParam> LabReportParams { get; set; }
        public List<Sample> Samples { get; set; }
    }
}
