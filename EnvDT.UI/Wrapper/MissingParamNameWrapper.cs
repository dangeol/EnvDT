using EnvDT.Model.Entity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EnvDT.UI.Wrapper
{
    public class MissingParamNameWrapper : ModelWrapper<ParamNameVariant>
    {
        private ObservableCollection<string> _paramNameAliases = new ObservableCollection<string>();
        private ObservableCollection<string> _languageNames = new ObservableCollection<string>();
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

        public ObservableCollection<string> ParamNameAliases
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

        public ObservableCollection<string> LanguageNames
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
            ClearErrors(propertyName);/*
            switch (propertyName)
            {
                case nameof(ParamNameAlias):
                    if (string.Equals(ParamNameAlias, "", StringComparison.OrdinalIgnoreCase))
                    {
                        yield return "Parameter Name Alias cannot be empty.";
                    }
                    if (Guid.Equals(ParameterId, Guid.Empty))
                    {
                        yield return "ParameterId cannot be empty.";
                    }
                    if (Guid.Equals(LanguageId, Guid.Empty))
                    {
                        yield return "LanguageId Name cannot be empty.";
                    }
                    break;
            }*/
            return null;
        }
    }
}
