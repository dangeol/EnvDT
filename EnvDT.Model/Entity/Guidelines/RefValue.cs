using System;

namespace EnvDT.Model.Entity
{
    public class RefValue
    {
        public Guid RefValueId { get; set; }
        public double RValue { get; set; }
        public Guid PublicationId { get; set; }
        public Publication Publication { get; set; }
        public Guid ParameterId { get; set; }
        public Parameter Parameter { get; set; }
        public Guid UnitId { get; set; }
        public Unit Unit { get; set; }
        public Guid ValuationClassId { get; set; }
        public ValuationClass ValuationClass { get; set; }
        public Guid MediumId { get; set; }
        public Medium Medium { get; set; }
    }
}
