using System;
using System.Collections.Generic;

namespace EnvDT.Model.Entity
{
    public class Footnote
    {
        public Guid FootnoteId { get; set; }
        public string FootnoteRef { get; set; }
        public string Expression1 { get; set; }
        public string Expression2 { get; set; }
        public bool IsNotExclusionCriterion { get; set; }
        public string GeneralFootnoteTexts { get; set; }

        public List<FootnoteParam> FootnoteParams { get; set; }
        public List<PublParam> PublParams { get; set; }
        public List<RefValue> RefValues { get; set; }
    }
}
