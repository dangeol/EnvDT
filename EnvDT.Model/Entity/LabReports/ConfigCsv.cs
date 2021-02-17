using System;

namespace EnvDT.Model.Entity
{
    public class ConfigCsv : ConfigBase
    {
        public Guid ConfigCsvId { get; set; }
        public int HeaderRow { get; set; }
        public string DelimiterChar { get; set; }
        public string DecimalSepChar { get; set; }
    }
}
