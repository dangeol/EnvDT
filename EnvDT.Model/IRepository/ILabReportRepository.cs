using EnvDT.Model.Entity;
using System;

namespace EnvDT.Model.IRepository
{
    public interface ILabReportRepository : IGenericRepository<LabReport>
    {
        public Laboratory GetLabByLabName(string laboratoryName);
        public Laboratory GetLabByLabId(Guid laboratoryId);
        public LabReport GetByReportLabIdent(string ReportLabIdent);
    }
}