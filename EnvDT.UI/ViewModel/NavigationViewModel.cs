﻿using EnvDT.Model;
using EnvDT.UI.Data.Repository;
using EnvDT.UI.Data.Service;
using EnvDT.UI.Event;
using Prism.Commands;
using Prism.Events;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace EnvDT.UI.ViewModel
{
    public class NavigationViewModel : ViewModelBase, INavigationViewModel
    {
        private IOpenLabReportService _openLabReportService;
        private IEvalLabReportService _evalLabReportService;
        private IProjectRepository _projectRepository;
        private IEventAggregator _eventAggregator;
        private ProjectItemViewModel _selectedProject;

        public NavigationViewModel(IProjectRepository projectRepository, IEventAggregator eventAggregator)
        {
            _projectRepository = projectRepository;
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<ProjectSavedEvent>().Subscribe(OnProjectSaved);
            Projects = new ObservableCollection<ProjectItemViewModel>();
        }

        public NavigationViewModel(IProjectRepository projectRepository, IEventAggregator eventAggregator,
            IOpenLabReportService openLabReportService, IEvalLabReportService evalLabReportService)
        {

            _projectRepository = projectRepository;
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<ProjectSavedEvent>().Subscribe(OnProjectSaved);
            _openLabReportService = openLabReportService;
            _evalLabReportService = evalLabReportService;

            OpenLabReportCommand = new DelegateCommand(OnOpenExecute, OnOpenCanExecute);
            EvalLabReportCommand = new DelegateCommand(OnEvalExecute, OnEvalCanExecute);

            Projects = new ObservableCollection<ProjectItemViewModel>();
        }

        private void OnOpenExecute()
        {
            _openLabReportService.OpenLabReport();
        }

        private bool OnOpenCanExecute()
        {
            // TODO: Check if publication is valid
            return true;
        }

        private void OnEvalExecute()
        {
            _evalLabReportService.evalLabReport();
        }

        private bool OnEvalCanExecute()
        {
            // TODO: Check if publication is valid
            return true;
        }

        private void OnProjectSaved(Project project)
        {
            var projectItem = Projects.Single(p => p.LookupItemId == project.ProjectId);
            projectItem.DisplayMember = $"{project.ProjectNumber} {project.ProjectName}";
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

        public ICommand OpenLabReportCommand { get; }
        public ICommand EvalLabReportCommand { get; }

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
