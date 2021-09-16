using EnvDT.Model.Entity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EnvDT.UI.Wrapper
{
    public class ProjectWrapper : ModelWrapper<Project>
    {
        private ObservableCollection<LookupItem> _countries = new();
        private ObservableCollection<LookupItem> _regions = new();
        public ProjectWrapper(Project model) : base(model)
        {
        }

        public Guid ProjectId 
        { 
            get { return Model.ProjectId; } 
        }

        public string ProjectNumber 
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string ProjectClient 
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string ProjectName 
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public Guid CountryId
        {
            get { return GetValue<Guid>(); }
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

        public Guid RegionId
        {
            get { return GetValue<Guid>(); }
            set { SetValue(value); }
        }

        public ObservableCollection<LookupItem> Regions
        {
            get { return _regions; }
            set
            {
                _regions = value;
                OnPropertyChanged();
            }
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
                case nameof(ProjectNumber):
                    if (string.Equals(ProjectNumber, "", StringComparison.OrdinalIgnoreCase))
                    {
                        yield return Translator["EnvDT.UI.Properties.Strings.Wrapper_TextBlock_ValidationText"];
                    }
                    break;
                case nameof(ProjectClient):
                    if (string.Equals(ProjectClient, "", StringComparison.OrdinalIgnoreCase))
                    {
                        yield return Translator["EnvDT.UI.Properties.Strings.Wrapper_TextBlock_ValidationText"];
                    }
                    break;
                case nameof(ProjectName):
                    if (string.Equals(ProjectName, "", StringComparison.OrdinalIgnoreCase))
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
                case nameof(RegionId):
                    if (Guid.Equals(RegionId, Guid.Empty))
                    {
                        yield return Translator["EnvDT.UI.Properties.Strings.Wrapper_TextBlock_ValidationText"];
                    }
                    break;
            }
        }
    }
}
