﻿using EnvDT.Model.Entity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EnvDT.UI.Wrapper
{
    public class MissingParamNameWrapper : ModelWrapper<ParamNameVariant>
    {
        private ObservableCollection<LookupItem> _paramNameAliases = new ObservableCollection<LookupItem>();
        private ObservableCollection<LookupItem> _languageNames = new ObservableCollection<LookupItem>();
        private LookupItem _selectedParameter;
        private LookupItem _selectedLanguage;
        private string _paramName;

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

        public string ParamNameAlias
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
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

        // Here parameter is the LabReportParameter LookupItem
        public LookupItem SelectedParameter
        {
            get { return _selectedParameter; }
            set
            {
                _selectedParameter = value;
                ParamNameAlias = _selectedParameter.DisplayMember;
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

        public LookupItem SelectedLanguage
        {
            get { return _selectedLanguage; }
            set
            {
                _selectedLanguage = value;
                LanguageId = _selectedLanguage.LookupItemId;
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
                case nameof(ParamNameAlias):
                    if (string.Equals(ParamNameAlias, "", StringComparison.OrdinalIgnoreCase))
                    {
                        yield return "Parameter Name Alias must be chosen.";
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