using Prism.Commands;
using System.Windows.Input;

namespace EnvDT.UI.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private ViewModelBase _currentViewModel;
        private IProjectViewModel _projectViewModel;
        private IEvalViewModel _labReportMainViewModel;

        public MainViewModel(IProjectViewModel projectViewModel, IEvalViewModel labReportMainViewModel)
        {
            NavCommand = new DelegateCommand<string>(OnNavigationExecute);

            _projectViewModel = projectViewModel;
            _labReportMainViewModel = labReportMainViewModel;
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
                case "projects":
                    CurrentViewModel = (ViewModelBase)_projectViewModel;
                    break;
                case "labReports":
                default:
                    CurrentViewModel = (ViewModelBase)_labReportMainViewModel;
                    break;
            }
        }
    }
}
