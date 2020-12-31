using EnvDT.Model.Entity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EnvDT.UI.Wrapper
{
    public class LabWrapper : ModelWrapper<Laboratory>
    {
        private ObservableCollection<LookupItem> _countries;

        public LabWrapper(Laboratory model) : base(model)
        {
            Countries = new ObservableCollection<LookupItem>();
        }

        public Guid LaboratoryId
        { 
            get { return Model.LaboratoryId; } 
        }

        public string LabCompany
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string LabName
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public ObservableCollection<LookupItem> Countries
        {
            get { return _countries; }
            set
            {
                _countries = value;
                OnPropertyChanged();
            }
        }

        public Guid CountryId
        {
            get { return GetValue<Guid>(); }
            set { SetValue(value); }
        }

        protected override IEnumerable<string> ValidateProperty(string propertyName)
        {
            ClearErrors(propertyName);
            switch (propertyName)
            {
                case nameof(LabCompany):
                    if (string.Equals(LabCompany, "", StringComparison.OrdinalIgnoreCase))
                    {
                        yield return Translator["EnvDT.UI.Properties.Strings.Wrapper_TextBlock_ValidationText"];
                    }
                    break;
                case nameof(LabName):
                    if (string.Equals(LabCompany, "", StringComparison.OrdinalIgnoreCase))
                    {
                        yield return Translator["EnvDT.UI.Properties.Strings.Wrapper_TextBlock_ValidationText"];
                    }
                    break;
                case nameof(CountryId):
                    if (Guid.Equals(CountryId, Guid.Empty))
                    {
                        yield return Translator["EnvDT.UI.Properties.Strings.Wrapper_TextBlock_ValidationText"];
                    }
                    break;
            }
        }
    }
}
