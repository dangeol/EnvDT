using EnvDT.Model.Entity;
using System;
using System.Collections.Generic;

namespace EnvDT.UI.Wrapper
{
    public class SampleWrapper : ModelWrapper<Sample>
    {
        public SampleWrapper(Sample model) : base(model)
        {
        }

        public Guid SampleId 
        { 
            get { return Model.SampleId; } 
        }

        public bool IsSelected { get; set; }

        public string SampleLabIdent
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public DateTime SampleDate
        {
            get { return GetValue<DateTime>(); }
            set { SetValue(value); }
        }

        public string SampleName
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string ProjectAddress 
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        protected override IEnumerable<string> ValidateProperty(string propertyName)
        {
            ClearErrors(propertyName);
            switch (propertyName)
            {
                case nameof(SampleName):
                    if (string.Equals(SampleName, "", StringComparison.OrdinalIgnoreCase))
                    {
                        yield return "Sample Name cannot be empty.";
                    }
                    break;
            }
        }
    }
}
