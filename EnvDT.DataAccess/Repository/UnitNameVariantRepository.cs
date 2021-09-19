using EnvDT.Model.Entity;
using EnvDT.Model.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace EnvDT.DataAccess.Repository
{
    public class UnitNameVariantRepository : GenericRepository<UnitNameVariant, EnvDTDbContext>,
        IUnitNameVariantRepository
    {
        public UnitNameVariantRepository(EnvDTDbContext context)
            :base(context)
        {
        }

        public UnitNameVariant GetUnitNameVariantByLabParamUnitName(string labParamUnitName)
        {
            return Context.UnitNameVariants.AsNoTracking()
                .FirstOrDefault(uv => uv.UnitNameAlias == labParamUnitName);
        }
    }
}