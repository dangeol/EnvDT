﻿using System;

namespace EnvDT.Model
{
    public class PublRegion
    {
        public Guid PublicationId { get; set; }
        public Publication Publication { get; set; }

        public Guid RegionId { get; set; }
        public Region Region { get; set; }
    }
}
