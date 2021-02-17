using EnvDT.Model.Core.HelperEntity;
using System;
using System.Collections.Generic;

namespace EnvDT.Model.Core
{
    public interface IEvalLabReportService
    {
        public bool LabReportPreCheck(Guid labReportId, IReadOnlyCollection<Guid> publicationIds);
        public EvalResult GetEvalResult(EvalArgs evalArgs);        
    }
}