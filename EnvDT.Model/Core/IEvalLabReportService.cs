using EnvDT.Model.Core.HelperClasses;
using System;
using System.Collections.Generic;

namespace EnvDT.Model.Core
{
    public interface IEvalLabReportService
    {
        public EvalResult GetEvalResult(Guid labReportId, Guid sampleId, Guid publicationId);
        public void LabReportPreCheck(Guid labReportId, IReadOnlyCollection<Guid> publicationIds);
    }
}