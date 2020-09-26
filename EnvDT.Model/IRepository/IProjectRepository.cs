using EnvDT.Model.Entity;

namespace EnvDT.Model.IRepository
{
    public interface IProjectRepository : IGenericRepository<Project>
    {
        void DeleteLabReport(LabReport model);
    }
}