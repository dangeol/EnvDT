using Prism.Commands;
using System.Windows.Input;

namespace EnvDT.UI.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private IMenuViewModel _currentViewModel;
        private IMainTabViewModel _mainTabViewModel;
        private ISettingsDetailViewModel _settingsDetailViewModel;

        public MainViewModel(IMainTabViewModel mainTabViewModel, ISettingsDetailViewModel settingsDetailViewModel)
        {
            NavCommand = new DelegateCommand<string>(OnNavigationExecute);

            _mainTabViewModel = mainTabViewModel;
            _settingsDetailViewModel = settingsDetailViewModel;
        }

        public ICommand NavCommand { get; private set; }

        public IMenuViewModel CurrentViewModel
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
                    CurrentViewModel = _settingsDetailViewModel;
                    break;
            }
        }
    }
}
