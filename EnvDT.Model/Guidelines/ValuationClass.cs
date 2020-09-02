using System;
using System.Collections.Generic;

namespace EnvDT.Model
{
    public class ValuationClass
    {
        public Guid ValuationClassId { get; set; }
        public string ValuationClassName { get; set; }
        public int ValClassLevel { get; set; }

        public Guid PublicationId { get; set; }
        public Publication Publication { get; set; }

        public List<RefValue> RefValues { get; } = new List<RefValue>();
        public List<ValuationClassMedSubType> ValuationClassMedSubTypes { get; set; }
        public List<ValuationClassCondition> ValuationClassConditions { get; set; }
    }
}
