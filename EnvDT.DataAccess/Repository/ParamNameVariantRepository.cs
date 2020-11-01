using EnvDT.Model.Entity;
using EnvDT.Model.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace EnvDT.DataAccess.Repository
{
    public class ParamNameVariantRepository : GenericRepository<ParamNameVariant, EnvDTDbContext>,
        IParamNameVariantRepository
    {
        public ParamNameVariantRepository(EnvDTDbContext context)
            :base(context)
        {
        }

        public IEnumerable<ParamNameVariant> GetParamNameVariantsByLabParamName(string labParamName)
        {
            return Context.Set<ParamNameVariant>().AsNoTracking().ToList()
                .Where(pl => pl.ParamNameAlias == labParamName);
        }
    }
}