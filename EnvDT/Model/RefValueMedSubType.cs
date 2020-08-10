using System;
using System.Collections.Generic;
using System.Text;

namespace EnvDT.Model
{
    public class RefValueMedSubType
    {
        public int RefValueId { get; set; }
        public RefValue RefValue { get; set; }

        public int MedSubTypeId { get; set; }
        public MediumSubType MediumSubType { get; set; }
    }
}
