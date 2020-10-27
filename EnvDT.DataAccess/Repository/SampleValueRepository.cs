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

        public IEnumerable<ParamNameVariant> GetParamNameVariantsByLabParamName(string labParamName)
        {
            return Context.Set<ParamNameVariant>().AsNoTracking().ToList()
                .Where(pl => pl.ParamNameAlias == labParamName);
        }

        public Unit GetUnitIdByName(string unitName)
        {
            return Context.Units.AsNoTracking()
                .FirstOrDefault(u => u.UnitName == unitName);
        }

        public IEnumerable<SampleValue> GetSampleValuesBySampleIdAndPublParam(Guid sampleId, PublParam publParam)
        {
            return 
            (
                from s in Context.SampleValues
                    .Where(s => s.SampleId == sampleId && s.ParameterId == publParam.ParameterId)
                join su in Context.Units on s.UnitId equals su.UnitId
                join ppu in Context.Units on publParam.UnitId equals ppu.UnitId
                where (su.UnitName.Length > 0 &&
                    ppu.UnitName.Length > 0 &&
                    su.UnitName.Substring(1, su.UnitName.Length - 1).Equals(ppu.UnitName.Substring(1, ppu.UnitName.Length - 1))) ||
                    (su.UnitName.Length == 0 && ppu.UnitName.Length == 0)
                select s
            )
            .ToList();
        }
    }
}