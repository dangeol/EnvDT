using EnvDT.UI.Event;
using Prism.Events;
using System;

namespace EnvDT.UI.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private IEventAggregator _eventAggregator;
        private IProjectEditViewModel _projectEditViewModel;
        private Func<IProjectEditViewModel> _projectEditVmCreator;

        public MainViewModel(IProjectMainViewModel projectMainViewModel,
            Func<IProjectEditViewModel> projectEditVmCreator,
            IEventAggregator eventAggregator)
        {
            ProjectMainViewModel = projectMainViewModel;
            _projectEditVmCreator = projectEditVmCreator;
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<OpenProjectEditViewEvent>().Subscribe(OnOpenProjectEditView);
            ProjectEditViewModel = _projectEditVmCreator();
        }

        private void OnOpenProjectEditView(Guid projectId)
        {
            ProjectEditViewModel.Load(projectId);
        }

        public void Load()
        {
        }

        public IProjectMainViewModel ProjectMainViewModel { get; private set; }

        public IProjectEditViewModel ProjectEditViewModel
        {
            get { return _projectEditViewModel; }
            set
            {
                _projectEditViewModel = value;
                OnPropertyChanged();
            }
        }
    }
}
