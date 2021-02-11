﻿using EnvDT.Model.Entity;
using System;
using System.Collections.Generic;

namespace EnvDT.UI.Wrapper
{
    public class ConfigCsvWrapper : ModelWrapper<ConfigCsv>
    {
        public ConfigCsvWrapper(ConfigCsv model) : base(model)
        {
        }

        public Guid ConfigCsvId
        {
            get { return Model.ConfigCsvId; }
        }

        public string IdentWord
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public int IdentWordCol
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        public int IdentWordRow
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
                case nameof(IdentWord):
                    if (string.Equals(IdentWord, "", StringComparison.OrdinalIgnoreCase))
                    {
                        yield return Translator["EnvDT.UI.Properties.Strings.Wrapper_TextBlock_ValidationText"];
                    }
                    break;
                case nameof(IdentWordCol):
                    if (IdentWordCol == -1)
                    {
                        yield return Translator["EnvDT.UI.Properties.Strings.Wrapper_TextBlock_ValidationText"];
                    }
                    break;
                case nameof(IdentWordRow):
                    if (IdentWordRow == -1)
                    {
                        yield return Translator["EnvDT.UI.Properties.Strings.Wrapper_TextBlock_ValidationText"];
                    }
                    break;
                case nameof(ReportLabidentCol):
                    if (ReportLabidentCol == -1)
                    {
                        yield return Translator["EnvDT.UI.Properties.Strings.Wrapper_TextBlock_ValidationText"];
                    }
                    break;
                case nameof(ReportLabidentRow):
                    if (ReportLabidentRow == -1)
                    {
                        yield return Translator["EnvDT.UI.Properties.Strings.Wrapper_TextBlock_ValidationText"];
                    }
                    break;
                case nameof(FirstSampleValueCol):
                    if (FirstSampleValueCol == -1)
                    {
                        yield return Translator["EnvDT.UI.Properties.Strings.Wrapper_TextBlock_ValidationText"];
                    }
                    break;
                case nameof(SampleLabIdentRow):
                    if (SampleLabIdentRow == -1)
                    {
                        yield return Translator["EnvDT.UI.Properties.Strings.Wrapper_TextBlock_ValidationText"];
                    }
                    break;
                case nameof(SampleNameRow):
                    if (SampleNameRow == -1)
                    {
                        yield return Translator["EnvDT.UI.Properties.Strings.Wrapper_TextBlock_ValidationText"];
                    }
                    break;
                case nameof(FirstDataRow):
                    if (FirstDataRow == -1)
                    {
                        yield return Translator["EnvDT.UI.Properties.Strings.Wrapper_TextBlock_ValidationText"];
                    }
                    break;
                case nameof(ParamNameCol):
                    if (ParamNameCol == -1)
                    {
                        yield return Translator["EnvDT.UI.Properties.Strings.Wrapper_TextBlock_ValidationText"];
                    }
                    break;
                case nameof(UnitNameCol):
                    if (UnitNameCol == -1)
                    {
                        yield return Translator["EnvDT.UI.Properties.Strings.Wrapper_TextBlock_ValidationText"];
                    }
                    break;
                case nameof(DetectionLimitCol):
                    if (DetectionLimitCol == -1)
                    {
                        yield return Translator["EnvDT.UI.Properties.Strings.Wrapper_TextBlock_ValidationText"];
                    }
                    break;
            }
        }
    }
}
