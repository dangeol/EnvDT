using EnvDT.UI.Data.Services;
using Prism.Commands;
using System.Windows.Input;

namespace EnvDT.UI.ViewModel
{
    public class NavigationViewModel : ViewModelBase, INavigationViewModel
    {
        private IOpenLabReportService _openLabReportService;
        private IEvalLabReportService _evalLabReportService;

        public NavigationViewModel(IOpenLabReportService openLabReportService, IEvalLabReportService evalLabReportService)
        {
            _openLabReportService = openLabReportService;
            _evalLabReportService = evalLabReportService;

            OpenLabReportCommand = new DelegateCommand(OnOpenExecute, OnOpenCanExecute);
            EvalLabReportCommand = new DelegateCommand(OnEvalExecute, OnEvalCanExecute);
        }

        private void OnOpenExecute()
        {
            _openLabReportService.OpenLabReport();
        }

        private bool OnOpenCanExecute()
        {
            // TODO: Check if publication is valid
            return true;
        }

        private void OnEvalExecute()
        {
            _evalLabReportService.evalLabReport();
        }

        private bool OnEvalCanExecute()
        {
            // TODO: Check if publication is valid
            return true;
        }

        public ICommand OpenLabReportCommand { get; }
        public ICommand EvalLabReportCommand { get; }
    }
}
