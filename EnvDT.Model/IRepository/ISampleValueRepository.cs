using EnvDT.Model.Core.HelperEntity;
using EnvDT.Model.Entity;
using System;
using System.Collections.Generic;

namespace EnvDT.Model.IRepository
{
    public interface ISampleValueRepository : IGenericRepository<SampleValue>
    {
        public IEnumerable<SampleValue> GetSampleValuesByLabReportParamId(Guid labReportParamId);
        public IEnumerable<SampleValue> GetSampleValuesBySampleIdAndLabReportParamId(Guid sampleId, Guid labReportParamId);
        public IEnumerable<SampleValueAndLrUnitName> GetSampleValuesAndLrUnitNamesByLabReportIdParameterIdAndUnitName(
            Guid labReportId, Guid parameterId, string unitName);
    }
}