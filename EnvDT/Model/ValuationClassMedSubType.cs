﻿using System;

namespace EnvDT.Model
{
    public class ValuationClassMedSubType
    {
        public Guid ValuationClassId { get; set; }
        public ValuationClass ValuationClass { get; set; }

        public Guid MedSubTypeId { get; set; }
        public MediumSubType MediumSubType { get; set; }
    }
}
