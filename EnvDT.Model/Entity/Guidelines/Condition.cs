using System;
using System.Collections.Generic;

namespace EnvDT.Model.Entity
{
    public class Condition
    {
        public Guid ConditionId { get; set; }
        public string ConditionName { get; set; }

        public List<ValuationClassCondition> ValuationClassConditions { get; set; }
        public List<Sample> Samples { get; set; }
    }
}
