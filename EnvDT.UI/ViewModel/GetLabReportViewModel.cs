using EnvDT.UI.Data.Services;
using Prism.Commands;
using System.Windows.Input;

namespace EnvDT.UI.ViewModel
{
    public class GetLabReportViewModel : ViewModelBase, IGetLabReportViewModel
    {
        private IOpenLabReportService _openLabReportService;

        public GetLabReportViewModel(IOpenLabReportService openLabReportService)
        {
            _openLabReportService = openLabReportService;

            OpenLabReportCommand = new DelegateCommand(OnOpenExecute, OnOpenCanExecute);
            ImportLabReportCommand = new DelegateCommand(OnImportExecute, OnImportCanExecute);
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

        private void OnImportExecute()
        {

        }

        private bool OnImportCanExecute()
        {
            // TODO: Check if publication is valid
            return true;
        }

        public ICommand OpenLabReportCommand { get; }
        public ICommand ImportLabReportCommand { get; }
    }
}
