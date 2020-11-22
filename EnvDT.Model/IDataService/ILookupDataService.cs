using EnvDT.Model.Entity;
using System;
using System.Collections.Generic;

namespace EnvDT.Model.IDataService
{
    public interface ILookupDataService
    {
        IEnumerable<LookupItem> GetAllProjectsLookup();
        IEnumerable<LookupItem> GetAllLabReportsLookupByProjectId(Guid? projectId);
    }
}
