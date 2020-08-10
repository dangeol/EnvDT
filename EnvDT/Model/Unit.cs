using System;
using System.Collections.Generic;
using System.Text;

namespace EnvDT.Model
{
    public class Unit
    {
        public int UnitId { get; set; }
        public string UnitName { get; set; }
        public string UnitDescEn { get; set; }
        public string UnitDescDe { get; set; }

        public List<RefValue> RefValues { get; } = new List<RefValue>();
    }
}
