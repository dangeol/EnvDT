using System.Collections.ObjectModel;

namespace EnvDT.UI.ViewModel
{
    public class MainTabViewModel : ViewModelBase, IMainTabViewModel
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

        private ViewModelBase _selectedTabbedViewModel;

        public ObservableCollection<ViewModelBase> TabbedViewModels { get; set; }

        public ViewModelBase SelectedTabbedViewModel
        {
            get { return _selectedTabbedViewModel; }
            set
            {
                _selectedTabbedViewModel = value;
                OnPropertyChanged();
            }
        }
    }
}
