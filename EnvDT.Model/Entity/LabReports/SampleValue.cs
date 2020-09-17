using System;
using System.Collections.Generic;

namespace EnvDT.Model.Entity
{
    public class SampleValue
    {
        public Guid SampleValueId { get; set; }
        public double SValue { get; set; }
        public double DetectionLimit { get; set; }

        public Guid SampleId { get; set; }
        public Sample Sample { get; set; }
        public Guid ParameterId { get; set; }
        public Parameter Parameter { get; set; }
        public Guid UnitId { get; set; }
        public Unit Unit { get; set; }

    }
}
