using System;
using System.Collections.Generic;

namespace EnvDT.Model.Entity
{
    public class PublParam
    {
        public Guid PublParamId { get; set; }
        public int Position { get; set; }
        public bool IsMandatory { get; set; }
        public string FootnoteId { get; set; }
        public double Tolerance { get; set; }
        public Guid PublicationId { get; set; }
        public Publication Publication { get; set; }
        public Guid ParameterId { get; set; }
        public Parameter Parameter { get; set; }
        public Guid UnitId { get; set; }
        public Unit Unit { get; set; }
        public Guid MediumId { get; set; }
        public Medium Medium { get; set; }

        public List<RefValue> RefValues { get; set; }
    }
}
