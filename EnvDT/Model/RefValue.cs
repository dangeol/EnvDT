using System;
using System.Collections.Generic;
using System.Text;

namespace EnvDT.Model
{
    public class RefValue
    {
        public int RefValueId { get; set; }
        public double Value { get; set; }

        public int ConditionId { get; set; }
        public Condition Condition { get; set; }
        public int RefValueNameId { get; set; }
        public RefValueName RefValueName { get; set; }
        public int UnitId { get; set; }
        public Unit Unit { get; set; }
        public int MediumId { get; set; }
        public Medium Medium { get; set; }
        public int PublicationId { get; set; }
        public Publication Publication { get; set; }

        public List<RefValueParam> RefValueParams { get; } = new List<RefValueParam>();
        public List<RefValueMedSubType> RefValueMedSubTypes { get; set; }
    }
}
