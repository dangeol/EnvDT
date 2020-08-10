using System;
using System.Collections.Generic;
using System.Text;

namespace EnvDT.Model
{
    public class Medium
    {
        public int MediumId { get; set; }
        public string MediumNameEn { get; set; }
        public string MediumNameDe { get; set; }

        public List<RefValue> RefValues { get; } = new List<RefValue>();
    }
}
