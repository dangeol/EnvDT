using EnvDT.Model.Entity;
using EnvDT.Model.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
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

        public Laboratory GetLabIdByLabName(string laboratoryName)
        {
            return Context.Laboratories.AsNoTracking()
                .Single(l => l.LaboratoryName == laboratoryName);
        }

        public Laboratory GetLabByLabId(Guid laboratoryId)
        {
            return Context.Laboratories.AsNoTracking()
                .Single(l => l.LaboratoryId == laboratoryId);
        }

        public LabReport GetByReportLabIdent(string ReportLabIdent)
        {
            return Context.LabReports.AsNoTracking()
                .FirstOrDefault(lr => lr.ReportLabIdent == ReportLabIdent);
        }
    }
}