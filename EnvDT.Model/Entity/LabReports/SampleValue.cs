using System;

namespace EnvDT.Model.Entity
{
    public class SampleValue
    {
        public Guid SampleValueId { get; set; }
        public double SValue { get; set; }

        public Guid LabReportParamId { get; set; }
        public LabReportParam LabReportParam { get; set; }
        public Guid SampleId { get; set; }
        public Sample Sample { get; set; }
    }
}
