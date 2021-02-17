using EnvDT.Model.Entity;
using System;
using System.Data;

namespace EnvDT.Model.Core.HelperEntity
{
    public class ImportedFileData
    {
        public DataTable WorkSheet { get; set; }
        public Guid ConfigId { get; set; }
        public ConfigCsv ConfigCsv { get; set; }
        public string ConfigType { get; set; }
        public string ReportLabIdent { get; set; }
    }
}
