using System;

namespace EnvDT.Model.Core
{
    public interface IEvalLabReportService
    {
        public void evalLabReport(Guid sampleId, Guid publicationId);
    }
}