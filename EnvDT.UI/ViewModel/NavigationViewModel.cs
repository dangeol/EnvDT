using EnvDT.UI.Data.Lookups;
using EnvDT.UI.Event;
using Prism.Events;
using System.Collections.ObjectModel;
using System.Linq;

namespace EnvDT.UI.ViewModel
{
    public class NavigationViewModel : ViewModelBase, INavigationViewModel
    {
        private IPublicationLookupDataService _publicationLookupService;
        private IEventAggregator _eventAggregator;

        public NavigationViewModel(IPublicationLookupDataService publicationLookupDataService,
            IEventAggregator eventAggregator)
        {
            _publicationLookupService = publicationLookupDataService;
            _eventAggregator = eventAggregator;
            Publications = new ObservableCollection<NavigationItemViewModel>();
            _eventAggregator.GetEvent<AfterPublicationSavedEvent>()
                .Subscribe(AfterFriendSaved);
        }

        private void AfterFriendSaved(AfterPublicationSavedEventArgs obj)
        {
            var lookupItem = Publications.Single(l => l.LookupItemId == obj.PublicationId);
            lookupItem.DisplayMember = obj.DisplayMember;
        }

        public void Load()
        {
            var lookup = _publicationLookupService.GetPublicationLookup();
            Publications.Clear();
            foreach (var item in lookup)
            {
                Publications.Add(new NavigationItemViewModel(item.LookupItemId, item.DisplayMember));
            }
        }

        public ObservableCollection<NavigationItemViewModel> Publications { get; }

        private NavigationItemViewModel _selectedPublication;

        public NavigationItemViewModel SelectedPublication
        {
            get { return _selectedPublication; }
            set 
            { 
                _selectedPublication = value;
                OnPropertyChanged();
                if(_selectedPublication != null)
                {
                    _eventAggregator.GetEvent<OpenPublicationDetailViewEvent>()
                        .Publish(_selectedPublication.LookupItemId);
                }
            }
        }

    }
}
