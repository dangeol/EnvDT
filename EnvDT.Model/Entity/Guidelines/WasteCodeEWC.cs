using System;
using System.Collections.Generic;

namespace EnvDT.Model.Entity
{
    public class WasteCodeEWC
    {
        public Guid WasteCodeEWCId { get; set; }
        public string WasteCodeNumber { get; set; }
        public string WasteCodeDescrEn { get; set; }
        public string WasteCodeDescrDeAVV { get; set; }
        public List<Sample> Samples { get; set; }
    }
}
