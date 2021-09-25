using EnvDT.Model.Entity;
using EnvDT.Model.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace EnvDT.DataAccess.Repository
{
    public class ValuationClassRepository : GenericRepository<ValuationClass, EnvDTDbContext>,
        IValuationClassRepository
    {
        public ValuationClassRepository(EnvDTDbContext context)
            :base(context)
        {
        }

        public string GetValClassNameNextLevelFromLevel(int level, Guid publicationId)
        {
            return Context.ValuationClasses.AsNoTracking()
                .FirstOrDefault(v => v.ValClassLevel == level + 1 && v.PublicationId == publicationId && !v.IsGroupClass)?
                .ValuationClassName ?? string.Empty;
        }
    }
}