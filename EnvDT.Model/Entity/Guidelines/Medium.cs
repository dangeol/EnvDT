using System;
using System.Collections.Generic;

namespace EnvDT.Model.Entity
{
    public class Medium
    {
        public Guid MediumId { get; set; }
        public string MediumNameEn { get; set; }
        public string MediumNameDe { get; set; }

        public List<RefValue> RefValues { get; } = new List<RefValue>();
    }
}
