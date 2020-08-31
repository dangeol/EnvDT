using System;
using System.Collections.Generic;

namespace EnvDT.Model
{
    public class Condition
    {
        public Guid ConditionId { get; set; }
        public string ConditionName { get; set; }

        public List<ValuationClassCondition> ValuationClassConditions { get; set; }
    }
}
