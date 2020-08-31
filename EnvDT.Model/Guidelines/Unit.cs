using System;
using System.Collections.Generic;

namespace EnvDT.Model
{
    public class Unit
    {
        public Guid UnitId { get; set; }
        public string UnitName { get; set; }
        public string UnitDescEn { get; set; }
        public string UnitDescDe { get; set; }

        public List<RefValue> RefValues { get; } = new List<RefValue>();
        public List<SampleValue> SampleValues { get; } = new List<SampleValue>();
    }
}
