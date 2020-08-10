using System;
using System.Collections.Generic;
using System.Text;

namespace EnvDT.Model
{
    public class RefValueName
    {
        public int RefValueNameId { get; set; }
        public string RefValueNameEn { get; set; }
        public string RefValueNameDe { get; set; }

        public List<RefValue> RefValues { get; } = new List<RefValue>();
    }
}
