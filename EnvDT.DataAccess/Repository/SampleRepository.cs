using EnvDT.Model.Entity;
using EnvDT.Model.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EnvDT.DataAccess.Repository
{
    public class SampleRepository : GenericRepository<Sample, EnvDTDbContext>,
        ISampleRepository
    {
        public SampleRepository(EnvDTDbContext context)
            :base(context)
        {
        }

        public IEnumerable<Sample> GetSamplesByLabReportId(Guid labReportId)
        {
            return Context.Set<Sample>().AsNoTracking().ToList()
                .Where(s => s.LabReportId == labReportId);
        }
    }
}