using EnvDT.Model;
using EnvDT.UI.Data.Repository;
using System;

namespace EnvDT.UI.ViewModel
{
    public class ProjectEditViewModel : ViewModelBase, IProjectEditViewModel
    {
        private IProjectRepository _projectRepository;
        private Project _project;

        public ProjectEditViewModel(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public void Load(Guid projectId)
        {
            Project = _projectRepository.GetProjectById(projectId);
        }

        public Project Project
        {
            get { return _project; }
            private set
            {
                _project = value;
                OnPropertyChanged();
            }
        }
    }
}
