using System;
using System.Collections.Generic;

namespace EnvDT.Model.Entity
{
    public class LabReportParam
    {
        public Guid LabReportParamId { get; set; }
        public double DetectionLimit { get; set; }
        public string Method { get; set; }

        public Guid LabReportId { get; set; }
        public LabReport LabReport { get; set; }
        public Guid ParameterId { get; set; }
        public Parameter Parameter { get; set; }
        public Guid UnitId { get; set; }
        public Unit Unit { get; set; }
        public List<SampleValue> SampleValues { get; set; }
    }
}
