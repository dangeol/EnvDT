using System;
using System.Collections.Generic;
using System.Text;

namespace EnvDT.Model
{
    public class Condition
    {
        public int ConditionId { get; set; }
        public string Condition1 { get; set; }
        public string Condition2 { get; set; }

        public List<RefValue> RefValues { get; } = new List<RefValue>();
    }
}
