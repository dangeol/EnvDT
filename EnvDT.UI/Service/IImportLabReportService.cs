using System;

namespace EnvDT.UI.Service
{
    public interface IImportLabReportService
    {
        void importLabReport(string filename, Guid projectId);
    }
}