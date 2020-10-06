using System.Collections.ObjectModel;

namespace EnvDT.UI.ViewModel
{
    public class MainTabViewModel : ViewModelBase
    {
        private IProjectViewModel _projectViewModel;

        public MainTabViewModel(IProjectViewModel projectViewModel)
        {
            _projectViewModel = projectViewModel;
            TabbedViewModels = new ObservableCollection<ViewModelBase>();
            TabbedViewModels.Clear();
            TabbedViewModels.Add((ViewModelBase)_projectViewModel);
            SelectedTabbedViewModel = TabbedViewModels[0];
        }
    }
}
