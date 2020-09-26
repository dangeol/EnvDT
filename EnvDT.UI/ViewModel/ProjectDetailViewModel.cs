using EnvDT.Model.Entity;
using EnvDT.Model.IRepository;
using EnvDT.UI.Dialogs;
using EnvDT.UI.Service;
using EnvDT.UI.Wrapper;
using FriendOrganizer.UI.Wrapper;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace EnvDT.UI.ViewModel
{
    public class ProjectDetailViewModel : DetailViewModelBase, IProjectDetailViewModel
    {
        private IProjectRepository _projectRepository;
        private IMessageDialogService _messageDialogService;
        private IOpenLabReportService _openLabReportService;
        private ProjectWrapper _project;
        private LabReportWrapper _labReport;

        public ProjectDetailViewModel(IProjectRepository projectRepository, IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService, IOpenLabReportService openLabReportService)
            :base(eventAggregator)
        {
            _projectRepository = projectRepository;
            _messageDialogService = messageDialogService;
            _openLabReportService = openLabReportService;

            OpenLabReportCommand = new DelegateCommand(OnOpenLabReportExecute, OnOpenLabReportCanExecute);
            DeleteLabReportCommand = new DelegateCommand(OnDeleteLabReportExecute, OnDeleteLabReportCanExecute);

            LabReports = new ObservableCollection<LabReportWrapper>();
        }

        public ICommand OpenLabReportCommand { get; }
        public ICommand DeleteLabReportCommand { get; }
        public ObservableCollection<LabReportWrapper> LabReports { get; }

        public ProjectWrapper Project
        {
            get { return _project; }
            private set
            {
                _project = value;
                OnPropertyChanged();
            }
        }

        public LabReportWrapper SelectedLabReport
        {
            get { return _labReport; }
            set
            {
                _labReport = value;
                OnPropertyChanged();
                ((DelegateCommand)DeleteLabReportCommand).RaiseCanExecuteChanged();
            }
        }

        public override void Load(Guid? projectId)
        {
            var project = projectId.HasValue
                ? _projectRepository.GetById(projectId.Value)
                : CreateNewProject();

            InitializeProject(projectId, project);

            //InitializeLabReports(project.LabReports);
        }

        private void InitializeProject(Guid? projectId, Project project)
        {
            Project = new ProjectWrapper(project);
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
            ((DelegateCommand)DeleteCommand).RaiseCanExecuteChanged();
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            if (projectId == null)
            {
                // Trigger the validation
                Project.ProjectName = "";
            }
        }

        private void InitializeLabReports(ICollection<LabReport> labReports)
        {
            foreach (var wrapper in LabReports)
            {
                wrapper.PropertyChanged -= LabReportWrapper_PropertyChanged;
            }
            LabReports.Clear();
            foreach (var labReport in labReports)
            {
                var wrapper = new LabReportWrapper(labReport);
                LabReports.Add(wrapper);
                wrapper.PropertyChanged += LabReportWrapper_PropertyChanged;
            }
        }

        private void LabReportWrapper_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!HasChanges)
            {
                HasChanges = _projectRepository.HasChanges();
            }
            if (e.PropertyName == nameof(LabReportWrapper.HasErrors))
            {
                ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            }
        }

        protected override void OnSaveExecute()
        {
            _projectRepository.Save();
            HasChanges = _projectRepository.HasChanges();
            RaiseDetailSavedEvent(Project.ProjectId,
                $"{Project.ProjectNumber} {Project.ProjectName}");
            ((DelegateCommand)DeleteCommand).RaiseCanExecuteChanged();
        }

        protected override bool OnSaveCanExecute()
        {
            return Project != null 
                && !Project.HasErrors 
                && LabReports.All(lr => !lr.HasErrors)
                && HasChanges;
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

        private void OnOpenLabReportExecute()
        {
            _openLabReportService.OpenLabReport();
        }

        private bool OnOpenLabReportCanExecute()
        {
            // TODO: Check if publication is valid
            return true;
        }

        private void OnDeleteLabReportExecute()
        {
            throw new NotImplementedException();
        }

        private bool OnDeleteLabReportCanExecute()
        {
            return SelectedLabReport != null;
        }

        private Project CreateNewProject()
        {
            var project = new Project();
            _projectRepository.Create(project);
            return project;
        }
    }
}
