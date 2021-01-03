using System;

namespace EnvDT.Model.Entity
{
    public class ConfigXml
    {
        public Guid ConfigXmlId { get; set; }
        public string RootElement { get; set; }

        public Guid LaboratoryId { get; set; }
        public Laboratory Laboratory { get; set; }
    }
}
