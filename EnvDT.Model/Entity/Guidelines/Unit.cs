using System;
using System.Collections.Generic;

namespace EnvDT.Model.Entity
{
    public class Unit
    {
        public Guid UnitId { get; set; }
        public string UnitName { get; set; }
        public string UnitDescEn { get; set; }
        public string UnitDescDe { get; set; }

        public List<PublParam> PublParams { get; } = new List<PublParam>();
        public List<SampleValue> SampleValues { get; } = new List<SampleValue>();
        public List<UnitNameVariant> UnitNameVariants { get; set; }
    }
}
