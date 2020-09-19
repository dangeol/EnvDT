using EnvDT.UI.Data.Service;
using Prism.Commands;
using System.Windows.Input;

namespace EnvDT.UI.ViewModel
{
    public class LabReportMainViewModel : ViewModelBase, ILabReportMainViewModel
    {
        private IOpenLabReportService _openLabReportService;
        private IEvalLabReportService _evalLabReportService;
        public LabReportMainViewModel(IOpenLabReportService openLabReportService, IEvalLabReportService evalLabReportService)
        {
            _openLabReportService = openLabReportService;
            _evalLabReportService = evalLabReportService;

            OpenLabReportCommand = new DelegateCommand(OnOpenExecute, OnOpenCanExecute);
            EvalLabReportCommand = new DelegateCommand(OnEvalExecute, OnEvalCanExecute);

        }

        public ICommand OpenLabReportCommand { get; }
        public ICommand EvalLabReportCommand { get; }

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
    }
}
