using Prism.Events;

namespace EnvDT.UI.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private IEventAggregator _eventAggregator;

        public MainViewModel(INavigationViewModel navigationViewModel, 
            IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;

            NavigationViewModel = navigationViewModel;
        }

        public void Load()
        {
        }

        public INavigationViewModel NavigationViewModel { get; }
    }
}
