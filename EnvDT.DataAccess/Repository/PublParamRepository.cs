using EnvDT.Model.Entity;
using EnvDT.Model.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
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

        public override PublParam GetById(Guid? projectParamId)
        {
            return Context.PublParams.AsNoTracking()
                .Include(pp => pp.RefValues)
                .FirstOrDefault(pp => pp.PublParamId == projectParamId);
        }
    }
}