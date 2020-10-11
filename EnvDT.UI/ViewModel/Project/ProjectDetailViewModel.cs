﻿using EnvDT.Model.Entity;
using EnvDT.Model.IRepository;
using EnvDT.UI.Dialogs;
using EnvDT.UI.Wrapper;
using Prism.Commands;
using Prism.Events;
using System;

namespace EnvDT.UI.ViewModel
{
    public class ProjectDetailViewModel : DetailViewModelBase, IProjectDetailViewModel
    {
        private IUnitOfWork _unitOfWork;
        private IMessageDialogService _messageDialogService;
        private Func<ILabReportViewModel> _labReportDetailVmCreator;
        private ILabReportViewModel _labReportViewModel;

        private ProjectWrapper _project;

        public ProjectDetailViewModel(IUnitOfWork unitOfWork, IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService, Func<ILabReportViewModel> labReportDetailVmCreator)
            :base(eventAggregator)
        {
            _unitOfWork = unitOfWork;
            _messageDialogService = messageDialogService;
            _labReportDetailVmCreator = labReportDetailVmCreator;
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
                ? _unitOfWork.Projects.GetById(projectId.Value)
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
                    HasChanges = _unitOfWork.Projects.HasChanges();
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

        private void CreateLabReportVm(Guid? projectId)
        {
            LabReportViewModel = _labReportDetailVmCreator();
            LabReportViewModel.Load(projectId);
        }

        protected override void OnSaveExecute()
        {
            _unitOfWork.Save();
            HasChanges = _unitOfWork.Projects.HasChanges();
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
            var result = _messageDialogService.ShowOkCancelDialog("Delete Project",
                $"Do you really want to delete the friend '{Project.ProjectClient} {Project.ProjectName}'?");
            if (result == MessageDialogResult.Yes)
            {
                RaiseDetailDeletedEvent(Project.Model.ProjectId);
                SetPropertyValueToNull(this, "LabReportViewModel");
                _unitOfWork.Projects.Delete(Project.Model);
                _unitOfWork.Save();
            }
        }

        protected override bool OnDeleteCanExecute()
        {
            return Project != null && Project.ProjectId != Guid.Empty 
                && _unitOfWork.Projects.GetById(Project.ProjectId) != null;
        }

        private Project CreateNewProject()
        {
            var project = new Project();
            _unitOfWork.Projects.Create(project);
            return project;
        }
    }
}
