using System;
using System.Collections.Generic;

namespace EnvDT.Model.Core
{
    public interface ILabReportPreCheck
    {
        public void FindMissingParametersUnits(Guid labReportId, IReadOnlyCollection<Guid> publicationIds);
    }
}