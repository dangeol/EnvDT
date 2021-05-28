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

        public List<PublParam> PublParams { get; set; }
        public List<FootnoteParam> FootnoteParams { get; set; }
        public List<LabReportParam> LabReportParams { get; set; }
        public List<UnitNameVariant> UnitNameVariants { get; set; }
    }
}
