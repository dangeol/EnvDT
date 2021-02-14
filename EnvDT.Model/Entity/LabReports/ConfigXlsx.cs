using System;

namespace EnvDT.Model.Entity
{
    public class ConfigXlsx : ConfigBase
    {
        public Guid ConfigXlsxId { get; set; }
        public string WorksheetName { get; set; }
    }
}
