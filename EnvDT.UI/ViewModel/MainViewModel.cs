using EnvDT.Model;
using EnvDT.UI.Data;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace EnvDT.UI.ViewModel
{
    public partial class MainViewModel : ViewModelBase
    {
        private IPublicationDataService _publicationDataService;
        private Publication _selectedPublication;

        public event PropertyChangedEventHandler PropertyChanged;

        public MainViewModel(IPublicationDataService publicationDataService)
        {
            Publications = new ObservableCollection<Publication>();
            _publicationDataService = publicationDataService;
        }

        public void Load()
        {
            var publications = _publicationDataService.GetAll();
            Publications.Clear();
            foreach (var publication in publications)
            {
                Publications.Add(publication);
            }
        }

        public ObservableCollection<Publication> Publications { get; set; }
  
        public Publication SelectedPublication
        {
            get { return _selectedPublication; }
            set
            {
                _selectedPublication = value;
                OnPropertyChanged();
            }
        }
    }
}
