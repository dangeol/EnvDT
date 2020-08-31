using EnvDT.UI.Event;
using Prism.Events;
using System;

namespace EnvDT.UI.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private IEventAggregator _eventAggregator;
        private IPublicationDetailViewModel _publicationDetailViewModel;
        private Func<IPublicationDetailViewModel> _publicationDetailViewModelCreator;

        public MainViewModel(INavigationViewModel navigationViewModel, IGetLabReportViewModel getLabReportViewModel,
            Func<IPublicationDetailViewModel> publicationDetailViewModelCreator,
            IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _publicationDetailViewModelCreator = publicationDetailViewModelCreator;

            _eventAggregator.GetEvent<OpenPublicationDetailViewEvent>()
                .Subscribe(OnOpenPublicationDetailView);

            NavigationViewModel = navigationViewModel;
            GetLabReportViewModel = getLabReportViewModel;
        }

        public void Load()
        {
            NavigationViewModel.Load();
        }

        public INavigationViewModel NavigationViewModel { get; }
        public IGetLabReportViewModel GetLabReportViewModel { get; }

        public IPublicationDetailViewModel PublicationDetailViewModel
        {
            get { return _publicationDetailViewModel; }
            private set { 
                _publicationDetailViewModel = value;
                OnPropertyChanged();
            }
        }

        private void OnOpenPublicationDetailView(Guid publicationId)
        {
            PublicationDetailViewModel = _publicationDetailViewModelCreator();
            PublicationDetailViewModel.Load(publicationId);
        }
    }
}
