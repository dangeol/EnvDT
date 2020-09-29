using EnvDT.Model.Entity;

namespace EnvDT.Model.IRepository
{
    public interface ILabReportRepository : IGenericRepository<LabReport>
    {
        public Laboratory GetLabIdByName(string laboratoryName);
    }
}