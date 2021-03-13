using EnvDT.Model.Entity;
using EnvDT.Model.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EnvDT.DataAccess.Repository
{
    public class PublParamRepository : GenericRepository<PublParam, EnvDTDbContext>,
        IPublParamRepository
    {
        public PublParamRepository(EnvDTDbContext context)
            :base(context)
        {
        }

        public override PublParam GetById(Guid projectParamId)
        {
            return Context.PublParams.AsNoTracking()
                .Include(pp => pp.RefValues)
                .FirstOrDefault(pp => pp.PublParamId == projectParamId);
        }

        public PublParam GetByPublIdParameterNameDeAndUnitName(Guid publicationId, string paramNameDe, string unitName)
        {
            return
           (
               from pp in Context.PublParams
               where (pp.PublicationId.Equals(publicationId))
               join p in Context.Parameters on pp.ParameterId equals p.ParameterId
               where (p.ParamNameDe.Equals(paramNameDe))
               join u in Context.Units on pp.UnitId equals u.UnitId
               where (u.UnitName.Length > 0 &&
                   u.UnitName.Substring(1, u.UnitName.Length - 1).Equals(unitName.Substring(1, unitName.Length - 1))) ||
                   (u.UnitName.Length == 0 && unitName.Length == 0)
               select pp
           )
           .AsNoTracking().SingleOrDefault();
        }
    }
}