using EnvDT.Model.Entity;
using EnvDT.Model.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace EnvDT.DataAccess.Repository
{
    public class LabReportRepository : GenericRepository<LabReport, EnvDTDbContext>,
        ILabReportRepository
    {
        public LabReportRepository(EnvDTDbContext context)
            :base(context)
        {
        }

        public Laboratory GetLabIdByName(string laboratoryName)
        {
            return Context.Laboratories.AsNoTracking()
                .Single(l => l.LaboratoryName == laboratoryName);
        }
    }
}