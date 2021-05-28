using System;

namespace EnvDT.Model.Entity
{
    public class RefValue
    {
        public Guid RefValueId { get; set; }
        public double RValue { get; set; }
        public double RValueAlt { get; set; }
        public Guid PublParamId { get; set; }
        public PublParam PublParam { get; set; }
        public Guid ValuationClassId { get; set; }
        public ValuationClass ValuationClass { get; set; }
        public Guid? FootnoteId { get; set; }
        public Footnote Footnote { get; set; }
    }
}
