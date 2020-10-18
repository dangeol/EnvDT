using EnvDT.Model.Entity;
using EnvDT.Model.IRepository;

namespace EnvDT.DataAccess.Repository
{
    public class UnitRepository : GenericRepository<Unit, EnvDTDbContext>,
        IUnitRepository
    {
        public UnitRepository(EnvDTDbContext context)
            :base(context)
        {
        }
    }
}