using EnvDT.Model.Entity;
using System;
using System.Collections.Generic;

namespace EnvDT.Model.IRepository
{
    public interface ISampleRepository : IGenericRepository<Sample>
    {
        public IEnumerable<Sample> GetAllByLabReportId(Guid labReportId);
    }
}