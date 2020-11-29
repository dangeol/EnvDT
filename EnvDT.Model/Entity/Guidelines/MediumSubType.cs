using System;
using System.Collections.Generic;

namespace EnvDT.Model.Entity
{
    public class MediumSubType
    {
        public Guid MedSubTypeId { get; set; }
        public string MedSubTypeNameEn { get; set; }
        public string MedSubTypeNameDe { get; set; }

        public List<ValuationClassMedSubType> ValuationClassMedSubTypes { get; set; }
        public List<Sample> Samples { get; set; }
    }
}
