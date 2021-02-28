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
        private Func<IProjectDetailViewModel> _projectDetailVmCreator;

        public ProjectViewModel(ILookupDataService lookupDataService, IEventAggregator eventAggregator, 
            Func<IProjectDetailViewModel> projectDetailVmCreator, IMessageDialogService messageDialogService)
            : base(eventAggregator, messageDialogService, lookupDataService)
        {
            _projectDetailVmCreator = projectDetailVmCreator;
            EventAggregator.GetEvent<OpenDetailViewEvent>().Subscribe(OnItemSelected);
            EventAggregator.GetEvent<DetailSavedEvent>().Subscribe(OnDetailSaved);
            EventAggregator.GetEvent<DetailDeletedEvent>().Subscribe(OnDetailDeleted);
            Projects = new ObservableCollection<NavItemViewModel>();
            Title = Translator["EnvDT.UI.Properties.Strings.MainTabView_Header_Title"];
            IsSampleTab = false;
            LoadModels();
        }

        public ObservableCollection<NavItemViewModel> Projects { get; private set; }

        public ICommand CloseDetailViewCommand { get; }

        // Title of current tab
        public string Title { get; private set; }

        public bool IsSampleTab { get; private set; }

        public Guid? LabReportId { get; set; }

        public override void LoadModels()
        {
            Projects.Clear();
            var navItemViewModelNull = new NavItemViewModelNull();
            Projects.Add(navItemViewModelNull);
            SelectedItem = navItemViewModelNull;
            foreach (var project in LookupDataService.GetAllProjectsLookup())
            {
                Projects.Add(new NavItemViewModel(
                    project.LookupItemId, project.DisplayMember, 
                    nameof(ProjectDetailViewModel),
                    EventAggregator));
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
                var result = MessageDialogService.ShowYesNoDialog(
                    Translator["EnvDT.UI.Properties.Strings.VM_DialogTitle_ConfirmNavigate"],
                    Translator["EnvDT.UI.Properties.Strings.VM_DialogMsg_ConfirmNavigate"]);

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
                            EventAggregator);
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
