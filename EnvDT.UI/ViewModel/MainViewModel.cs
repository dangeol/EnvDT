using Prism.Commands;
using System.Windows.Input;

namespace EnvDT.UI.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private ViewModelBase _currentViewModel;
        private MainTabViewModel _mainTabViewModel;
        private ISettingsDetailViewModel _settingsDetailViewModel;

        public MainViewModel(MainTabViewModel mainTabViewModel, ISettingsDetailViewModel settingsDetailViewModel)
        {
            NavCommand = new DelegateCommand<string>(OnNavigationExecute);

            _mainTabViewModel = mainTabViewModel;
            _settingsDetailViewModel = settingsDetailViewModel;
        }

        public ICommand NavCommand { get; private set; }

        public ViewModelBase CurrentViewModel
        {
            get { return _currentViewModel; }
            set 
            { 
                _currentViewModel = value; 
                OnPropertyChanged(); 
            } 
        }

        private void OnNavigationExecute(string destination)
        {
            switch (destination)
            {
                case "main":
                    CurrentViewModel = _mainTabViewModel;
                    break;
                case "settings":
                default:
                    CurrentViewModel = (ViewModelBase)_settingsDetailViewModel;
                    break;
            }
        }
    }
}
