using EnvDT.Model.Entity;
using EnvDT.Model.IRepository;

namespace EnvDT.DataAccess.Repository
{
    public class WasteCodeEWCRepository : GenericRepository<WasteCodeEWC, EnvDTDbContext>,
        IWasteCodeEWCRepository
    {
        public WasteCodeEWCRepository(EnvDTDbContext context)
            :base(context)
        {
        }
    }
}