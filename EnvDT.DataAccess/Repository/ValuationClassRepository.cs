using EnvDT.Model.Entity;
using EnvDT.Model.IRepository;
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

        public string getValClassNameNextLevelFromLevel(int level, Guid publicationId)
        {
            return Context.ValuationClasses
                .FirstOrDefault(v => v.ValClassLevel == level + 1 && v.PublicationId == publicationId)?
                .ValuationClassName ?? string.Empty;
        }
    }
}