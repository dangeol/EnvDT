using EnvDT.UI.Event;
using Prism.Events;
using System;
using System.Collections.ObjectModel;

namespace EnvDT.UI.ViewModel
{
    public class MainTabViewModel : ViewModelBase, IMainTabViewModel
    {
        private IEventAggregator _eventAggregator;
        private IProjectViewModel _projectViewModel;
        private Func<ISampleDetailViewModel> _sampleDetailVmCreator;
        private IMainTabViewModel _selectedTabbedViewModel;

        public MainTabViewModel(IEventAggregator eventAggregator, IProjectViewModel projectViewModel,
            Func<ISampleDetailViewModel> sampleDetailVmCreator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<OpenDetailViewEvent>().Subscribe(OnItemSelected);
            _projectViewModel = projectViewModel;
            _sampleDetailVmCreator = sampleDetailVmCreator;
            TabbedViewModels = new ObservableCollection<IMainTabViewModel>();
            TabbedViewModels.Clear();
            TabbedViewModels.Add(_projectViewModel);
            SelectedTabbedViewModel = TabbedViewModels[0];
        }

        public ObservableCollection<IMainTabViewModel> TabbedViewModels { get; set; }

        public IMainTabViewModel SelectedTabbedViewModel
        {
            get { return _selectedTabbedViewModel; }
            set
            {
                _selectedTabbedViewModel = value;
                OnPropertyChanged();
            }
        }

        private void OnItemSelected(OpenDetailViewEventArgs args)
        {
            if (args.Id != Guid.Empty && args.ViewModelName == nameof(SampleDetailViewModel))
            {
                CreateAndLoadSampleDetailViewModel(args);
            }
        }

        private void CreateAndLoadSampleDetailViewModel(OpenDetailViewEventArgs args)
        {
            ISampleDetailViewModel detailViewModel = _sampleDetailVmCreator(); 
            TabbedViewModels.Add(detailViewModel);
            SelectedTabbedViewModel = detailViewModel;
            //detailViewModel.Load(args.Id);
        }
    }
}
