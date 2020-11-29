using EnvDT.Model.Entity;
using System;
using System.Collections.Generic;

namespace EnvDT.Model.IRepository
{
    public interface IRefValueRepository : IGenericRepository<RefValue>
    {
        public IEnumerable<RefValue> GetRefValuesByPublParamIdAndSample(Guid publParamId, Sample sample);
        public IEnumerable<RefValue> GetRefValuesWithMedSubTypesByPublParamIdAndSample(Guid publParamId, Sample sample);
        public IEnumerable<RefValue> GetRefValuesWithConditionsByPublParamIdAndSample(Guid publParamId, Sample sample);
        public IEnumerable<RefValue> GetRefValuesWithMedSubTypesAndConditionsByPublParamIdAndSample(Guid publParamId, Sample sample);
    }
}