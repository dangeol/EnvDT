using EnvDT.Model.Entity;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace EnvDT.UI.Wrapper
{
    public class ProjectWrapper : ModelWrapper<Project>
    {
        private bool _isChanged;

        public ProjectWrapper(Project model) : base(model)
        {
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName != nameof(IsChanged))
            {
                IsChanged = true;
            }
        }

        public bool IsChanged
        {
            get { return _isChanged; }
            private set 
            { 
                _isChanged = value;
                OnPropertyChanged();
            }
        }

        public void AcceptChanges()
        {
            IsChanged = false;
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
                case nameof(ProjectName):
                    if (string.Equals(ProjectName, "", StringComparison.OrdinalIgnoreCase))
                    {
                        yield return "Project Name cannot be empty.";
                    }
                    break;
            }
        }
    }
}
