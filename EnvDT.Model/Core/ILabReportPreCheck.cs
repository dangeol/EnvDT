using System;
using System.Collections.Generic;

namespace EnvDT.Model.Core
{
    public interface ILabReportPreCheck
    {
        public bool FindMissingParametersUnits(Guid labReportId, IReadOnlyCollection<Guid> publicationIds);
    }
}