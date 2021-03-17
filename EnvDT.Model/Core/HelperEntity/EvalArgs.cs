using EnvDT.Model.Entity;
using System;

namespace EnvDT.Model.Core.HelperEntity
{
    public class EvalArgs
    {
        public Guid LabReportId { get; set; }
        public Sample Sample { get; set; }
        public Guid PublicationId { get; set; }
        public Guid MediumSubTypeId { get; set; }
        public Guid ConditionId { get; set; }
        public bool SelectSameLrParamMaxValue { get; set; }
        public bool SelectDiffLrParamMaxValue { get; set; }
        public bool EvalFootnotes { get; set; }
    }
}
