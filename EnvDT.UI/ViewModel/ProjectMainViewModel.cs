using EnvDT.Model;
using EnvDT.UI.Data.Repository;
using EnvDT.UI.Event;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace EnvDT.UI.ViewModel
{
    public class ProjectMainViewModel : ViewModelBase, IProjectMainViewModel
    {
        private IProjectRepository _projectRepository;
        private IEventAggregator _eventAggregator;
        private ProjectItemViewModel _selectedProject;

        public ProjectMainViewModel(IProjectRepository projectRepository, IEventAggregator eventAggregator)
        {
            _projectRepository = projectRepository;
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<ProjectSavedEvent>().Subscribe(OnProjectSaved);
            Projects = new ObservableCollection<ProjectItemViewModel>();
            AddProjectCommand = new DelegateCommand(OnAddProjectExecute);
        }

        private void OnAddProjectExecute()
        {
            throw new NotImplementedException();
        }

        private void OnProjectSaved(Project project)
        {
            var projectItem = Projects.Single(p => p.LookupItemId == project.ProjectId);
            projectItem.DisplayMember = $"{project.ProjectNumber} {project.ProjectName}";
        }

        public ICommand AddProjectCommand { get; private set; }
        public ICommand DeleteProjectCommand { get; private set; }

        public void LoadProjects()
        {
            Projects.Clear();
            foreach (var project in _projectRepository.GetAllProjects())
            {
                Projects.Add(new ProjectItemViewModel(
                    project.LookupItemId, project.DisplayMember, _eventAggregator));
            }
        }

        public ObservableCollection<ProjectItemViewModel> Projects { get; private set; }
        
        public ProjectItemViewModel SelectedProject
        {
            get { return _selectedProject; }
            set
            {
                _selectedProject = value;
                OnPropertyChanged();
                if (_selectedProject != null)
                {
                    _eventAggregator.GetEvent<OpenProjectEditViewEvent>()
                        .Publish(_selectedProject.LookupItemId);
                }
            }
        }
    }
}
