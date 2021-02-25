using EnvDT.Model.Entity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EnvDT.UI.Wrapper
{
    public class ConfigCsvWrapper : ModelWrapper<ConfigCsv>
    {
        public ConfigCsvWrapper(ConfigCsv model) : base(model)
        {
            Rows = new Dictionary<int, int>();
            Cols = new ObservableCollection<int>();
            FeedRows();
            FeedCols();
        }

        public Dictionary<int, int> Rows { get; }

        public ObservableCollection<int> Cols { get; }

        public Guid ConfigCsvId
        {
            get { return Model.ConfigCsvId; }
        }

        public int HeaderRow
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        public string DelimiterChar
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string DecimalSepChar
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string IdentWord
        {
            get { return GetValue<string>(); }
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
                case nameof(HeaderRow):
                    if (HeaderRow == -1)
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
                case nameof(DelimiterChar):
                    if (string.Equals(DelimiterChar, "", StringComparison.OrdinalIgnoreCase))
                    {
                        yield return Translator["EnvDT.UI.Properties.Strings.Wrapper_TextBlock_ValidationText"];
                    }
                    break;
                case nameof(DecimalSepChar):
                    if (string.Equals(DecimalSepChar, "", StringComparison.OrdinalIgnoreCase))
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

        private void FeedCols()
        {
            for (int i = 0; i < 31; i++)
            {
                Cols.Add(i);
            }
        }

        private void FeedRows()
        {
            for (int i = 0; i < 31; i++)
            {
                Rows.Add(i + 1, i);
            }
        }
    }
}
