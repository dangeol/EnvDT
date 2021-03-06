using EnvDT.Model.Entity;
using EnvDT.Model.IRepository;
using EnvDT.UI.Dialogs;
using EnvDT.UI.Event;
using EnvDT.UI.Wrapper;
using Prism.Commands;
using Prism.Events;
using System;

namespace EnvDT.UI.ViewModel
{
    public class ProjectDetailViewModel : DetailViewModelBase, IProjectDetailViewModel
    {
        private Func<ILabReportViewModel> _labReportDetailVmCreator;
        private ILabReportViewModel _labReportViewModel;
        private ITab _tab;

        private ProjectWrapper _project;

        public ProjectDetailViewModel(IUnitOfWork unitOfWork, IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService, Func<ILabReportViewModel> labReportDetailVmCreator,
            ITab tab)
            :base(eventAggregator, messageDialogService, unitOfWork)
        {
            _labReportDetailVmCreator = labReportDetailVmCreator;
            _tab = tab;
            eventAggregator.GetEvent<DetailSavedEvent>().Subscribe(OnDetailSaved);
            eventAggregator.GetEvent<LabReportImportedEvent>().Subscribe(OnLabReportImported);
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

        public ILabReportViewModel LabReportViewModel
        {
            get { return _labReportViewModel; }
            set
            {
                _labReportViewModel = value;
                OnPropertyChanged();
            }
        }

        public override void Load(Guid? projectId)
        {
            var project = projectId.HasValue
                ? UnitOfWork.Projects.GetById(projectId.Value)
                : CreateNewProject();

            InitializeProject(projectId, project);

            CreateLabReportVm(projectId);
        }

        private void InitializeProject(Guid? projectId, Project project)
        {
            Project = new ProjectWrapper(project);
            Project.PropertyChanged += (s, e) =>
            {
                if (!HasChanges)
                {
                    HasChanges = UnitOfWork.Projects.HasChanges();
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
                Project.ProjectNumber = "";
                Project.ProjectClient = "";
                Project.ProjectName = "";
            }
        }

        private void OnDetailSaved(DetailSavedEventArgs args)
        {
            switch (args.ViewModelName)
            {
                case nameof(ConfigXlsxDetailViewModel):
                    CreateLabReportVm(Project.ProjectId);
                    break;
            }
        }

        private void OnLabReportImported(LabReportImportedEventArgs args)
        {
            CreateLabReportVm(Project.ProjectId);
        }

        private void CreateLabReportVm(Guid? projectId)
        {
            LabReportViewModel = _labReportDetailVmCreator();
            LabReportViewModel.Load(projectId);
        }

        protected override void OnSaveExecute()
        {
            UnitOfWork.Save();
            HasChanges = UnitOfWork.Projects.HasChanges();
            RaiseDetailSavedEvent(Project.ProjectId,
                $"{Project.ProjectNumber} {Project.ProjectName}");
            ((DelegateCommand)DeleteCommand).RaiseCanExecuteChanged();
            CreateLabReportVm(Project.ProjectId);
        }

        protected override bool OnSaveCanExecute()
        {
            return Project != null 
                && !Project.HasErrors 
                && HasChanges;
        }

        protected override void OnDeleteExecute()
        {
            // For simplicity, for now the only condition is that at least one SampleDetailViewModel Tab is open.
            // It would be nice to check which Tabs belongs to the current project and to tell the user
            // to close these.
            if (_tab.TabbedViewModels.Count > 1)
            {
                MessageDialogService.ShowOkDialog(
                    Translator["EnvDT.UI.Properties.Strings.ProjectDetailVM_DialogTitle_DeleteCloseTabs"],
                    Translator["EnvDT.UI.Properties.Strings.ProjectDetailVM_DialogMsg_DeleteCloseTabs"]);
                return;
            }
            var result = MessageDialogService.ShowOkCancelDialog(
                Translator["EnvDT.UI.Properties.Strings.ProjectDetailVM_DialogTitle_ConfirmDeletion"],
                string.Format(Translator["EnvDT.UI.Properties.Strings.ProjectDetailVM_DialogMsg_ConfirmDeletion"],
                Project.ProjectClient, Project.ProjectName));

            if (result == MessageDialogResult.Yes)
            {
                RaiseDetailDeletedEvent(Project.Model.ProjectId);
                SetPropertyValueToNull(this, "LabReportViewModel");
                UnitOfWork.Projects.Delete(Project.Model);
                UnitOfWork.Save();
            }
        }

        protected override bool OnDeleteCanExecute()
        {
            return Project != null && Project.ProjectId != Guid.Empty 
                && UnitOfWork.Projects.GetById(Project.ProjectId) != null;
        }

        private Project CreateNewProject()
        {
            var project = new Project();
            UnitOfWork.Projects.Create(project);
            return project;
        }
    }
}
