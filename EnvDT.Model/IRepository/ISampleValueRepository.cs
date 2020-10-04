﻿using EnvDT.Model.Entity;
using System.Collections.Generic;

namespace EnvDT.Model.IRepository
{
    public interface ISampleValueRepository : IGenericRepository<SampleValue>
    {
        public IEnumerable<ParameterLaboratory> GetParamLabsByLabParamName(string labParamName);
        public Unit GetUnitIdByName(string unitName);
    }
}