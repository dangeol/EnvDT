using Prism.Commands;
using System.Windows.Input;

namespace EnvDT.UI.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private ViewModelBase _currentViewModel;
        private IProjectMainViewModel _projectMainViewModel;
        private ILabReportMainViewModel _labReportMainViewModel;

        public MainViewModel(IProjectMainViewModel projectMainViewModel, ILabReportMainViewModel labReportMainViewModel)
        {
            NavCommand = new DelegateCommand<string>(OnNavigationExecute);

            _projectMainViewModel = projectMainViewModel;
            _labReportMainViewModel = labReportMainViewModel;
        }

        private void OnNavigationExecute(string destination)
        {
            switch (destination)
            {
                case "projects":
                    CurrentViewModel = (ViewModelBase)_projectMainViewModel;
                    break;
                case "labReports":
                default:
                    CurrentViewModel = (ViewModelBase)_labReportMainViewModel;
                    break;
            }
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
    }
}
