using EnvDT.Model.Core.HelperClasses;
using System;

namespace EnvDT.Model.Core
{
    public interface IEvalLabReportService
    {
        public EvalResult evalLabReport(Guid sampleId, Guid publicationId);
    }
}