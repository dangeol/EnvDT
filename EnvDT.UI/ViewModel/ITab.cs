using EnvDT.UI.Event;
using System.Collections.ObjectModel;

namespace EnvDT.UI.ViewModel
{
    public interface ITab
    {
        public ObservableCollection<IMainTabViewModel> TabbedViewModels { get; set; }
        public IMainTabViewModel GetTabbedViewModelByEventArgs(IDetailEventArgs args);
    }
}
