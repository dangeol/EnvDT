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
        private ITab _tab;
        private IMainTabViewModel _selectedTabbedViewModel;
        
        public MainTabViewModel(IEventAggregator eventAggregator, ITab tab,
            IProjectViewModel projectViewModel, Func<ISampleDetailViewModel> sampleDetailVmCreator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<OpenDetailViewEvent>().Subscribe(OnItemSelected);
            _eventAggregator.GetEvent<DetailClosedEvent>().Subscribe(OnSampleDetailViewClosed);
            _projectViewModel = projectViewModel;
            _sampleDetailVmCreator = sampleDetailVmCreator;
            _tab = tab;
            _tab.TabbedViewModels = new ObservableCollection<IMainTabViewModel>();
            _tab.TabbedViewModels.Clear();
            _tab.TabbedViewModels.Add(_projectViewModel);
            SelectedTabbedViewModel = _tab.TabbedViewModels[0];
        }

        public ObservableCollection<IMainTabViewModel> TabbedViewModels
        {
            get { return _tab.TabbedViewModels; }
        }

        public Guid? LabReportId { get; set; }

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
            IMainTabViewModel tabbedViewModel = _tab.GetTabbedViewModelByEventArgs(args);
            if (args.Id != Guid.Empty && tabbedViewModel == null 
                && args.ViewModelName == nameof(SampleDetailViewModel))
            {
                CreateAndLoadSampleDetailViewModel(args);
            }
            else if (tabbedViewModel != null)
            {
                SelectedTabbedViewModel = tabbedViewModel;
            }
        }

        private void OnSampleDetailViewClosed(DetailClosedEventArgs args)
        {
            IMainTabViewModel tabbedViewModel = _tab.GetTabbedViewModelByEventArgs(args);
            if (tabbedViewModel != null)
            {
                SelectedTabbedViewModel = _tab.TabbedViewModels[0];
                _tab.TabbedViewModels.Remove(tabbedViewModel);
            }
        }

        private void CreateAndLoadSampleDetailViewModel(OpenDetailViewEventArgs args)
        {
            ISampleDetailViewModel detailViewModel = _sampleDetailVmCreator();
            detailViewModel.Load(args.Id);
            _tab.TabbedViewModels.Add(detailViewModel);
            SelectedTabbedViewModel = detailViewModel;
        }
    }
}
