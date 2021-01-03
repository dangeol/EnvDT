using EnvDT.Model.Entity;
using System;
using System.Collections.Generic;

namespace EnvDT.UI.Wrapper
{
    public class ConfigXlsxWrapper : ModelWrapper<ConfigXlsx>
    {
        public ConfigXlsxWrapper(ConfigXlsx model) : base(model)
        {
        }

        public Guid ConfigXlsxId
        { 
            get { return Model.ConfigXlsxId; } 
        }

        public string WorksheetName
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public int LabNameCol
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        public int LabNameRow
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        public int ReportLabidentCol
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        public int ReportLabidentRow
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        public int FirstSampleValueCol
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        public int SampleLabIdentRow
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        public int SampleNameRow
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        public int FirstDataRow
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        public int ParamNameCol
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        public int UnitNameCol
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        public int DetectionLimitCol
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        public int MethodCol
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        protected override IEnumerable<string> ValidateProperty(string propertyName)
        {
            ClearErrors(propertyName);
            switch (propertyName)
            {
                case nameof(WorksheetName):
                    if (string.Equals(WorksheetName, "", StringComparison.OrdinalIgnoreCase))
                    {
                        yield return Translator["EnvDT.UI.Properties.Strings.ProjectWrapper_TextBlock_ProjectName"];
                    }
                    break;
            }
        }
    }
}
