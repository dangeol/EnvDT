using EnvDT.Model.Entity;
using System;
using System.Collections.Generic;

namespace EnvDT.UI.Wrapper
{
    public class ConfigXlsxWrapper : ModelWrapper<ConfigXlsx>
    {
        public ConfigXlsxWrapper(ConfigXlsx model) : base(model)
        {
            Rows = new Dictionary<int, int>();
            Cols = new Dictionary<char, int>();
            FeedRowDict();
            FeedColDict();            
        }

        public Dictionary<int, int> Rows { get; }

        public Dictionary<char, int> Cols { get; }

        public Guid ConfigXlsxId
        { 
            get { return Model.ConfigXlsxId; } 
        }

        public string WorksheetName
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
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
                case nameof(WorksheetName):
                    if (string.Equals(WorksheetName, "", StringComparison.OrdinalIgnoreCase))
                    {
                        yield return Translator["EnvDT.UI.Properties.Strings.Wrapper_TextBlock_ValidationText"];
                    }
                    break;
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

        private void FeedColDict()
        {
            for (int i = 65; i < 91; i++)
            {
                Cols.Add((char)i, i - 65);
            }
        }

        private void FeedRowDict()
        {
            for (int i = 0; i < 31; i++)
            {
                Rows.Add(i + 1, i);
            }
        }
    }
}
