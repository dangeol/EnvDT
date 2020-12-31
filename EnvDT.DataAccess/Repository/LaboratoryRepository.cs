using EnvDT.Model.Entity;
using EnvDT.Model.IRepository;

namespace EnvDT.DataAccess.Repository
{
    public class LaboratoryRepository : GenericRepository<Laboratory, EnvDTDbContext>,
        ILaboratoryRepository
    {
        public LaboratoryRepository(EnvDTDbContext context)
            :base(context)
        {
        }
    }
}