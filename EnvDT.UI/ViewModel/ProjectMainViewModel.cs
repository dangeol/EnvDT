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

        private Func<IProjectEditViewModel> _projectEditVmCreator;
        private bool _isProjectEditViewEnabled = false;
        private IProjectEditViewModel _projectEditViewModel;
        private ProjectItemViewModel _selectedProject;

        public ProjectMainViewModel(IProjectRepository projectRepository, IEventAggregator eventAggregator, Func<IProjectEditViewModel> projectEditVmCreator)
        {
            _projectRepository = projectRepository;
            _eventAggregator = eventAggregator;
            _projectEditVmCreator = projectEditVmCreator;
            _eventAggregator.GetEvent<OpenProjectEditViewEvent>().Subscribe(OnOpenProjectEditView);
            _eventAggregator.GetEvent<ProjectSavedEvent>().Subscribe(OnProjectSaved);
            Projects = new ObservableCollection<ProjectItemViewModel>();
            AddProjectCommand = new DelegateCommand(OnAddProjectExecute);
        }

        private void OnAddProjectExecute()
        {
            CreateAndLoadProjectEditViewModel(null);
        }

        private void OnOpenProjectEditView(Guid projectId)
        {
            CreateAndLoadProjectEditViewModel(projectId);
        }

        private void CreateAndLoadProjectEditViewModel(Guid? projectId)
        {
            ProjectEditViewModel = _projectEditVmCreator();
            ProjectEditViewModel.Load(projectId);
            IsProjectEditViewEnabled = true;
        }

        private void OnProjectSaved(Project project)
        {
            var displayMember = $"{project.ProjectNumber} {project.ProjectName}";
            var projectItem = Projects.SingleOrDefault(p => p.LookupItemId == project.ProjectId);
            if (projectItem != null)
            { 
                projectItem.DisplayMember = displayMember;
            }
            else
            {
                projectItem = new ProjectItemViewModel(project.ProjectId, 
                    displayMember, _eventAggregator);
                Projects.Add(projectItem);
            }
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

        public bool IsProjectEditViewEnabled 
        {
            get { return _isProjectEditViewEnabled; }
            set
            {
                _isProjectEditViewEnabled = value; 
                OnPropertyChanged(); 
            }
        }

        public IProjectEditViewModel ProjectEditViewModel
        {
            get { return _projectEditViewModel; }
            set
            {
                _projectEditViewModel = value;
                OnPropertyChanged();
            }
        }

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
