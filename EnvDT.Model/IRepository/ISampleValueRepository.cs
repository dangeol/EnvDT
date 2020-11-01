using EnvDT.Model.Entity;
using System;
using System.Collections.Generic;

namespace EnvDT.Model.IRepository
{
    public interface ISampleValueRepository : IGenericRepository<SampleValue>
    {
        public IEnumerable<SampleValue> GetSampleValuesBySampleIdAndLabReportParam(Guid sampleId, Guid labReportParamId);
    }
}