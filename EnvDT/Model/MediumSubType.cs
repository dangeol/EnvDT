using System;
using System.Collections.Generic;
using System.Text;

namespace EnvDT.Model
{
    public class MediumSubType
    {
        public int MedSubTypeId { get; set; }
        public string MedSubTypeNameEn { get; set; }
        public string MedSubTypeNameDe { get; set; }

        public List<RefValueMedSubType> RefValueMedSubTypes { get; set; }
    }
}
