﻿using EnvDT.Model;
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

        public ProjectEditViewModel(IProjectRepository projectRepository, IEventAggregator eventAggregator)
        {
            _projectRepository = projectRepository;
            _eventAggregator = eventAggregator;
            SaveProjectCommand = new DelegateCommand(OnSaveExecute, OnSaveCanExecute);
            DeleteProjectCommand = new DelegateCommand(OnDeleteExecute, OnDeleteCanExecute);
        }

        private void Project_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            InvalidateCommands();
        }

        private void InvalidateCommands()
        {
            ((DelegateCommand)SaveProjectCommand).RaiseCanExecuteChanged();
            ((DelegateCommand)DeleteProjectCommand).RaiseCanExecuteChanged();
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

        private void OnDeleteExecute()
        {
            _eventAggregator.GetEvent<ProjectDeletedEvent>()
                .Publish(Project.Model.ProjectId);
            _projectRepository.DeleteProject(Project.Model.ProjectId);
        }

        private bool OnDeleteCanExecute()
        {
            return Project != null && Project.ProjectId != Guid.Empty;
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
                : new Project();
            Project = new ProjectWrapper(project);
            Project.PropertyChanged += Project_PropertyChanged;

            InvalidateCommands();
        } 
    }
}
