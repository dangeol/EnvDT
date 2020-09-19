using EnvDT.Model.Entity;
using EnvDT.Model.IRepository;
using EnvDT.UI.Data.Dialogs;
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
        private IMessageDialogService _messageDialogService;
        private ProjectWrapper _project;

        public ProjectEditViewModel(IProjectRepository projectRepository, IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService)
        {
            _projectRepository = projectRepository;
            _eventAggregator = eventAggregator;
            _messageDialogService = messageDialogService;
            SaveProjectCommand = new DelegateCommand(OnSaveExecute, OnSaveCanExecute);
            DeleteProjectCommand = new DelegateCommand(OnDeleteExecute, OnDeleteCanExecute);
        }

        public ICommand SaveProjectCommand { get; private set; }
        public ICommand DeleteProjectCommand { get; private set; }

        public ProjectWrapper Project
        {
            get { return _project; }
            private set
            {
                _project = value;
                OnPropertyChanged();
            }
        }

        public void Load(Guid? projectId)
        {
            var project = projectId.HasValue
                ? _projectRepository.GetProjectById(projectId.Value)
                :  new Project() ;
            Project = new ProjectWrapper(project);
            Project.PropertyChanged += Project_PropertyChanged;

            InvalidateCommands();
            if (Project.ProjectId == Guid.Empty)
            {
                // Trigger the validation
                Project.ProjectName = "";
            }
        }

        private void Project_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            InvalidateCommands();
        }

        private void InvalidateCommands()
        {
            ((DelegateCommand)SaveProjectCommand).RaiseCanExecuteChanged();
            ((DelegateCommand)DeleteProjectCommand).RaiseCanExecuteChanged();

            Project.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(Project.HasErrors))
                {
                    ((DelegateCommand)SaveProjectCommand).RaiseCanExecuteChanged();
                }
            };
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
            return Project != null && Project.IsChanged && !Project.HasErrors;
        }

        private void OnDeleteExecute()
        {
            var result = _messageDialogService.ShowYesNoDialog("Delete Project",
                $"Do you really want to delete the friend '{Project.ProjectClient} {Project.ProjectName}'?");
            if (result == MessageDialogResult.Yes)
            {
                _eventAggregator.GetEvent<ProjectDeletedEvent>()
                    .Publish(Project.Model.ProjectId);
                _projectRepository.DeleteProject(Project.Model.ProjectId);
            }
        }

        private bool OnDeleteCanExecute()
        {
            return Project != null && Project.ProjectId != Guid.Empty;
        }
    }
}
