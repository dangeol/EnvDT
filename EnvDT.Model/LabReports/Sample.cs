using System;
using System.Collections.Generic;

namespace EnvDT.Model
{
    public class Sample
    {
        public Guid SampleId { get; set; }
        public string SampleLabIdent { get; set; }
        public DateTime SampleDate { get; set; }
        public string SampleName { get; set; }

        public Guid LabReportId { get; set; }
        public LabReport LabReport { get; set; }
        public List<SampleValue> SampleValues { get; set; }
    }
}
