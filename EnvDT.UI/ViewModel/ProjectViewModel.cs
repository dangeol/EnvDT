using EnvDT.Model.IRepository;
using EnvDT.UI.Data.Dialogs;
using EnvDT.UI.Event;
using Prism.Events;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace EnvDT.UI.ViewModel
{
    public class ProjectViewModel : NavViewModelBase, IProjectViewModel
    {
        private IProjectRepository _projectRepository;
        private IEventAggregator _eventAggregator;
        private IMessageDialogService _messageDialogService;

        private Func<IProjectDetailViewModel> _projectDetailVmCreator;

        public ProjectViewModel(IProjectRepository projectRepository, IEventAggregator eventAggregator, 
            Func<IProjectDetailViewModel> projectDetailVmCreator, IMessageDialogService messageDialogService)
            : base(eventAggregator)
        {
            _projectRepository = projectRepository;
            _eventAggregator = eventAggregator;
            _projectDetailVmCreator = projectDetailVmCreator;
            _messageDialogService = messageDialogService;
            _eventAggregator.GetEvent<OpenDetailViewEvent>().Subscribe(OnOpenDetailView);
            _eventAggregator.GetEvent<DetailSavedEvent>().Subscribe(OnDetailSaved);
            _eventAggregator.GetEvent<DetailDeletedEvent>().Subscribe(OnDetailDeleted);
            Projects = new ObservableCollection<NavItemViewModel>();
            LoadModels();
        }

        public ObservableCollection<NavItemViewModel> Projects { get; private set; }

        public override void LoadModels()
        {
            Projects.Clear();
            foreach (var project in _projectRepository.GetAllProjects())
            {
                Projects.Add(new NavItemViewModel(
                    project.LookupItemId, project.DisplayMember, 
                    nameof(ProjectDetailViewModel),
                    _eventAggregator));
            }
        }

        protected override void OnOpenDetailView(OpenDetailViewEventArgs args)
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
