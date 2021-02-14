using System;

namespace EnvDT.Model.Entity
{
    public class ConfigBase
    {
        public string IdentWord { get; set; }
        public int IdentWordCol { get; set; }
        public int IdentWordRow { get; set; }
        public int ReportLabidentCol { get; set; }
        public int ReportLabidentRow { get; set; }
        public int FirstSampleValueCol { get; set; }
        public int SampleLabIdentRow { get; set; }
        public int SampleNameRow { get; set; }
        public int FirstDataRow { get; set; }
        public int ParamNameCol { get; set; }
        public int UnitNameCol { get; set; }
        public int DetectionLimitCol { get; set; }
        public int MethodCol { get; set; }

        public Guid LaboratoryId { get; set; }
        public Laboratory Laboratory { get; set; }
    }
}
