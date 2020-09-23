using EnvDT.Model.IRepository;
using EnvDT.UI.Data.Dialogs;
using EnvDT.UI.Event;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows.Input;

namespace EnvDT.UI.ViewModel
{
    public class ProjectMainViewModel : ViewModelBase, IProjectMainViewModel
    {
        private IProjectRepository _projectRepository;
        private IEventAggregator _eventAggregator;
        private IMessageDialogService _messageDialogService;

        private Func<IProjectDetailViewModel> _projectDetailVmCreator;
        private bool _isProjectEditViewEnabled = false;
        private IDetailViewModel _detailViewModel;
        private ProjectItemViewModel _selectedProject;

        public ProjectMainViewModel(IProjectRepository projectRepository, IEventAggregator eventAggregator, 
            Func<IProjectDetailViewModel> projectDetailVmCreator, IMessageDialogService messageDialogService)
        {
            _projectRepository = projectRepository;
            _eventAggregator = eventAggregator;
            _projectDetailVmCreator = projectDetailVmCreator;
            _messageDialogService = messageDialogService;
            _eventAggregator.GetEvent<OpenDetailViewEvent>().Subscribe(OnOpenDetailView);
            _eventAggregator.GetEvent<DetailSavedEvent>().Subscribe(OnDetailSaved);
            _eventAggregator.GetEvent<DetailDeletedEvent>().Subscribe(OnDetailDeleted);
            Projects = new ObservableCollection<ProjectItemViewModel>();
            CreateNewDetailCommand = new DelegateCommand<Type>(OnCreateNewDetailExecute);
            LoadProjects();
        }

        public ICommand CreateNewDetailCommand { get; private set; }

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
                                ViewModelName = nameof(ProjectDetailViewModel)
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

        private void OnCreateNewDetailExecute(Type viewModelType)
        {
            OnOpenDetailView(
                new OpenDetailViewEventArgs
                {
                    ViewModelName = viewModelType.Name
                });
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

            switch (args.ViewModelName)
            {
                case nameof(ProjectDetailViewModel):
                    DetailViewModel = _projectDetailVmCreator();
                    break;
            }
            DetailViewModel.Load(args.Id);
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
                    SetPropertyValueToNull(this);
                    break;
            }
        }

        private void SetPropertyValueToNull(object instance)
        {
            PropertyInfo prop = instance.GetType().GetProperty("DetailViewModel");
            prop.SetValue(instance, null, null); //We need this overload for .NET < 4.5
        }
    }
}
