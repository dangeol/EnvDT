using EnvDT.Model.Core.HelperEntity;
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

        public IEnumerable<SampleValue> GetSampleValuesBySampleIdAndLabReportParamId(Guid sampleId, Guid labReportParamId)
        {
            return Context.Set<SampleValue>().AsNoTracking().ToList()
                .Where(sv => (sv.SampleId == sampleId && sv.LabReportParamId == labReportParamId));
        }

        public IEnumerable<SampleValueAndLrUnitName> GetSampleValuesAndLrUnitNamesByLabReportIdParamNameDeAndUnitName(
            Guid labReportId, string paramNameDe, string unitName)
        {
            return
            (
                from sv in Context.SampleValues
                join lp in Context.LabReportParams on sv.LabReportParamId equals lp.LabReportParamId
                where (lp.LabReportId == labReportId)
                join u in Context.Units on lp.UnitId equals u.UnitId
                where (u.UnitName.Length > 0 &&
                    u.UnitName.Substring(1, u.UnitName.Length - 1).Equals(unitName.Substring(1, unitName.Length - 1))) ||
                    (u.UnitName.Length == 0 && unitName.Length == 0)
                join p in Context.Parameters on lp.ParameterId equals p.ParameterId
                where (p.ParamNameDe.Equals(paramNameDe))
                select new SampleValueAndLrUnitName
                {
                    sampleValue = sv,
                    unitName = u.UnitName
                }
            )
            .AsNoTracking().ToList();
        }        
    }
}