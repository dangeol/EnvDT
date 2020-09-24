using System.Collections.ObjectModel;

namespace EnvDT.UI.ViewModel
{
    public interface IProjectViewModel
    {
        public ObservableCollection<NavItemViewModel> Projects { get; }
    }
}