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

        public IEnumerable<RefValue> GetRefValuesByPublicationId(Guid publicationId)
        {
            return
            (
                from rv in Context.RefValues
                join pp in Context.PublParams on rv.PublParamId equals pp.PublParamId
                where (pp.PublicationId == publicationId)
                select rv
            )
            .ToList();
        }
    }
}

