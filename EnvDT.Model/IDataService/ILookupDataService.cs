using EnvDT.Model.Entity;
using System;
using System.Collections.Generic;

namespace EnvDT.Model.IDataService
{
    public interface ILookupDataService
    {
        public IEnumerable<LookupItem> GetAllProjectsLookup();
        public IEnumerable<LookupItem> GetAllLabReportsLookupByProjectId(Guid? projectId);
        public IEnumerable<LookupItem> GetAllLanguagesLookup();
        public IEnumerable<LookupItem> GetAllMediumSubTypesLookup();
        public IEnumerable<LookupItem> GetAllConditionsLookup();
    }
}
