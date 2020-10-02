using System;

namespace EnvDT.UI.Service
{
    public interface IImportLabReportService
    {
        public void ImportLabReport(string filename, Guid? projectId);
        public bool IsLabReportAlreadyPresent(string reportLabIdent);
    }
}