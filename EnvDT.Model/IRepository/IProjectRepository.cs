using EnvDT.Model.Entity;
using System.Collections.Generic;

namespace EnvDT.Model.IRepository
{
    public interface IProjectRepository : IGenericRepository<Project>
    {
        IEnumerable<LookupItem> GetAllProjects();
        void DeleteLabReport(LabReport model);
    }
}