using Prism.Commands;
using System.Windows.Input;

namespace EnvDT.UI.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private IMenuViewModel _currentViewModel;
        private IMainTabViewModel _mainTabViewModel;
        private ILabViewModel _labViewModel;
        private ISettingsDetailViewModel _settingsDetailViewModel;
        private IInfoDetailViewModel _infoDetailViewModel;

        public MainViewModel(IMainTabViewModel mainTabViewModel, ILabViewModel labViewModel,
            ISettingsDetailViewModel settingsDetailViewModel, IInfoDetailViewModel infoDetailViewModel)
        {
            NavCommand = new DelegateCommand<string>(OnNavigationExecute);

            _mainTabViewModel = mainTabViewModel;
            _labViewModel = labViewModel;
            _settingsDetailViewModel = settingsDetailViewModel;
            _infoDetailViewModel = infoDetailViewModel;
            CurrentViewModel = _mainTabViewModel;
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
                case "labconfig":
                    CurrentViewModel = _labViewModel;
                    break;
                case "settings":
                    CurrentViewModel = _settingsDetailViewModel;
                    break;
                case "info":
                    CurrentViewModel = _infoDetailViewModel;
                    break;
                default:
                    CurrentViewModel = _mainTabViewModel;
                    break;
            }
        }
    }
}
