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

        private Func<IProjectDetailViewModel> _projectEditVmCreator;
        private bool _isProjectEditViewEnabled = false;
        private IDetailViewModel _detailViewModel;
        private ProjectItemViewModel _selectedProject;

        public ProjectMainViewModel(IProjectRepository projectRepository, IEventAggregator eventAggregator, 
            Func<IProjectDetailViewModel> projectDetailVmCreator, IMessageDialogService messageDialogService)
        {
            _projectRepository = projectRepository;
            _eventAggregator = eventAggregator;
            _projectEditVmCreator = projectDetailVmCreator;
            _messageDialogService = messageDialogService;
            _eventAggregator.GetEvent<OpenDetailViewEvent>().Subscribe(OnOpenDetailView);
            _eventAggregator.GetEvent<DetailSavedEvent>().Subscribe(OnDetailSaved);
            _eventAggregator.GetEvent<DetailDeletedEvent>().Subscribe(OnDetailDeleted);
            Projects = new ObservableCollection<ProjectItemViewModel>();
            AddProjectCommand = new DelegateCommand(OnAddProjectExecute);
            LoadProjects();
        }

        public ICommand AddProjectCommand { get; private set; }

        public ObservableCollection<ProjectItemViewModel> Projects { get; private set; }

        public bool IsProjectDetailViewEnabled 
        {
            get { return _isProjectEditViewEnabled; }
            set
            {
                _isProjectEditViewEnabled = value; 
                OnPropertyChanged(); 
            }
        }

        public IDetailViewModel DetailViewModel
        {
            get { return _detailViewModel; }
            set
            {
                _detailViewModel = value;
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
                    _eventAggregator.GetEvent<OpenDetailViewEvent>()
                        .Publish(
                            new OpenDetailViewEventArgs
                            {
                                Id = _selectedProject.LookupItemId,
                                ViewModelName = nameof(ProjectItemViewModel)
                            });
                }
            }
        }

        public void LoadProjects()
        {
            Projects.Clear();
            foreach (var project in _projectRepository.GetAllProjects())
            {
                Projects.Add(new ProjectItemViewModel(
                    project.LookupItemId, project.DisplayMember, 
                    nameof(ProjectDetailViewModel),
                    _eventAggregator));
            }
        }

        private void OnAddProjectExecute()
        {
            CreateAndLoadProjectDetailViewModel(null);
        }

        private void OnOpenDetailView(OpenDetailViewEventArgs args)
        {
            CreateAndLoadProjectDetailViewModel(args);
        }

        private void CreateAndLoadProjectDetailViewModel(OpenDetailViewEventArgs args)
        {
            if (DetailViewModel != null && DetailViewModel.HasChanges)
            { 
                var result = _messageDialogService.ShowYesNoDialog("Question",
                    $"You've made changes. Navigate away?");
                if (result == MessageDialogResult.No)
                {
                    return;
                }
            }

            if (args != null)
            { 
                switch (args.ViewModelName)
                {
                    case nameof(ProjectItemViewModel):
                        DetailViewModel = _projectEditVmCreator();
                        break;
                    case nameof(ProjectDetailViewModel):
                        DetailViewModel = _projectEditVmCreator();
                        break;
                }
                DetailViewModel.Load(args.Id);
            }
            else
            {
                DetailViewModel = _projectEditVmCreator();
                DetailViewModel.Load(null);
            }
            IsProjectDetailViewEnabled = true;
        }

        private void OnDetailSaved(DetailSavedEventArgs args)
        {
            var displayMember = args.DisplayMember;

            switch(args.ViewModelName)
            {
                case nameof(ProjectDetailViewModel):
                    var projectItem = Projects.SingleOrDefault(p => p.LookupItemId == args.Id);
                    if (projectItem != null)
                    {
                        projectItem.DisplayMember = displayMember;
                    }
                    else
                    {
                        projectItem = new ProjectItemViewModel(args.Id, displayMember, 
                            nameof(ProjectDetailViewModel),
                            _eventAggregator);
                        Projects.Add(projectItem);
                    }
                    break;
            }
    }

        private void OnDetailDeleted(DetailDeletedEventArgs args)
        {
            switch(args.ViewModelName)
            {
                case nameof(ProjectDetailViewModel):
                    var projectItem = Projects.SingleOrDefault(p => p.LookupItemId == args.Id);
                    IsProjectDetailViewEnabled = false;
                    if (projectItem != null)
                    {
                        Projects.Remove(projectItem);
                    }
                    break;
            }
    }
    }
}
