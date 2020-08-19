using System;

namespace EnvDT.Model
{
    public class ValuationClassCondition
    {
        public Guid ValuationClassId { get; set; }
        public ValuationClass ValuationClass { get; set; }

        public Guid ConditionId { get; set; }
        public Condition Condition { get; set; }
    }
}
