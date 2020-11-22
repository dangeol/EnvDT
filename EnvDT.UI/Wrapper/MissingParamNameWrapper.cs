using EnvDT.Model.Entity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EnvDT.UI.Wrapper
{
    public class MissingParamNameWrapper : ModelWrapper<ParamNameVariant>
    {
        private ObservableCollection<LookupItem> _paramNameAliases = new ObservableCollection<LookupItem>();
        private ObservableCollection<LookupItem> _languageNames = new ObservableCollection<LookupItem>();
        private string _paramName;
        private string _parameterName;
        private string _languageName;

        public MissingParamNameWrapper(ParamNameVariant model) : base(model)
        {
        }

        public Guid ParamNameVariantId
        { 
            get { return Model.ParamNameVariantId; } 
        }

        public string ParamName
        {
            get { return _paramName; }
            set
            {
                _paramName = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<LookupItem> ParamNameAliases
        {
            get { return _paramNameAliases; }
            set
            {
                _paramNameAliases = value;
                OnPropertyChanged();
            }
        }

        public string ParameterName
        {
            get { return _parameterName; }
            set
            {
                _parameterName = value;
                OnPropertyChanged();
            }
        }

        public Guid ParameterId
        {
            get { return GetValue<Guid>(); }
            set { SetValue(value); }
        }

        public ObservableCollection<LookupItem> LanguageNames
        {
            get { return _languageNames; }
            set
            {
                _languageNames = value;
                OnPropertyChanged();
            }
        }

        public string LanguageName
        {
            get { return _languageName; }
            set
            {
                _languageName = value;
                OnPropertyChanged();
            }
        }

        public Guid LanguageId
        {
            get { return GetValue<Guid>(); }
            set { SetValue(value); }
        }

        protected override IEnumerable<string> ValidateProperty(string propertyName)
        {
            ClearErrors(propertyName);
            switch (propertyName)
            {
                case nameof(ParameterId):
                    if (Guid.Equals(ParameterId, Guid.Empty))
                    {
                        yield return "Parameter Name must be chosen.";
                    }
                    break;
                case nameof(LanguageId):
                    if (Guid.Equals(LanguageId, Guid.Empty))
                    {
                        yield return "Language must be chosen.";
                    }
                    break;
            }
        }
    }
}
