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

        public MainTabViewModel(IEventAggregator eventAggregator, IProjectViewModel projectViewModel,
            Func<ISampleDetailViewModel> sampleDetailVmCreator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<OpenDetailViewEvent>().Subscribe(OnItemSelected);
            _projectViewModel = projectViewModel;
            _sampleDetailVmCreator = sampleDetailVmCreator;
            TabbedViewModels = new ObservableCollection<ViewModelBase>();
            TabbedViewModels.Clear();
            TabbedViewModels.Add((ViewModelBase)_projectViewModel);
            SelectedTabbedViewModel = TabbedViewModels[0];
        }

        private ViewModelBase _selectedTabbedViewModel;

        public ObservableCollection<ViewModelBase> TabbedViewModels { get; set; }

        public ViewModelBase SelectedTabbedViewModel
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
            TabbedViewModels.Add((ViewModelBase)detailViewModel);
            SelectedTabbedViewModel = (ViewModelBase)detailViewModel;
            //detailViewModel.Load(args.Id);
        }
    }
}
