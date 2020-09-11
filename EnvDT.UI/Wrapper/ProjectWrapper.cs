using EnvDT.Model;
using EnvDT.UI.ViewModel;
using System;
using System.Runtime.CompilerServices;

namespace EnvDT.UI.Wrapper
{
    public class ProjectWrapper : ViewModelBase
    {
        private Project _project;
        private bool _isChanged;

        public ProjectWrapper(Project project)
        {
            _project = project;
        }

        public Project Model {  get { return _project;  } }

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
            get { return _project.ProjectId; } 
        }

        public string ProjectNumber 
        {
            get { return _project.ProjectNumber; }
            set
            {
                _project.ProjectNumber = value;
                OnPropertyChanged();
            }
        }

        public string ProjectClient 
        {
            get { return _project.ProjectClient; }
            set
            {
                _project.ProjectClient = value;
                OnPropertyChanged();
            }
        }

        public string ProjectName 
        {
            get { return _project.ProjectName; }
            set
            {
                _project.ProjectName = value;
                OnPropertyChanged();
            }
        }

        public string ProjectAddress 
        {
            get { return _project.ProjectAddress; }
            set
            {
                _project.ProjectAddress = value;
                OnPropertyChanged();
            }
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName != nameof(IsChanged))
            {
                IsChanged = true;
            }
        }

    }
}
