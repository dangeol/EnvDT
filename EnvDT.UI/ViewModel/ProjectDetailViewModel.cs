using EnvDT.Model.Entity;
using EnvDT.Model.IRepository;
using EnvDT.UI.Data.Dialogs;
using EnvDT.UI.Wrapper;
using Prism.Commands;
using Prism.Events;
using System;

namespace EnvDT.UI.ViewModel
{
    public class ProjectDetailViewModel : DetailViewModelBase, IProjectDetailViewModel
    {
        private IProjectRepository _projectRepository;
        private IMessageDialogService _messageDialogService;
        private ProjectWrapper _project;
        private bool _hasChanges;

        public ProjectDetailViewModel(IProjectRepository projectRepository, IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService)
            :base(eventAggregator)
        {
            _projectRepository = projectRepository;
            _messageDialogService = messageDialogService;
        }

        public ProjectWrapper Project
        {
            get { return _project; }
            private set
            {
                _project = value;
                OnPropertyChanged();
            }
        }

        public override void Load(Guid? projectId)
        {
            var project = projectId.HasValue
                ? _projectRepository.GetById(projectId.Value)
                : CreateNewProject();
            Project = new ProjectWrapper(project);
            Project.PropertyChanged += Project_PropertyChanged;

            InvalidateCommands();
            if (projectId == null)
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
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            ((DelegateCommand)DeleteCommand).RaiseCanExecuteChanged();

            Project.PropertyChanged += (s, e) =>
            {
                if (!HasChanges)
                {
                    HasChanges = _projectRepository.HasChanges();
                }
                if (e.PropertyName == nameof(Project.HasErrors))
                {
                    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
                }
            };
        }

        protected override void OnSaveExecute()
        {
            _projectRepository.Save();
            HasChanges = _projectRepository.HasChanges();
            RaiseDetailSavedEventd(Project.ProjectId,
                $"{Project.ProjectNumber} {Project.ProjectName}");
        }

        protected override bool OnSaveCanExecute()
        {
            return Project != null && !Project.HasErrors && HasChanges;
        }

        protected override void OnDeleteExecute()
        {
            var result = _messageDialogService.ShowYesNoDialog("Delete Project",
                $"Do you really want to delete the friend '{Project.ProjectClient} {Project.ProjectName}'?");
            if (result == MessageDialogResult.Yes)
            {
                RaiseDetailDeletedEvent(Project.Model.ProjectId);
                _projectRepository.Delete(Project.Model);
                _projectRepository.Save();
            }
        }

        protected override bool OnDeleteCanExecute()
        {
            return Project != null && Project.ProjectId != Guid.Empty 
                && _projectRepository.GetById(Project.ProjectId) != null;
        }

        private Project CreateNewProject()
        {
            var project = new Project();
            _projectRepository.Create(project);
            return project;
        }
    }
}
