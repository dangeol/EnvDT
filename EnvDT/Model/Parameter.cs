using System;
using System.Collections.Generic;

namespace EnvDT.Model
{
    public class Parameter
    {
        public Guid ParameterId { get; set; }
        public string ParamNameEn { get; set; }
        public string ParamNameDe { get; set; }
        public string ParamAnnotation { get; set; }

        public List<RefValue> RefValues { get; } = new List<RefValue>();
        public List<CAS> CASs { get; } = new List<CAS>();
        public List<ParameterLaboratory> ParameterLaboratories { get; set; }
    }
}
