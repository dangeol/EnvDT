using System;
using System.Collections.Generic;

namespace EnvDT.Model.Entity
{
    public class Parameter
    {
        public Guid ParameterId { get; set; }
        public string ParamNameEn { get; set; }
        public string ParamNameDe { get; set; }
        public string ParamAnnotation { get; set; }

        public List<PublParam> PublParams { get; set; }
        public List<CAS> CASs { get; set; }
        public List<ParamNameVariant> ParamNameVariants { get; set; }
        public List<LabReportParam> LabReportParams { get; set; }
    }
}
