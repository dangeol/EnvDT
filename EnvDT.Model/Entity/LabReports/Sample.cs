using System;
using System.Collections.Generic;

namespace EnvDT.Model.Entity
{
    public class Sample
    {
        public Guid SampleId { get; set; }
        public string SampleLabIdent { get; set; }
        public DateTime SampleDate { get; set; }
        public string SampleName { get; set; }

        public Guid LabReportId { get; set; }
        public LabReport LabReport { get; set; }
        public Guid? MediumId { get; set; }
        public Medium Medium { get; set; }
        public Guid? MediumSubTypeId { get; set; }
        public MediumSubType MediumSubType { get; set; }
        public Guid? ConditionId { get; set; }
        public Condition Condition { get; set; }
        public Guid? WasteCodeEWCId { get; set; }
        public WasteCodeEWC WasteCodeEWC { get; set; }

        public List<SampleValue> SampleValues { get; set; }
    }
}
