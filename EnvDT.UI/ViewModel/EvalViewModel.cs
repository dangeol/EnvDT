using EnvDT.UI.Service;
using Prism.Commands;
using System.Windows.Input;

namespace EnvDT.UI.ViewModel
{
    public class EvalViewModel : ViewModelBase, IEvalViewModel
    {
        private IEvalLabReportService _evalLabReportService;
        public EvalViewModel(IEvalLabReportService evalLabReportService)
        {
            _evalLabReportService = evalLabReportService;

            EvalLabReportCommand = new DelegateCommand(OnEvalExecute, OnEvalCanExecute);
        }

        public ICommand EvalLabReportCommand { get; }

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
