using EnvDT.Model.Entity;
using System.Collections.Generic;

namespace EnvDT.Model.IDataService
{
    public interface IProjectDataService
    {
        IEnumerable<LookupItem> GetAllProjectsLookup();
    }
}
