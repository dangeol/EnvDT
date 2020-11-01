using EnvDT.Model.Entity;
using EnvDT.Model.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EnvDT.DataAccess.Repository
{
    public class SampleValueRepository : GenericRepository<SampleValue, EnvDTDbContext>,
        ISampleValueRepository
    {
        public SampleValueRepository(EnvDTDbContext context)
            :base(context)
        {
        }

        public IEnumerable<SampleValue> GetSampleValuesBySampleIdAndLabReportParam(Guid sampleId, Guid labReportParamId)
        {
            return Context.Set<SampleValue>().AsNoTracking().ToList()
                .Where(sv => (sv.SampleId == sampleId && sv.LabReportParamId == labReportParamId));
        }
    }
}