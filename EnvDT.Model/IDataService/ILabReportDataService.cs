using EnvDT.Model.Entity;
using System;
using System.Collections.Generic;

namespace EnvDT.Model.IDataService
{
    public interface ILabReportDataService
    {
        IEnumerable<LookupItem> GetAllLabReportsLookupByProjectId(Guid? projectId);
    }
}
