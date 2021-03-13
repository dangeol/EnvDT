using EnvDT.Model.Core.HelperEntity;
using EnvDT.Model.Entity;
using System;
using System.Collections.Generic;

namespace EnvDT.Model.IRepository
{
    public interface ISampleValueRepository : IGenericRepository<SampleValue>
    {
        public IEnumerable<SampleValue> GetSampleValuesBySampleIdAndLabReportParamId(Guid sampleId, Guid labReportParamId);
        public IEnumerable<SampleValueAndLrUnitName> GetSampleValuesAndLrUnitNamesByLabReportIdParamNameDeAndUnitName(
            Guid labReportId, string paramNameDe, string unitName);
    }
}