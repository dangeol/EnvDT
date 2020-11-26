using System;

namespace EnvDT.Model.Core.HelperClasses
{
    public class EvalArgs
    {
        public Guid LabReportId { get; set; }
        public Guid SampleId { get; set; }
        public Guid PublicationId { get; set; }
        public Guid MediumSubTypeId { get; set; }
        public Guid ConditionId { get; set; }
    }
}
