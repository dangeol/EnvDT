using EnvDT.Model.Entity;
using EnvDT.Model.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EnvDT.DataAccess.Repository
{
    public class RefValueRepository : GenericRepository<RefValue, EnvDTDbContext>,
        IRefValueRepository
    {
        public RefValueRepository(EnvDTDbContext context)
            :base(context)
        {
        }

        public IEnumerable<RefValue> GetRefValuesByPublParamId(Guid publParamId)
        {
            return Context.Set<RefValue>().AsNoTracking().ToList()
                .Where(r => r.PublParamId == publParamId);
        }
    }
}

