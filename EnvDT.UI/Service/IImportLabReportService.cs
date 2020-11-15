using System;

namespace EnvDT.UI.Service
{
    public interface IImportLabReportService
    {
        public void RunImport(string filename, Guid? projectId);
        public bool IsLabReportAlreadyPresent(string reportLabIdent);
    }
}