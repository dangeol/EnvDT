using System;

namespace EnvDT.UI.Data.Services
{
    public class OpenLabReportService : IOpenLabReportService
    {
        private IImportLabReportService _importLabReportService;

        public OpenLabReportService(IImportLabReportService importLabReportService)
        {
            _importLabReportService = importLabReportService;
        }
        public void OpenLabReport()
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "Document";
            dlg.DefaultExt = ".xls";
            dlg.Filter = "Excel documents (.xls)|*.xls";

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                string filename = dlg.FileName;
                _importLabReportService.importLabReport(filename);
            }
        }
    }
}
