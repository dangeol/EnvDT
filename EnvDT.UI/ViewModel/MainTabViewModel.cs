using EnvDT.UI.Event;
using Prism.Events;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace EnvDT.UI.ViewModel
{
    public class MainTabViewModel : ViewModelBase, IMainTabViewModel
    {
        private IEventAggregator _eventAggregator;
        private IProjectViewModel _projectViewModel;
        private Func<ISampleDetailViewModel> _sampleDetailVmCreator;
        private IMainTabViewModel _selectedTabbedViewModel;
        
        public MainTabViewModel(IEventAggregator eventAggregator,
            IProjectViewModel projectViewModel, Func<ISampleDetailViewModel> sampleDetailVmCreator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<OpenDetailViewEvent>().Subscribe(OnItemSelected);
            _eventAggregator.GetEvent<DetailClosedEvent>().Subscribe(OnSampleDetailViewClosed);
            _projectViewModel = projectViewModel;
            _sampleDetailVmCreator = sampleDetailVmCreator;
            TabbedViewModels = new ObservableCollection<IMainTabViewModel>();
            TabbedViewModels.Clear();
            TabbedViewModels.Add(_projectViewModel);
            SelectedTabbedViewModel = TabbedViewModels[0];
        }

        public ObservableCollection<IMainTabViewModel> TabbedViewModels { get; set; }

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
            IMainTabViewModel tabbedViewModel = GetTabbedViewModelByEventArgs(args);
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
            IMainTabViewModel tabbedViewModel = GetTabbedViewModelByEventArgs(args);
            if (tabbedViewModel != null)
            {
                SelectedTabbedViewModel = TabbedViewModels[0];
                TabbedViewModels.Remove(tabbedViewModel);
            }
        }

        private IMainTabViewModel GetTabbedViewModelByEventArgs(IDetailEventArgs args)
        {
            return TabbedViewModels
                   .SingleOrDefault(vm => vm.LabReportId == args.Id
                   && vm.GetType().Name == args.ViewModelName);
        }

        private void CreateAndLoadSampleDetailViewModel(OpenDetailViewEventArgs args)
        {
            ISampleDetailViewModel detailViewModel = _sampleDetailVmCreator();
            detailViewModel.Load(args.Id);
            TabbedViewModels.Add(detailViewModel);
            SelectedTabbedViewModel = detailViewModel;
        }
    }
}
