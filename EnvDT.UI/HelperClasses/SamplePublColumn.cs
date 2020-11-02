using System;

namespace EnvDT.UI.HelperClasses
{
    public class SamplePublColumn
    {
        public string FieldName { get; set; }
        public Guid PublicationId { get; set; }
        public SettingsType Settings { get; set; }
    }
    public enum SettingsType { Default, Combo }
}
