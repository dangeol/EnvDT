namespace EnvDT.UI.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel(INavigationViewModel navigationViewModel,
            IPublicationDetailViewModel publicationDetailViewModel)
        {
            NavigationViewModel = navigationViewModel;
            PublicationDetailViewModel = publicationDetailViewModel;
        }

        public void Load()
        {
            NavigationViewModel.Load();
        }

        public INavigationViewModel NavigationViewModel { get; }
        public IPublicationDetailViewModel PublicationDetailViewModel { get; }
    }
}
