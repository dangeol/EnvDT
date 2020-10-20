using EnvDT.Model.Core.HelperClasses;
using System;

namespace EnvDT.Model.Core
{
    public interface IEvalLabReportService
    {
        public EvalResult getEvalResult(Guid sampleId, Guid publicationId);
    }
}