using System;

namespace EnvDT.Model.Entity
{
    public class FootnoteRegion
    {
        public Guid FootnoteId { get; set; }
        public Footnote Footnote { get; set; }

        public Guid RegionId { get; set; }
        public Region Region { get; set; }
    }
}
