using System;

namespace EnvDT.Model.Entity
{
    public class FootnoteParam
    {
        public Guid FootnoteParamId { get; set; }
        public Guid FootnoteId { get; set; }
        public Footnote Footnote { get; set; }
        public int OrderNo { get; set; }
        public Guid PublParamId { get; set; }
        public PublParam PublParam { get; set; }
    }
}
