using EnvDT.Model.Entity;
using EnvDT.Model.IRepository;
using EnvDT.UI.Data.Dialogs;
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
        private IMessageDialogService _messageDialogService;

        private Func<IProjectEditViewModel> _projectEditVmCreator;
        private bool _isProjectEditViewEnabled = false;
        private IProjectEditViewModel _projectEditViewModel;
        private ProjectItemViewModel _selectedProject;

        public ProjectMainViewModel(IProjectRepository projectRepository, IEventAggregator eventAggregator, 
            Func<IProjectEditViewModel> projectEditVmCreator, IMessageDialogService messageDialogService)
        {
            _projectRepository = projectRepository;
            _eventAggregator = eventAggregator;
            _projectEditVmCreator = projectEditVmCreator;
            _messageDialogService = messageDialogService;
            _eventAggregator.GetEvent<OpenProjectEditViewEvent>().Subscribe(OnOpenProjectEditView);
            _eventAggregator.GetEvent<ProjectSavedEvent>().Subscribe(OnProjectSaved);
            _eventAggregator.GetEvent<ProjectDeletedEvent>().Subscribe(OnProjectDeleted);
            Projects = new ObservableCollection<ProjectItemViewModel>();
            AddProjectCommand = new DelegateCommand(OnAddProjectExecute);
            LoadProjects();
        }

        public ICommand AddProjectCommand { get; private set; }

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

        public void LoadProjects()
        {
            Projects.Clear();
            foreach (var project in _projectRepository.GetAllProjects())
            {
                Projects.Add(new ProjectItemViewModel(
                    project.LookupItemId, project.DisplayMember, _eventAggregator));
            }
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
            if (ProjectEditViewModel != null && ProjectEditViewModel.HasChanges)
            { 
                var result = _messageDialogService.ShowYesNoDialog("Question",
                    $"You've made changes. Navigate away?");
                if (result == MessageDialogResult.No)
                {
                    return;
                }
            }
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

        private void OnProjectDeleted(Guid projectId)
        {
            var projectItem = Projects.SingleOrDefault(p => p.LookupItemId == projectId);
            CreateAndLoadProjectEditViewModel(null);
            IsProjectEditViewEnabled = false;
            if (projectItem != null)
            {
                Projects.Remove(projectItem);
            }
        }
    }
}
