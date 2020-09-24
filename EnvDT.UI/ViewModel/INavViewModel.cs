using System.Collections.ObjectModel;

namespace EnvDT.UI.ViewModel
{
    public interface INavViewModel
    {
        public ObservableCollection<NavItemViewModel> Models { get; }
        public bool IsDetailViewEnabled { get; }
        public IDetailViewModel DetailViewModel { get; }
        public NavItemViewModel SelectedItem { get; }
    }
}
