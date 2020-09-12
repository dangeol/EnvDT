using EnvDT.UI.Data.Repository;
using EnvDT.UI.Event;
using EnvDT.UI.Wrapper;
using Prism.Commands;
using Prism.Events;
using System;
using System.Windows.Input;

namespace EnvDT.UI.ViewModel
{
    public class ProjectEditViewModel : ViewModelBase, IProjectEditViewModel
    {
        private IProjectRepository _projectRepository;
        private IEventAggregator _eventAggregator;
        private ProjectWrapper _project;

        public ProjectEditViewModel(IProjectRepository projectRepository, IEventAggregator eventAggregator,
            INavigationViewModel navigationViewModel)
        {
            _projectRepository = projectRepository;
            _eventAggregator = eventAggregator;
            SaveCommand = new DelegateCommand(OnSaveExecute, OnSaveCanExecute);
            NavigationViewModel = navigationViewModel;
            NavigationViewModel.LoadProjects();
        }

        private void Project_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }

        private void OnSaveExecute()
        {
            _projectRepository.SaveProject(Project.Model);
            Project.AcceptChanges();
            _eventAggregator.GetEvent<ProjectSavedEvent>()
                .Publish(Project.Model);
        }

        private bool OnSaveCanExecute()
        {
            return Project != null && Project.IsChanged;
        }

        public ICommand SaveCommand { get; private set; }

        public ProjectWrapper Project
        {
            get { return _project; }
            private set
            {
                _project = value;
                OnPropertyChanged();
            }
        }

        public void Load(Guid projectId)
        {
            var project = _projectRepository.GetProjectById(projectId);
            Project = new ProjectWrapper(project);
            Project.PropertyChanged += Project_PropertyChanged;
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        } 

        public INavigationViewModel NavigationViewModel { get; private set; }
    }
}
