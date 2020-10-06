using System.Collections.ObjectModel;

namespace EnvDT.UI.ViewModel
{
    public interface IMainTabViewModel 
    {
        public ObservableCollection<ViewModelBase> TabbedViewModels { get; }
        public ViewModelBase SelectedTabbedViewModel { get; set; }
    }
}
