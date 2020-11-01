using EnvDT.Model.Entity;
using EnvDT.Model.IRepository;

namespace EnvDT.DataAccess.Repository
{
    public class UnitNameVariantRepository : GenericRepository<UnitNameVariant, EnvDTDbContext>,
        IUnitNameVariantRepository
    {
        public UnitNameVariantRepository(EnvDTDbContext context)
            :base(context)
        {
        }
    }
}