using EnvDT.Model.IDataService;
using EnvDT.UI.Dialogs;
using EnvDT.UI.Event;
using Prism.Events;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace EnvDT.UI.ViewModel
{
    public class ProjectViewModel : NavViewModelBase, IProjectViewModel
    {
        private IProjectDataService _projectDataService;
        private IEventAggregator _eventAggregator;
        private IMessageDialogService _messageDialogService;

        private Func<IProjectDetailViewModel> _projectDetailVmCreator;

        public ProjectViewModel(IProjectDataService projectDataService, IEventAggregator eventAggregator, 
            Func<IProjectDetailViewModel> projectDetailVmCreator, IMessageDialogService messageDialogService)
            : base(eventAggregator)
        {
            _projectDataService = projectDataService;
            _eventAggregator = eventAggregator;
            _projectDetailVmCreator = projectDetailVmCreator;
            _messageDialogService = messageDialogService;
            _eventAggregator.GetEvent<OpenDetailViewEvent>().Subscribe(OnItemSelected);
            _eventAggregator.GetEvent<DetailSavedEvent>().Subscribe(OnDetailSaved);
            _eventAggregator.GetEvent<DetailDeletedEvent>().Subscribe(OnDetailDeleted);
            Projects = new ObservableCollection<NavItemViewModel>();
            Title = "Project";
            IsSampleTab = false;
            LoadModels();
        }

        public ObservableCollection<NavItemViewModel> Projects { get; private set; }

        public string Title { get; private set; }

        public ICommand CloseDetailViewCommand { get; }

        public bool IsSampleTab { get; private set; }

        public override void LoadModels()
        {
            Projects.Clear();
            var navItemViewModelNull = new NavItemViewModelNull();
            Projects.Add(navItemViewModelNull);
            SelectedItem = navItemViewModelNull;
            foreach (var project in _projectDataService.GetAllProjectsLookup())
            {
                Projects.Add(new NavItemViewModel(
                    project.LookupItemId, project.DisplayMember, 
                    nameof(ProjectDetailViewModel),
                    _eventAggregator));
            }
        }

        protected override void OnItemSelected(OpenDetailViewEventArgs args)
        {
            if (args.Id != Guid.Empty && args.ViewModelName == nameof(ProjectDetailViewModel))
            {
                CreateAndLoadProjectDetailViewModel(args);
            }
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

            DetailViewModel = _projectDetailVmCreator();
            DetailViewModel.Load(args.Id);
            IsDetailViewEnabled = true;
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
                        projectItem = new NavItemViewModel(args.Id, displayMember, 
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
                    IsDetailViewEnabled = false;
                    if (projectItem != null)
                    {
                        Projects.Remove(projectItem);
                    }
                    SetPropertyValueToNull(this, "DetailViewModel");
                    break;
            }
        }
    }
}
