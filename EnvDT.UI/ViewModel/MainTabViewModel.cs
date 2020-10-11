using EnvDT.Model.IRepository;
using EnvDT.UI.Event;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace EnvDT.UI.ViewModel
{
    public class MainTabViewModel : ViewModelBase, IMainTabViewModel
    {
        private IEventAggregator _eventAggregator;
        private IUnitOfWork _unitOfWork;
        private IProjectViewModel _projectViewModel;
        private Func<ISampleDetailViewModel> _sampleDetailVmCreator;
        private IMainTabViewModel _selectedTabbedViewModel;
        private string _title = "Project";

        public MainTabViewModel(IEventAggregator eventAggregator, IUnitOfWork unitOfWork,
            IProjectViewModel projectViewModel, Func<ISampleDetailViewModel> sampleDetailVmCreator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<OpenDetailViewEvent>().Subscribe(OnItemSelected);
            _unitOfWork = unitOfWork;
            _projectViewModel = projectViewModel;
            _sampleDetailVmCreator = sampleDetailVmCreator;
            TabbedViewModels = new ObservableCollection<IMainTabViewModel>();
            TabbedViewModels.Clear();
            TabbedViewModels.Add(_projectViewModel);
            CloseDetailViewCommand = new DelegateCommand(OnCloseDetailViewExecute);
            SelectedTabbedViewModel = TabbedViewModels[0];
            IsSampleTab = false;
        }

        public ObservableCollection<IMainTabViewModel> TabbedViewModels { get; set; }

        public ICommand CloseDetailViewCommand { get; }

        public bool IsSampleTab { get; set; }

        public string Title
        {
            get { return _title; }
            protected set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        public IMainTabViewModel SelectedTabbedViewModel
        {
            get { return _selectedTabbedViewModel; }
            set
            {
                _selectedTabbedViewModel = value;
                OnPropertyChanged();
                System.Diagnostics.Debug.WriteLine("_selectedTabbedViewModel: "+_selectedTabbedViewModel);
            }
        }

        private void SetTitle(OpenDetailViewEventArgs args)
        {
            var ReportLabIdent = _unitOfWork.LabReports.GetById((Guid)args.Id).ReportLabIdent;
            Title = ReportLabIdent;
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
            IsSampleTab = true;
            SetTitle(args);
            detailViewModel.Load(args.Id);
        }

        protected void OnCloseDetailViewExecute() 
        {
        }
    }
}
