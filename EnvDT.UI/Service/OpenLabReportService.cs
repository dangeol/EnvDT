using System;

namespace EnvDT.UI.Service
{
    public class OpenLabReportService : IOpenLabReportService
    {

        public OpenLabReportService()
        {
        }

        public string OpenLabReport()
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "Document";
            dlg.DefaultExt = ".xls";
            dlg.Filter = "Supported files (*.xls,*.xlsx,*.csv)|*.xls;*.xlsx;*.csv";

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                string filename = dlg.FileName;
                return filename;
            }
            return "<< error >>";
        }
    }
}
