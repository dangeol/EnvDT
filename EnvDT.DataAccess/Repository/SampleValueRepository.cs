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

        public IEnumerable<ParameterLaboratory> GetParamLabsByLabParamName(string labParamName)
        {
            return Context.Set<ParameterLaboratory>().AsNoTracking().ToList()
                .Where(pl => pl.LabParamName == labParamName);
        }

        public Unit GetUnitIdByName(string unitName)
        {
            return Context.Units.AsNoTracking()
                .FirstOrDefault(u => u.UnitName == unitName);
        }

        public IEnumerable<SampleValue> GetSampleValuesBySampleIdAndRefValue(Guid sampleId, RefValue refValue)
        {
            return 
            (
                from s in Context.SampleValues
                    .Where(s => s.SampleId == sampleId && s.ParameterId == refValue.ParameterId)
                join su in Context.Units on s.UnitId equals su.UnitId
                join ru in Context.Units on refValue.UnitId equals ru.UnitId
                where (su.UnitName.Length > 0 &&
                    ru.UnitName.Length > 0 &&
                    su.UnitName.Substring(1, su.UnitName.Length - 1).Equals(ru.UnitName.Substring(1, ru.UnitName.Length - 1))) ||
                    (su.UnitName.Length == 0 && ru.UnitName.Length == 0)
                select s
            )
            .ToList();
        }
    }
}