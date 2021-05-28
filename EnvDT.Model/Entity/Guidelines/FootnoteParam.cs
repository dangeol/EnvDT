using System;

namespace EnvDT.Model.Entity
{
    public class FootnoteParam
    {
        public Guid FootnoteParamId { get; set; }
        public Guid FootnoteId { get; set; }
        public Footnote Footnote { get; set; }
        public int OrderNo { get; set; }
        public Guid PublicationId { get; set; }
        public Publication Publication { get; set; }
        public Guid ParameterId { get; set; }
        public Parameter Parameter { get; set; }
        public Guid UnitId { get; set; }
        public Unit Unit { get; set; }
    }
}
