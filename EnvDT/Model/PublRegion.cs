using System;
using System.Collections.Generic;
using System.Text;

namespace EnvDT.Model
{
    public class PublRegion
    {
        public int PublicationId { get; set; }
        public Publication Publication { get; set; }

        public int RegionId { get; set; }
        public Region Region { get; set; }
    }
}
